using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

/// <summary>
/// O usuário enxerga os tipos padrão do sistema (UserId nulo), mas só altera e remove os próprios (DB-P1-07).
/// </summary>
public class TipoCartaoRepository(ContextDB context) : RepositorioDoUsuario<TipoCartao>(context), ITipoCartaoRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Tipo de cartão não encontrado.";

    protected override Expression<Func<TipoCartao, bool>> Proprios(int userId) => t => t.UserId == userId;

    protected override Expression<Func<TipoCartao, bool>> Visiveis(int userId) => t => t.UserId == userId || t.UserId == null;

    // Padrão do sistema primeiro.
    protected override IOrderedQueryable<TipoCartao> Ordenar(IQueryable<TipoCartao> query) =>
        query.OrderBy(t => t.UserId != null).ThenBy(t => t.NomeTipoCartao);

    protected override void CopiarAlteracoes(TipoCartao existente, TipoCartao entrada)
    {
        existente.NomeTipoCartao = entrada.NomeTipoCartao;
        existente.Descricao = entrada.Descricao;
        existente.EhCredito = entrada.EhCredito;
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.TipoCartao.AnyAsync(t => t.Id == id && t.UserId == userId))
            return false;

        return await _context.Banco.AnyAsync(b => b.TipoCartaoId == id)
            || await _context.MovimentacaoDiaria.AnyAsync(m => m.TipoCartaoId == id)
            || await _context.ExtratoBancario.AnyAsync(e => e.TipoCartaoId == id)
            || await _context.ExtratoBancarioItens.AnyAsync(i => i.TipoCartaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(int userId, string nome, int ignorarId)
    {
        var alvo = (nome ?? string.Empty).Trim().ToUpper();
        // Inclui os padrão do sistema: o usuário não cria outro "Cartão de crédito" (DB-P1-07).
        return await _context.TipoCartao.AnyAsync(t => (t.UserId == userId || t.UserId == null) && t.Id != ignorarId
            && t.NomeTipoCartao.Trim().ToUpper() == alvo);
    }
}
