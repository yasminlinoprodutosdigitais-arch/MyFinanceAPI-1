using System.Net.Mail;
using Microsoft.Extensions.Options;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Email;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #50. Quem se cadastra entra com o e-mail como login e perfil Pessoa, e só consegue entrar depois de
/// confirmar o e-mail. Cadastro com e-mail que já tem conta não cria nada: o dono recebe um aviso (ou o link
/// de confirmação de novo, se ainda não confirmou).
/// </summary>
public class AcessoService(
    IUsuarioRepository usuarios,
    ITokensUsuarioService tokens,
    IEnvioEmail email,
    IOptions<EmailSettings> configuracao,
    TimeProvider relogio) : IAcessoService
{
    private const string MensagemSenhaFraca = "A senha precisa ter pelo menos 8 caracteres, com letras e números.";

    public async Task CadastrarAsync(CadastroDto dados)
    {
        var nome = dados.Nome?.Trim() ?? "";
        if (nome.Length == 0)
            throw new ArgumentException("Informe seu nome.");
        var endereco = EmailValido(dados.Email);
        ValidarSenha(dados.Senha);

        var existente = await usuarios.BuscarPorEmailAsync(endereco);
        if (existente != null)
        {
            if (existente.EmailConfirmed)
                await email.EnviarAsync(ModelosEmail.ContaJaExiste(endereco, existente.NomeUsuario, Link("login"), Link("login/recuperar-senha")));
            else
                await EnviarConfirmacaoAsync(existente);
            return;
        }

        var agora = relogio.GetUtcNow().UtcDateTime;
        var usuario = new Usuario(endereco, dados.Senha, nome, Perfis.Pessoa, true, agora, agora)
        {
            NormalizedUserName = endereco.ToUpperInvariant(),
            NormalizedEmail = endereco.ToUpperInvariant(),
            EmailConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };
        usuarios.Cadastrar(usuario);
        await EnviarConfirmacaoAsync(usuario);
    }

    public async Task<bool> ConfirmarEmailAsync(string token)
    {
        var usuario = await UsuarioDoLinkAsync(token, TipoTokenUsuario.ConfirmarEmail);
        if (usuario == null)
            return false;

        usuario.EmailConfirmed = true;
        await usuarios.AtualizarAsync(usuario);
        return true;
    }

    public async Task ReenviarConfirmacaoAsync(string endereco)
    {
        var usuario = await UsuarioPorEmailAsync(endereco);
        if (usuario is { EmailConfirmed: false })
            await EnviarConfirmacaoAsync(usuario);
    }

    public async Task EsqueciSenhaAsync(string endereco)
    {
        // Só conta confirmada: sem isso, o "esqueci" serviria para confirmar um e-mail que não é de quem cadastrou.
        var usuario = await UsuarioPorEmailAsync(endereco);
        if (usuario is not { EmailConfirmed: true } || string.IsNullOrWhiteSpace(usuario.Email))
            return;

        var token = await tokens.GerarAsync(usuario.Id, TipoTokenUsuario.RedefinirSenha);
        await email.EnviarAsync(ModelosEmail.RedefinirSenha(usuario.Email, usuario.NomeUsuario, Link("login/redefinir-senha", token)));
    }

    public async Task<bool> RedefinirSenhaAsync(RedefinirSenhaDto dados)
    {
        ValidarSenha(dados.Senha);
        var usuario = await UsuarioDoLinkAsync(dados.Token, TipoTokenUsuario.RedefinirSenha);
        if (usuario == null)
            return false;

        usuario.SetSenhaHash(dados.Senha);
        usuario.SecurityStamp = Guid.NewGuid().ToString("N");
        await usuarios.AtualizarAsync(usuario);
        return true;
    }

    private async Task EnviarConfirmacaoAsync(Usuario usuario)
    {
        var token = await tokens.GerarAsync(usuario.Id, TipoTokenUsuario.ConfirmarEmail);
        await email.EnviarAsync(ModelosEmail.ConfirmarEmail(usuario.Email!, usuario.NomeUsuario, Link("login/confirmar-email", token)));
    }

    private async Task<Usuario?> UsuarioDoLinkAsync(string token, TipoTokenUsuario tipo)
    {
        var usuarioId = await tokens.ConsumirAsync(token, tipo);
        return usuarioId == null ? null : await usuarios.ObterPorIdAsync(usuarioId.Value);
    }

    /// <summary>E-mail mal escrito nos pedidos anônimos não é erro: só não há para quem mandar.</summary>
    private async Task<Usuario?> UsuarioPorEmailAsync(string? endereco)
    {
        var limpo = endereco?.Trim().ToLowerInvariant() ?? "";
        return limpo.Length == 0 ? null : await usuarios.BuscarPorEmailAsync(limpo);
    }

    private string Link(string caminho, string? token = null)
    {
        var link = $"{configuracao.Value.UrlFront.TrimEnd('/')}/{caminho}";
        return token == null ? link : $"{link}?token={Uri.EscapeDataString(token)}";
    }

    private static string EmailValido(string? endereco)
    {
        var limpo = endereco?.Trim().ToLowerInvariant() ?? "";
        var valido = MailAddress.TryCreate(limpo, out var lido) && lido.Address == limpo && lido.Host.Contains('.');
        return valido ? limpo : throw new ArgumentException("Informe um e-mail válido.");
    }

    private static void ValidarSenha(string? senha)
    {
        if (senha is not { Length: >= 8 } || !senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
            throw new ArgumentException(MensagemSenhaFraca);
    }
}
