using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TipoMovimentacaoRepository(ContextDB context) : RepositorioDoUsuario<TipoMovimentacao>(context), ITipoMovimentacaoRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Tipo de movimentação não encontrado.";

    protected override Expression<Func<TipoMovimentacao, bool>> Proprios(int userId) => t => t.UsuarioId == userId;

    protected override IOrderedQueryable<TipoMovimentacao> Ordenar(IQueryable<TipoMovimentacao> query) =>
        query.OrderBy(t => t.NomeTipoMovimentacao);

    protected override void CopiarAlteracoes(TipoMovimentacao existente, TipoMovimentacao entrada)
    {
        existente.NomeTipoMovimentacao = entrada.NomeTipoMovimentacao;
        existente.Descricao = entrada.Descricao;
        existente.ValorMeta = entrada.ValorMeta;
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.TiposMovimentacao.AnyAsync(t => t.Id == id && t.UsuarioId == userId))
            return false;

        return await _context.MovimentacoesDiarias.AnyAsync(m => m.TipoMovimentacaoId == id)
            || await _context.ItensExtratoBancario.AnyAsync(i => i.TipoMovimentacaoId == id)
            || await _context.PessoasMovimentacao.AnyAsync(p => p.TipoMovimentacaoId == id);
    }
}
