using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// DB-P1-07: "é cartão de crédito" vem de TipoCartao.EhCredito, não de IDs fixos (1 e 13 só
/// existiam para os dois primeiros usuários). Cartão de crédito é filtrado pela fatura;
/// o resto, pela data de movimentação.
/// </summary>
public class ExtratoBancarioItemFiltrosTests
{
    private const int UserId = 42;
    private static readonly DateOnly Inicio = new(2026, 10, 1);
    private static readonly DateOnly Fim = new(2026, 11, 1);
    private const string Fatura = "2026-10";

    // IDs propositalmente diferentes de 1 e 13.
    private static readonly TipoCartao Credito = new() { Id = 77, UsuarioId = UserId, NomeTipoCartao = "Meu cartão", EhCredito = true };
    private static readonly TipoCartao Corrente = new() { Id = 78, UsuarioId = UserId, NomeTipoCartao = "Conta", EhCredito = false };

    private static ExtratoBancarioItem Item(int id, TipoCartao tipo, DateOnly data, string? fatura = null, int bancoId = 5, int userId = UserId) =>
        new()
        {
            Id = id,
            UsuarioId = userId,
            BancoId = bancoId,
            TipoCartaoId = tipo.Id,
            TipoCartao = tipo,
            DataMovimentacao = data,
            NumeroFatura = fatura,
            TipoLancamento = TipoLancamentoExtrato.Saida
        };

    private static int[] Filtrar(IEnumerable<ExtratoBancarioItem> itens, int? bancoId = null) =>
        itens.AsQueryable()
            .Where(ExtratoBancarioItemFiltros.DoMesOuDaFatura(UserId, Inicio, Fim, Fatura, bancoId))
            .Select(i => i.Id)
            .OrderBy(i => i)
            .ToArray();

    [Fact]
    public void Cartao_de_credito_e_filtrado_pela_fatura_mesmo_com_compra_em_outro_mes()
    {
        var itens = new[]
        {
            Item(1, Credito, new DateOnly(2026, 9, 28), fatura: "2026-10"),  // compra em setembro, fatura de outubro
            Item(2, Credito, new DateOnly(2026, 10, 5), fatura: "2026-11"),  // compra em outubro, fatura de novembro
        };

        Assert.Equal([1], Filtrar(itens));
    }

    [Fact]
    public void Conta_corrente_e_filtrada_pela_data_de_movimentacao()
    {
        var itens = new[]
        {
            Item(1, Corrente, new DateOnly(2026, 10, 1)),
            Item(2, Corrente, new DateOnly(2026, 10, 31)),
            Item(3, Corrente, new DateOnly(2026, 11, 1)),
            Item(4, Corrente, new DateOnly(2026, 9, 30), fatura: "2026-10"),  // fatura não vale para conta corrente
        };

        Assert.Equal([1, 2], Filtrar(itens));
    }

    [Fact]
    public void Nao_traz_itens_de_outro_usuario_nem_de_outro_banco()
    {
        var itens = new[]
        {
            Item(1, Credito, new DateOnly(2026, 10, 5), fatura: "2026-10"),
            Item(2, Credito, new DateOnly(2026, 10, 5), fatura: "2026-10", userId: 99),
            Item(3, Corrente, new DateOnly(2026, 10, 5), bancoId: 6),
            Item(4, Corrente, new DateOnly(2026, 10, 5)),
        };

        Assert.Equal([1, 4], Filtrar(itens, bancoId: 5));
        Assert.Equal([1, 3, 4], Filtrar(itens));
    }
}
