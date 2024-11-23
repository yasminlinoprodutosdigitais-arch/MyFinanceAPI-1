using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categoryCollection;

    public CategoryRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _categoryCollection = database.GetCollection<Category>("Categories");
    }
    public async Task<Category> Create(Category category)
    {
        var existingCategory = await _categoryCollection.Find(c => c.Id == 0).FirstOrDefaultAsync();
        if (existingCategory != null)
        {
            var maxCategory = await _categoryCollection.Find(Builders<Category>.Filter.Empty)
            .SortByDescending(c => c.Id)
            .FirstOrDefaultAsync();
            category.Id = maxCategory?.Id + 1 ?? 1;
        }

        // Caso contrário, insira o novo documento
        await _categoryCollection.InsertOneAsync(category);
        return category;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _categoryCollection.Find(c => true).ToListAsync();
    }

    public async Task<Category> GetCategoryById(int id)
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
