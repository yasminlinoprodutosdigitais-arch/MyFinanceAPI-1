using Microsoft.Extensions.Options;
using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Email;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #50: autocadastro com confirmação de e-mail e recuperação de senha. As respostas não revelam
/// quem tem conta (o aviso vai por e-mail para o dono do endereço).
/// </summary>
public class AcessoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<ITokensUsuarioService> _tokens = new();
    private readonly List<MensagemEmail> _enviados = [];
    private readonly Mock<IEnvioEmail> _email = new();
    private Usuario? _cadastrado;

    public AcessoServiceTests()
    {
        _email.Setup(e => e.EnviarAsync(It.IsAny<MensagemEmail>())).Callback<MensagemEmail>(_enviados.Add).Returns(Task.CompletedTask);
        _usuarios.Setup(r => r.Cadastrar(It.IsAny<Usuario>())).Callback<Usuario>(u => { u.Id = 40; _cadastrado = u; });
        _tokens.Setup(t => t.GerarAsync(It.IsAny<int>(), It.IsAny<TipoTokenUsuario>())).ReturnsAsync("tok+en/1");
    }

    private AcessoService Servico() => new(_usuarios.Object, _tokens.Object, _email.Object,
        Options.Create(new EmailSettings { UrlFront = "https://app/#/" }), TimeProvider.System);

    private static Usuario Existente(bool confirmado, string email = "ana@exemplo.com") =>
        new("ana@exemplo.com", "Senha123", "Ana", Perfis.Pessoa, true, DateTime.UtcNow, DateTime.UtcNow)
        { Id = 7, Email = email, EmailConfirmed = confirmado };

    // ---------- Cadastro ----------

    [Fact]
    public async Task Cadastro_cria_Pessoa_com_o_usuario_escolhido_e_manda_o_link()
    {
        await Servico().CadastrarAsync(new CadastroDto("  Ana Lima ", " Ana.Lima ", " Ana@Exemplo.COM ", "Senha123"));

        Assert.NotNull(_cadastrado);
        Assert.Equal("ana.lima", _cadastrado!.UserName);
        Assert.Equal("ANA.LIMA", _cadastrado.NormalizedUserName);
        Assert.Equal("ana@exemplo.com", _cadastrado.Email);
        Assert.Equal("ANA@EXEMPLO.COM", _cadastrado.NormalizedEmail);
        Assert.Equal("Ana Lima", _cadastrado.NomeUsuario);
        Assert.Equal(Perfis.Pessoa, _cadastrado.Perfil);
        Assert.False(_cadastrado.EmailConfirmed);
        Assert.NotEqual("Senha123", _cadastrado.PasswordHash);

        _tokens.Verify(t => t.GerarAsync(40, TipoTokenUsuario.ConfirmarEmail));
        var email = Assert.Single(_enviados);
        Assert.Equal("ana@exemplo.com", email.Para);
        Assert.Contains("https://app/#/login/confirmar-email?token=tok%2Ben%2F1", email.Html);
    }

    [Theory]
    [InlineData("curta1")]
    [InlineData("semnumeros")]
    [InlineData("12345678")]
    public async Task Senha_fraca_e_recusada(string senha)
    {
        var erro = await Assert.ThrowsAsync<ArgumentException>(() => Servico().CadastrarAsync(new CadastroDto("Ana", "ana.lima", "ana@exemplo.com", senha)));

        Assert.Equal("A senha precisa ter pelo menos 8 caracteres, com letras e números.", erro.Message);
        Assert.Null(_cadastrado);
    }

    [Theory]
    [InlineData("ana")]
    [InlineData("ana@")]
    [InlineData("")]
    public async Task Email_invalido_e_recusado(string email)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => Servico().CadastrarAsync(new CadastroDto("Ana", "ana.lima", email, "Senha123")));
    }

    /// <summary>Card #84: de 3 a 30, começando por letra; só letras sem acento, números, ".", "_" e "-".</summary>
    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("1ana")]
    [InlineData("ana@lima")]
    [InlineData("ana lima")]
    [InlineData("anã")]
    [InlineData("a123456789012345678901234567890")]
    public async Task Nome_de_usuario_fora_das_regras_e_recusado(string usuario)
    {
        var erro = await Assert.ThrowsAsync<ArgumentException>(() =>
            Servico().CadastrarAsync(new CadastroDto("Ana", usuario, "ana@exemplo.com", "Senha123")));

        Assert.StartsWith("O nome de usuário", erro.Message);
        Assert.Null(_cadastrado);
    }

    [Theory]
    [InlineData("ana")]
    [InlineData("ana.lima_2-b")]
    [InlineData("a12345678901234567890123456789")]
    public async Task Nome_de_usuario_dentro_das_regras_e_aceito(string usuario)
    {
        await Servico().CadastrarAsync(new CadastroDto("Ana", usuario, "ana@exemplo.com", "Senha123"));

        Assert.Equal(usuario, _cadastrado?.UserName);
    }

    [Fact]
    public async Task Nome_de_usuario_em_uso_e_recusado_sem_criar_nem_mandar_email()
    {
        _usuarios.Setup(r => r.LoginEmUsoAsync("ana.lima")).ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            Servico().CadastrarAsync(new CadastroDto("Ana", "Ana.Lima", "ana@exemplo.com", "Senha123")));

        Assert.Equal("Este nome de usuário já está em uso. Escolha outro.", erro.Message);
        Assert.Null(_cadastrado);
        Assert.Empty(_enviados);
    }

    /// <summary>E-mail já cadastrado responde igual de sempre, mesmo com o usuário em uso: a tela não revela o e-mail.</summary>
    [Fact]
    public async Task Email_ja_cadastrado_tem_prioridade_sobre_usuario_em_uso()
    {
        _usuarios.Setup(r => r.BuscarPorEmailAsync("ana@exemplo.com")).ReturnsAsync(Existente(confirmado: true));
        _usuarios.Setup(r => r.LoginEmUsoAsync("ana.lima")).ReturnsAsync(true);

        await Servico().CadastrarAsync(new CadastroDto("Ana", "ana.lima", "ana@exemplo.com", "Senha123"));

        Assert.Equal("Você já tem uma conta no Kanfy", Assert.Single(_enviados).Assunto);
    }

    [Fact]
    public async Task Nome_vazio_e_recusado()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => Servico().CadastrarAsync(new CadastroDto("  ", "ana.lima", "ana@exemplo.com", "Senha123")));
    }

    [Fact]
    public async Task Email_com_conta_confirmada_nao_cria_e_avisa_o_dono()
    {
        _usuarios.Setup(r => r.BuscarPorEmailAsync("ana@exemplo.com")).ReturnsAsync(Existente(confirmado: true));

        await Servico().CadastrarAsync(new CadastroDto("Outra", "outra", "ana@exemplo.com", "Senha123"));

        Assert.Null(_cadastrado);
        var email = Assert.Single(_enviados);
        Assert.Equal("Você já tem uma conta no Kanfy", email.Assunto);
        Assert.Contains("https://app/#/login/recuperar-senha", email.Html);
    }

    [Fact]
    public async Task Email_com_conta_sem_confirmar_reenvia_o_link()
    {
        _usuarios.Setup(r => r.BuscarPorEmailAsync("ana@exemplo.com")).ReturnsAsync(Existente(confirmado: false));

        await Servico().CadastrarAsync(new CadastroDto("Ana", "ana.lima", "ana@exemplo.com", "Senha123"));

        Assert.Null(_cadastrado);
        _tokens.Verify(t => t.GerarAsync(7, TipoTokenUsuario.ConfirmarEmail));
        Assert.Contains("confirmar-email", Assert.Single(_enviados).Html);
    }

    // ---------- Confirmar e reenviar ----------

    [Fact]
    public async Task Confirmar_com_token_valido_confirma_o_email()
    {
        var ana = Existente(confirmado: false);
        _tokens.Setup(t => t.ConsumirAsync("abc", TipoTokenUsuario.ConfirmarEmail)).ReturnsAsync(7);
        _usuarios.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(ana);

        Assert.True(await Servico().ConfirmarEmailAsync("abc"));
        Assert.True(ana.EmailConfirmed);
        _usuarios.Verify(r => r.AtualizarAsync(ana));
    }

    [Fact]
    public async Task Confirmar_com_token_invalido_nao_confirma()
    {
        Assert.False(await Servico().ConfirmarEmailAsync("vencido"));
        _usuarios.Verify(r => r.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task Reenviar_so_manda_para_conta_sem_confirmar()
    {
        _usuarios.Setup(r => r.BuscarPorEmailAsync("ana@exemplo.com")).ReturnsAsync(Existente(confirmado: true));
        _usuarios.Setup(r => r.BuscarPorEmailAsync("bia@exemplo.com")).ReturnsAsync(Existente(confirmado: false, email: "bia@exemplo.com"));

        await Servico().ReenviarConfirmacaoAsync("ana@exemplo.com");
        await Servico().ReenviarConfirmacaoAsync("ninguem@exemplo.com");
        await Servico().ReenviarConfirmacaoAsync(" BIA@exemplo.com ");

        Assert.Equal("bia@exemplo.com", Assert.Single(_enviados).Para);
    }

    // ---------- Esqueci e redefinir ----------

    [Fact]
    public async Task Esqueci_manda_o_link_so_para_conta_confirmada()
    {
        _usuarios.Setup(r => r.BuscarPorEmailAsync("ana@exemplo.com")).ReturnsAsync(Existente(confirmado: true));
        _usuarios.Setup(r => r.BuscarPorEmailAsync("bia@exemplo.com")).ReturnsAsync(Existente(confirmado: false, email: "bia@exemplo.com"));

        await Servico().EsqueciSenhaAsync("ana@exemplo.com");
        await Servico().EsqueciSenhaAsync("bia@exemplo.com");
        await Servico().EsqueciSenhaAsync("ninguem@exemplo.com");

        var email = Assert.Single(_enviados);
        Assert.Equal("ana@exemplo.com", email.Para);
        Assert.Contains("https://app/#/login/redefinir-senha?token=", email.Html);
        _tokens.Verify(t => t.GerarAsync(7, TipoTokenUsuario.RedefinirSenha));
    }

    [Fact]
    public async Task Redefinir_troca_a_senha()
    {
        var ana = Existente(confirmado: true);
        var hashAntigo = ana.PasswordHash;
        _tokens.Setup(t => t.ConsumirAsync("abc", TipoTokenUsuario.RedefinirSenha)).ReturnsAsync(7);
        _usuarios.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(ana);

        Assert.True(await Servico().RedefinirSenhaAsync(new RedefinirSenhaDto("abc", "NovaSenha9")));
        Assert.NotEqual(hashAntigo, ana.PasswordHash);
        _usuarios.Verify(r => r.AtualizarAsync(ana));
    }

    [Fact]
    public async Task Redefinir_com_senha_fraca_nao_gasta_o_token()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => Servico().RedefinirSenhaAsync(new RedefinirSenhaDto("abc", "fraca")));
        _tokens.Verify(t => t.ConsumirAsync(It.IsAny<string>(), It.IsAny<TipoTokenUsuario>()), Times.Never);
    }

    [Fact]
    public async Task Redefinir_com_token_invalido_nao_troca()
    {
        Assert.False(await Servico().RedefinirSenhaAsync(new RedefinirSenhaDto("vencido", "NovaSenha9")));
        _usuarios.Verify(r => r.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }
}
