using Moq;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// DB-P1-07: "Cartão de crédito", "Conta corrente" e "Conta Poupança" são padrão do sistema
/// (UserId nulo), valem para todos e não podem ser editados nem excluídos. Tipos do próprio
/// usuário continuam editáveis.
/// </summary>
public class TipoCartaoServiceTests
{
    private const int UserId = 7;

    private readonly Mock<ITipoCartaoRepository> _repo = new();

    private static readonly TipoCartao DoSistema = new() { Id = 1, UserId = null, NomeTipoCartao = "Cartão de crédito", EhCredito = true };
    private static readonly TipoCartao DoUsuario = new() { Id = 50, UserId = UserId, NomeTipoCartao = "Vale-refeição" };

    private TipoCartaoService CriarService() => new(_repo.Object, MapperFactory.Create());

    [Fact]
    public void Tipo_sem_usuario_e_do_sistema()
    {
        Assert.True(DoSistema.EhSistema);
        Assert.False(DoUsuario.EhSistema);
    }

    [Fact]
    public async Task Editar_tipo_do_sistema_e_recusado()
    {
        _repo.Setup(r => r.ObterPorIdAsync(1, UserId)).ReturnsAsync(DoSistema);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AtualizarAsync(new TipoCartaoDTO { Id = 1, NomeTipoCartao = "Crédito" }, UserId));

        Assert.Contains("padrão do sistema", erro.Message);
        _repo.Verify(r => r.AtualizarAsync(It.IsAny<TipoCartao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_tipo_do_sistema_e_recusado()
    {
        _repo.Setup(r => r.ObterPorIdAsync(1, UserId)).ReturnsAsync(DoSistema);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().RemoverAsync(1, UserId));

        Assert.Contains("padrão do sistema", erro.Message);
        _repo.Verify(r => r.RemoverAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Editar_tipo_proprio_continua_permitido()
    {
        _repo.Setup(r => r.ObterPorIdAsync(50, UserId)).ReturnsAsync(DoUsuario);

        await CriarService().AtualizarAsync(new TipoCartaoDTO { Id = 50, NomeTipoCartao = "VR", EhCredito = false }, UserId);

        _repo.Verify(r => r.AtualizarAsync(It.Is<TipoCartao>(t => t.Id == 50 && t.NomeTipoCartao == "VR"), UserId), Times.Once);
    }

    [Fact]
    public async Task Excluir_tipo_proprio_sem_uso_continua_permitido()
    {
        _repo.Setup(r => r.ObterPorIdAsync(50, UserId)).ReturnsAsync(DoUsuario);

        await CriarService().RemoverAsync(50, UserId);

        _repo.Verify(r => r.RemoverAsync(50, UserId), Times.Once);
    }

    [Fact]
    public async Task Criar_tipo_com_nome_de_um_padrao_e_recusado()
    {
        // O repositório considera também os tipos do sistema ao checar o nome.
        _repo.Setup(r => r.ExisteNomeAsync(UserId, "cartão de crédito", 0)).ReturnsAsync(true);

        await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AdicionarAsync(new TipoCartaoDTO { NomeTipoCartao = "cartão de crédito" }, UserId));

        _repo.Verify(r => r.CriarAsync(It.IsAny<TipoCartao>()), Times.Never);
    }
}
