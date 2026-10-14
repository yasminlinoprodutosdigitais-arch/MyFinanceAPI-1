using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// DB-P2-03: o mês é filtrado por intervalo meio-aberto (Date &gt;= dia 1 e &lt; dia 1 do mês seguinte),
/// que o índice (UserId, Date) consegue usar. Date.Month/Date.Year viravam EXTRACT() sobre a coluna.
/// </summary>
public class TransactionFiltrosTests
{
    private const int UserId = 42;

    private static Transaction T(int id, DateOnly data, int userId = UserId) =>
        new() { Id = id, UserId = userId, Date = data, Name = "x", Status = StatusTransacao.Pendente };

    private static int[] Filtrar(IEnumerable<Transaction> itens, DateOnly dia, bool diaExato) =>
        itens.AsQueryable()
            .Where(TransactionFiltros.DoDiaOuDoMes(UserId, dia, diaExato))
            .Select(t => t.Id)
            .OrderBy(i => i)
            .ToArray();

    [Fact]
    public void Mes_inclui_do_dia_1_ao_ultimo_dia_e_nada_fora()
    {
        var itens = new[]
        {
            T(1, new DateOnly(2026, 9, 30)),
            T(2, new DateOnly(2026, 10, 1)),
            T(3, new DateOnly(2026, 10, 31)),
            T(4, new DateOnly(2026, 11, 1)),
            T(5, new DateOnly(2025, 10, 15)), // mesmo mês, outro ano
        };

        Assert.Equal([2, 3], Filtrar(itens, new DateOnly(2026, 10, 13), diaExato: false));
    }

    [Fact]
    public void Dezembro_vai_ate_31_e_nao_pega_janeiro_do_ano_seguinte()
    {
        var itens = new[]
        {
            T(1, new DateOnly(2026, 12, 31)),
            T(2, new DateOnly(2027, 1, 1)),
        };

        Assert.Equal([1], Filtrar(itens, new DateOnly(2026, 12, 5), diaExato: false));
    }

    [Fact]
    public void Dia_exato_traz_so_aquele_dia()
    {
        var itens = new[]
        {
            T(1, new DateOnly(2026, 10, 12)),
            T(2, new DateOnly(2026, 10, 13)),
            T(3, new DateOnly(2026, 10, 14)),
        };

        Assert.Equal([2], Filtrar(itens, new DateOnly(2026, 10, 13), diaExato: true));
    }

    [Fact]
    public void Outro_usuario_fica_de_fora()
    {
        var itens = new[] { T(1, new DateOnly(2026, 10, 5)), T(2, new DateOnly(2026, 10, 5), userId: 7) };

        Assert.Equal([1], Filtrar(itens, new DateOnly(2026, 10, 1), diaExato: false));
    }

    [Fact]
    public void O_filtro_do_mes_nao_aplica_funcao_sobre_a_coluna()
    {
        var expressao = TransactionFiltros.DoDiaOuDoMes(UserId, new DateOnly(2026, 10, 1), diaExato: false).ToString();

        Assert.DoesNotContain(".Month", expressao);
        Assert.DoesNotContain(".Year", expressao);
    }

    [Fact]
    public void Intervalo_do_mes_e_meio_aberto()
    {
        var (inicio, fimExclusivo) = TransactionFiltros.IntervaloDoMes(new DateOnly(2024, 2, 29));

        Assert.Equal(new DateOnly(2024, 2, 1), inicio);
        Assert.Equal(new DateOnly(2024, 3, 1), fimExclusivo);
    }
}
