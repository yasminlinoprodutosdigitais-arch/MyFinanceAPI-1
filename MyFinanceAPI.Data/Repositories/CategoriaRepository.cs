using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class CategoriaRepository(ContextDB context) : RepositorioDoUsuario<Categoria>(context), ICategoriaRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Categoria não encontrada.";

    protected override Expression<Func<Categoria, bool>> Proprios(int userId) => c => c.UsuarioId == userId;

    protected override IOrderedQueryable<Categoria> Ordenar(IQueryable<Categoria> query) => query.OrderBy(c => c.Nome);

    protected override void CopiarAlteracoes(Categoria existente, Categoria entrada)
    {
        existente.Nome = entrada.Nome;
        existente.Subcategoria = entrada.Subcategoria;
        existente.NaturezaOperacao = entrada.NaturezaOperacao;
        existente.Ativo = entrada.Ativo;
    }

    public async Task<IEnumerable<Categoria>> GetCategoriesByUserIdAtiva(int userId)
    {
        var categories = await _context.Categorias.AsNoTracking().Where(c => c.UsuarioId == userId && c.Ativo == StatusAtivo.Ativo).OrderBy(c => c.Nome).ToListAsync();
        return categories;
    }

    public async Task<bool> ExisteNomeAsync(int userId, string name, string subCategory, int ignorarId)
    {
        var nome = (name ?? string.Empty).Trim().ToUpper();
        var sub = (subCategory ?? string.Empty).Trim().ToUpper();
        return await _context.Categorias.AnyAsync(c => c.UsuarioId == userId && c.Id != ignorarId
            && c.Nome.Trim().ToUpper() == nome && c.Subcategoria.Trim().ToUpper() == sub);
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.Categorias.AnyAsync(c => c.Id == id && c.UsuarioId == userId))
            return false;

        return await _context.Transacoes.AnyAsync(t => t.CategoriaId == id)
            || await _context.PessoasMovimentacao.AnyAsync(p => p.CategoriaId == id)
            || await _context.ItensExtratoBancario.AnyAsync(i => i.CategoriaId == id);
    }
}
