using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class CategoryRepository(ContextDB context) : RepositorioDoUsuario<Category>(context), ICategoryRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Categoria não encontrada.";

    protected override Expression<Func<Category, bool>> Proprios(int userId) => c => c.UserId == userId;

    protected override IOrderedQueryable<Category> Ordenar(IQueryable<Category> query) => query.OrderBy(c => c.Name);

    protected override void CopiarAlteracoes(Category existente, Category entrada)
    {
        existente.Name = entrada.Name;
        existente.SubCategory = entrada.SubCategory;
        existente.NaturezaOperacao = entrada.NaturezaOperacao;
        existente.Status = entrada.Status;
    }

    public async Task<IEnumerable<Category>> GetCategoriesByUserIdAtiva(int userId)
    {
        var categories = await _context.Categories.Where(c => c.UserId == userId && c.Status == StatusAtivo.Ativo).OrderBy(c => c.Name).ToListAsync();
        return categories;
    }

    public async Task<bool> ExisteNomeAsync(int userId, string name, string subCategory, int ignorarId)
    {
        var nome = (name ?? string.Empty).Trim().ToUpper();
        var sub = (subCategory ?? string.Empty).Trim().ToUpper();
        return await _context.Categories.AnyAsync(c => c.UserId == userId && c.Id != ignorarId
            && c.Name.Trim().ToUpper() == nome && c.SubCategory.Trim().ToUpper() == sub);
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == id && c.UserId == userId))
            return false;

        return await _context.Transactions.AnyAsync(t => t.CategoryId == id)
            || await _context.PessoaMovimentacao.AnyAsync(p => p.CategoriaId == id)
            || await _context.ExtratoBancarioItens.AnyAsync(i => i.CategoriaId == id);
    }
}
