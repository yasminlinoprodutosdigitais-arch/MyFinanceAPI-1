using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ICategoryService
{
    Task Add(CategoryDTO categoryDTO, int userId);
    Task<IEnumerable<CategoryDTO>> GetCategories(int userId);
    Task<CategoryDTO> GetCategoryById(int id, int userId);
    Task<bool> UpdateAsync(CategoryDTO dto, int userId);
    Task Remove(int id, int userId);
}
