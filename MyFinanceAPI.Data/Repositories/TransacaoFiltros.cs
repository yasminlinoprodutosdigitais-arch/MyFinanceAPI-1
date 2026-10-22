using System.Linq.Expressions;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Repositories;

/// <summary>Filtros de Transactions, separados para serem testados sem banco.</summary>
public static class TransacaoFiltros
{
    /// <summary>Do dia 1 do mês (inclusive) ao dia 1 do mês seguinte (exclusive).</summary>
    public static (DateOnly Inicio, DateOnly FimExclusivo) IntervaloDoMes(DateOnly dia)
    {
        var inicio = new DateOnly(dia.Year, dia.Month, 1);
        return (inicio, inicio.AddMonths(1));
    }

    /// <summary>
    /// Transações do usuário no dia exato ou no mês do dia. O mês é um intervalo sobre a coluna,
    /// que o índice (UserId, Date) usa; Date.Month/Date.Year viravam EXTRACT() e liam a tabela inteira (DB-P2-03).
    /// </summary>
    public static Expression<Func<Transacao, bool>> DoDiaOuDoMes(int userId, DateOnly dia, bool diaExato)
    {
        if (diaExato)
            return t => t.UsuarioId == userId && t.Data == dia;

        var (inicio, fimExclusivo) = IntervaloDoMes(dia);
        return t => t.UsuarioId == userId && t.Data >= inicio && t.Data < fimExclusivo;
    }
}
