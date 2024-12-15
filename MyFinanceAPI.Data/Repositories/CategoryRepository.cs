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

    public async Task<IEnumerable<Category>> GetCategories()
    {
        // var categories = await _context.Categories.Where(c => c.Status == true).OrderBy(c => c.Name).ToListAsync();
        var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        return categories;
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> Remove(int id)
    {
        var category = await GetCategoryById(id);

        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        return category;
    }

    public async Task<Category> Update(Category category)
    {
        var existingCategory = await GetCategoryById(category.Id) ?? throw new Exception("Category not found");

        if (existingCategory != null)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        return category;
    }
}
