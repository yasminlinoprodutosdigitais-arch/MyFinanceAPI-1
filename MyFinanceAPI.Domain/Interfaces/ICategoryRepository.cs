using System;
using MongoDB.Bson;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategoriesByUserId(int userId);
    Task<Category?> GetCategoryById(int id, int userId);
    Task<Category> Create(Category category);
    Task<Category?> FindByIdForUserAsync(int id, int userId);
    Task<bool> UpdateAsync(Category entity);
    Task<Category?> Remove(int id, int userId);
}
