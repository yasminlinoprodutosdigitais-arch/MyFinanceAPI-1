using Moq;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #75: histórico, cobertura por mês e desfazer.</summary>
public class ImportacoesServiceTests
{
    private const int UserId = 7;
    private static readonly DateOnly Hoje = new(2026, 11, 8);
    private static readonly Dictionary<int, string> Nomes = new() { [1] = "NuBank", [2] = "Caixa" };

    private readonly Mock<IExtratoBancarioRepository> _extratos = new();
    private readonly Mock<IExtratoBancarioItemRepository> _itens = new();
    private readonly UnitOfWorkFalso _uow = new();

    private static ImportacaoResumo Imp(int banco, string mes, int qtd = 10, int semCategoria = 0, DateTime? desfeito = null) =>
        new(1, banco, new DateTime(2026, 11, 1), "a.csv", DateOnly.ParseExact(mes + "-01", "yyyy-MM-dd"), 0, desfeito, null, null, qtd, 0, 0, semCategoria, null);

    private ImportacoesService Servico() => new(_extratos.Object, _itens.Object, Mock.Of<IBancoService>(), _uow);

    private static string[] Estados(MyFinanceAPI.Application.DTO.Movimentacoes.CoberturaImportacoesDTO c, int conta) =>
        c.Contas[conta].Celulas.Select(x => x.Estado).ToArray();

    [Fact]
    public void Cobertura_marca_falta_so_entre_a_primeira_importacao_e_o_mes_fechado()
    {
        var c = ImportacoesService.MontarCobertura([Imp(1, "2026-08"), Imp(1, "2026-10")], Nomes, Hoje, 5);

        Assert.Equal(["2026-07", "2026-08", "2026-09", "2026-10", "2026-11"], c.Meses);
        Assert.Equal(["sem-dados", "importado", "falta", "importado", "sem-dados"], Estados(c, 0));
    }

    [Fact]
    public void Cobertura_soma_pendencias_ignora_desfeita_e_conta_sem_importacao()
    {
        var c = ImportacoesService.MontarCobertura(
            [Imp(1, "2026-10", 27, 2), Imp(1, "2026-10", 3), Imp(2, "2026-10", 5, 0, DateTime.UtcNow)], Nomes, Hoje, 2);

        Assert.Single(c.Contas);
        var outubro = c.Contas[0].Celulas[0];
        Assert.Equal(("pendencias", 30, 2), (outubro.Estado, outubro.Lancamentos, outubro.Pendencias));
    }

    [Fact]
    public void Fatura_futura_estende_a_janela()
    {
        var c = ImportacoesService.MontarCobertura([Imp(1, "2026-12")], Nomes, Hoje, 2);
        Assert.Equal(["2026-11", "2026-12"], c.Meses);
    }

    [Fact]
    public void Mes_vem_da_coluna_da_fatura_ou_do_inicio_do_periodo()
    {
        var baseR = Imp(1, "2026-10") with { MesReferencia = null };
        Assert.Equal(new DateOnly(2026, 9, 1), ImportacoesService.MesDe(baseR with { NumeroFatura = "2026-09" }));
        Assert.Equal(new DateOnly(2026, 8, 1), ImportacoesService.MesDe(baseR with { DataInicioPeriodo = new DateOnly(2026, 8, 15) }));
        Assert.Null(ImportacoesService.MesDe(baseR));
    }

    [Fact]
    public async Task Desfazer_remove_os_lancamentos_e_mantem_a_linha()
    {
        var extrato = new ExtratoBancario { Id = 3, UsuarioId = UserId, QuantidadeLancamentos = 2, ValorTotal = 50, Itens = [new ExtratoBancarioItem { NumeroFatura = "2026-09" }] };
        _extratos.Setup(r => r.GetByIdAsync(3, UserId)).ReturnsAsync(extrato);

        await Servico().DesfazerAsync(UserId, 3, "  arquivo errado ");

        _itens.Verify(r => r.RemoveByExtratoIdAsync(3), Times.Once);
        _extratos.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _extratos.Verify(r => r.UpdateAsync(extrato), Times.Once);
        Assert.NotNull(extrato.DesfeitoEm);
        Assert.Equal("arquivo errado", extrato.MotivoDesfazer);
        Assert.Equal(new DateOnly(2026, 9, 1), extrato.MesReferencia);
        Assert.Equal(0, extrato.QuantidadeLancamentos);
    }

    [Fact]
    public async Task Desfazer_duas_vezes_e_recusado_e_outro_usuario_nao_acha()
    {
        _extratos.Setup(r => r.GetByIdAsync(3, UserId)).ReturnsAsync(new ExtratoBancario { Id = 3, UsuarioId = UserId, DesfeitoEm = DateTime.UtcNow, Itens = [] });

        await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().DesfazerAsync(UserId, 3, null));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Servico().DesfazerAsync(UserId, 4, null));
        _itens.Verify(r => r.RemoveByExtratoIdAsync(It.IsAny<int>()), Times.Never);
    }
}
