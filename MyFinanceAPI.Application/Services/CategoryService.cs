using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    public async Task Add(CategoryDTO categoryDTO, int userId)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        category.UserId = userId;
        await _categoryRepository.Create(category);
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategories(int userId)
    {
        var categories = await _categoryRepository.GetCategoriesByUserId(userId);
        return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
    }

    public async Task<CategoryDTO> GetCategoryById(int id, int userId)
    {
        var category = await _categoryRepository.GetCategoryById(id, userId);
        return _mapper.Map<CategoryDTO>(category);
    }

    public async Task Remove(int id, int userId)
    {
        await _categoryRepository.Remove(id, userId);
    }

    public async Task Update(CategoryDTO categoryDTO, int userId)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _categoryRepository.Update(category, userId);
    }
}
