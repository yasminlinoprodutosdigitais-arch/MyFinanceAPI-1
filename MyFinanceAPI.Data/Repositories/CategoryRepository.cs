using System;
using System.Security.Cryptography;
using MongoDB.Bson;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categoryCollection;

    public CategoryRepository(IMongoContext mongoContext)
    {
        _categoryCollection = mongoContext.GetCollection<Category>("Categories");
    }
    public async Task<Category> Create(Category category)
    {
        var existingCategory = await _categoryCollection
            .Find(c => c.Id == category.Id)
            .FirstOrDefaultAsync();

        if(existingCategory is not null)
            await _categoryCollection.InsertOneAsync(category);
        
        return category;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _categoryCollection.Find(c => true).ToListAsync();
    }

    public async Task<Category> GetCategoryById(ObjectId id)
    {
        return await _categoryCollection.Find(c=> c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Category> Remove(Category category)
    {
        await _categoryCollection.DeleteOneAsync(c => c.Id == category.Id);
        return category;        
    }

    public async Task<Category> Update(Category category)
    {
       await _categoryCollection.ReplaceOneAsync(c => c == category, category);
       return category;
    }
}
