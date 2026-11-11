using Moq;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #50: o link do e-mail leva um token aleatório; o banco guarda só o hash. Vale uma vez, vence
/// (24h para confirmar o e-mail, 30 min para redefinir a senha) e um token novo invalida os anteriores.
/// </summary>
public class TokensUsuarioServiceTests
{
    private sealed class RelogioFalso(DateTimeOffset agora) : TimeProvider
    {
        public DateTimeOffset Agora { get; set; } = agora;
        public override DateTimeOffset GetUtcNow() => Agora;
    }

    private readonly Mock<ITokenUsuarioRepository> _repo = new();
    private readonly RelogioFalso _relogio = new(new DateTimeOffset(2026, 11, 11, 12, 0, 0, TimeSpan.Zero));
    private readonly List<TokenUsuario> _gravados = [];

    public TokensUsuarioServiceTests()
    {
        _repo.Setup(r => r.SubstituirAsync(It.IsAny<TokenUsuario>(), It.IsAny<DateTime>()))
            .Callback<TokenUsuario, DateTime>((t, _) => _gravados.Add(t))
            .Returns(Task.CompletedTask);
        _repo.Setup(r => r.ObterPorHashAsync(It.IsAny<string>(), It.IsAny<TipoTokenUsuario>()))
            .ReturnsAsync((string hash, TipoTokenUsuario tipo) => _gravados.SingleOrDefault(t => t.Hash == hash && t.Tipo == tipo));
    }

    private TokensUsuarioService Servico() => new(_repo.Object, _relogio);

    [Fact]
    public async Task Gera_token_e_guarda_so_o_hash_com_a_validade_do_tipo()
    {
        var token = await Servico().GerarAsync(7, TipoTokenUsuario.RedefinirSenha);

        var gravado = Assert.Single(_gravados);
        Assert.True(token.Length >= 40);
        Assert.NotEqual(token, gravado.Hash);
        Assert.Equal(7, gravado.UsuarioId);
        Assert.Equal(_relogio.Agora.UtcDateTime.AddMinutes(30), gravado.ExpiraEm);
        Assert.Null(gravado.UsadoEm);
    }

    [Fact]
    public async Task Confirmacao_de_email_vale_24_horas()
    {
        await Servico().GerarAsync(7, TipoTokenUsuario.ConfirmarEmail);

        Assert.Equal(_relogio.Agora.UtcDateTime.AddHours(24), Assert.Single(_gravados).ExpiraEm);
    }

    [Fact]
    public async Task Tokens_seguidos_sao_diferentes()
    {
        var servico = Servico();

        Assert.NotEqual(await servico.GerarAsync(7, TipoTokenUsuario.ConfirmarEmail), await servico.GerarAsync(7, TipoTokenUsuario.ConfirmarEmail));
    }

    [Fact]
    public async Task Consumir_devolve_o_usuario_e_marca_como_usado()
    {
        var servico = Servico();
        var token = await servico.GerarAsync(7, TipoTokenUsuario.RedefinirSenha);

        Assert.Equal(7, await servico.ConsumirAsync(token, TipoTokenUsuario.RedefinirSenha));
        _repo.Verify(r => r.MarcarUsadoAsync(_gravados[0], _relogio.Agora.UtcDateTime), Times.Once);
    }

    [Fact]
    public async Task Token_usado_nao_vale_de_novo()
    {
        var servico = Servico();
        var token = await servico.GerarAsync(7, TipoTokenUsuario.RedefinirSenha);
        _gravados[0].UsadoEm = _relogio.Agora.UtcDateTime;

        Assert.Null(await servico.ConsumirAsync(token, TipoTokenUsuario.RedefinirSenha));
    }

    [Fact]
    public async Task Token_vencido_nao_vale()
    {
        var servico = Servico();
        var token = await servico.GerarAsync(7, TipoTokenUsuario.RedefinirSenha);
        _relogio.Agora = _relogio.Agora.AddMinutes(31);

        Assert.Null(await servico.ConsumirAsync(token, TipoTokenUsuario.RedefinirSenha));
        _repo.Verify(r => r.MarcarUsadoAsync(It.IsAny<TokenUsuario>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task Token_de_um_tipo_nao_serve_para_o_outro()
    {
        var servico = Servico();
        var token = await servico.GerarAsync(7, TipoTokenUsuario.ConfirmarEmail);

        Assert.Null(await servico.ConsumirAsync(token, TipoTokenUsuario.RedefinirSenha));
    }

    [Theory]
    [InlineData("")]
    [InlineData("inventado")]
    public async Task Token_desconhecido_nao_vale(string token)
    {
        Assert.Null(await Servico().ConsumirAsync(token, TipoTokenUsuario.ConfirmarEmail));
    }
}
