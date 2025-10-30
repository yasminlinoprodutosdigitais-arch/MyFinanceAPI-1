using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class CategoryRepository(ContextDB context) : ICategoryRepository
{
    private readonly ContextDB _context = context;

    public async Task<Category> Create(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<IEnumerable<Category>> GetCategoriesByUserId(int userId)
    {
        var categories = await _context.Categories.Where(c => c.UserId == userId).OrderBy(c => c.Name).ToListAsync();
        return categories;
    }

    public async Task<Category?> GetCategoryById(int id, int userId)
    {
        var category = await _context.Categories
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();

        if (category == null)
            return null;

        return category;
    }

    public async Task<Category?> Remove(int id, int userId)
    {
        var category = await GetCategoryById(id, userId);
        if (category == null)
            return null;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<Category?> FindByIdForUserAsync(int id, int userId)
    {
        // tracking ligado (vamos editar a entidade)
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(Category entity)
    {
        // Se a entidade veio do contexto (tracked), nem precisa do Update().
        // Mas não faz mal; o EF vai marcar modificado.
        _context.Categories.Update(entity);

        var changes = await _context.SaveChangesAsync();
        return changes > 0;
    }

}
