using System;
using System.Linq.Expressions;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Repositories
{
    /// <summary>Filtros de ExtratoBancarioItem, separados para serem testados sem banco.</summary>
    public static class ExtratoBancarioItemFiltros
    {
        /// <summary>
        /// Itens do mês, para o extrato de um banco. Cartão de crédito vem pela fatura
        /// (a compra pode ter sido no mês anterior); o resto, pela data de movimentação.
        /// "É cartão de crédito" é TipoCartao.EhCredito — antes eram os IDs fixos 1 e 13 (DB-P1-07).
        /// </summary>
        public static Expression<Func<ExtratoBancarioItem, bool>> DoMesOuDaFatura(
            int userId, DateOnly inicioInclusive, DateOnly fimExclusive, string? numeroFatura, int? bancoId) =>
            i => i.UserId == userId
                 && (bancoId == null || i.BancoId == bancoId)
                 && (i.TipoCartao != null && i.TipoCartao.EhCredito
                     ? i.NumeroFatura == numeroFatura
                     : i.DataMovimentacao >= inicioInclusive && i.DataMovimentacao < fimExclusive);
    }
}
