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

    protected override Expression<Func<TipoCartao, bool>> Proprios(int userId) => t => t.UsuarioId == userId;

    protected override Expression<Func<TipoCartao, bool>> Visiveis(int userId) => t => t.UsuarioId == userId || t.UsuarioId == null;

    // Padrão do sistema primeiro.
    protected override IOrderedQueryable<TipoCartao> Ordenar(IQueryable<TipoCartao> query) =>
        query.OrderBy(t => t.UsuarioId != null).ThenBy(t => t.NomeTipoCartao);

    protected override void CopiarAlteracoes(TipoCartao existente, TipoCartao entrada)
    {
        existente.NomeTipoCartao = entrada.NomeTipoCartao;
        existente.Descricao = entrada.Descricao;
        existente.EhCredito = entrada.EhCredito;
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.TiposCartao.AnyAsync(t => t.Id == id && t.UsuarioId == userId))
            return false;

        return await _context.Bancos.AnyAsync(b => b.TipoCartaoId == id)
            || await _context.MovimentacoesDiarias.AnyAsync(m => m.TipoCartaoId == id)
            || await _context.ExtratosBancarios.AnyAsync(e => e.TipoCartaoId == id)
            || await _context.ItensExtratoBancario.AnyAsync(i => i.TipoCartaoId == id);
    }

    public async Task<bool> ExisteNomeAsync(int userId, string nome, int ignorarId)
    {
        var alvo = (nome ?? string.Empty).Trim().ToUpper();
        // Inclui os padrão do sistema: o usuário não cria outro "Cartão de crédito" (DB-P1-07).
        return await _context.TiposCartao.AnyAsync(t => (t.UsuarioId == userId || t.UsuarioId == null) && t.Id != ignorarId
            && t.NomeTipoCartao.Trim().ToUpper() == alvo);
    }
}
