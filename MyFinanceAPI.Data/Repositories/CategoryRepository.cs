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

    public async Task<Category> Update(Category category, int userId)
    {
        var existingCategory = await _context.Categories
        .Where(c => c.UserId == userId && c.Id == category.Id).FirstOrDefaultAsync();
        
        if(existingCategory == null)
            throw new KeyNotFoundException("Categoria não encontrada");

        existingCategory.Name = category.Name;
        existingCategory.UserId = userId;

        _context.Categories.Update(existingCategory);
        await _context.SaveChangesAsync();

        return existingCategory;
    }

}
