using System;
using MongoDB.Bson;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategoriesByUserId(int userId);
    Task<Category?> GetCategoryById(int id, int userId);
    Task<Category> Create(Category category);
    Task<Category> Update(Category category, int userId);
    Task<Category?> Remove(int id, int userId);
}
