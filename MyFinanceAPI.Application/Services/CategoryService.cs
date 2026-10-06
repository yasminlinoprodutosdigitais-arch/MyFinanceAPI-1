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
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    public async Task Add(CategoryDTO categoryDTO, int userId)
    {
        await GarantirNomeUnico(userId, categoryDTO.Name, categoryDTO.SubCategory, ignorarId: 0);

        var category = _mapper.Map<Category>(categoryDTO);
        category.UserId = userId;
        await _categoryRepository.Create(category);
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategories(int userId)
    {
        var categories = await _categoryRepository.GetCategoriesByUserId(userId);
        return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategoriasAtivas(int userId)
    {
        var categories = await _categoryRepository.GetCategoriesByUserIdAtiva(userId);
        return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
    }

    public async Task<CategoryDTO> GetCategoryById(int id, int userId)
    {
        var category = await _categoryRepository.GetCategoryById(id, userId);
        return _mapper.Map<CategoryDTO>(category);
    }

    public async Task Remove(int id, int userId)
    {
        var contas = await _accountRepository.GetAccountsByCategory(id, userId);
        if (contas.Any())
            throw new InvalidOperationException("Não é possível excluir essa categoria, pois ela possui contas cadastradas.");

        if (await _categoryRepository.EmUsoAsync(id, userId))
            throw new InvalidOperationException("Não é possível excluir essa categoria, pois ela está em uso por transações, pessoas ou itens de extrato.");

        await _categoryRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(CategoryDTO dto, int userId)
    {
        var cat = await _categoryRepository.FindByIdForUserAsync(dto.Id, userId);
        if (cat == null) return false;

        var newName = dto.Name?.Trim() ?? string.Empty;
        var newSub  = dto.SubCategory?.Trim() ?? string.Empty;
        await GarantirNomeUnico(userId, newName, newSub, ignorarId: cat.Id);
        var newNatureza  = dto.NaturezaOperacao;
        var newStatus  = dto.Status;

        var hasChanges =
            !string.Equals(cat.Name, newName, StringComparison.Ordinal) ||
            !string.Equals(cat.SubCategory, newSub, StringComparison.Ordinal) ||
            !int.Equals(cat.NaturezaOperacao, newNatureza) ||
            !int.Equals(cat.Status, newStatus);

        cat.Name        = newName;
        cat.SubCategory = newSub;
        cat.NaturezaOperacao = newNatureza;
        cat.Status = newStatus;
        cat.DataAlteracao = DateTime.UtcNow;
        
        var saved = await _categoryRepository.UpdateAsync(cat);

        return saved;
    }

    // UX_Categories_UserId_Name_SubCategory é exata; aqui a regra também ignora maiúsculas e espaços.
    private async Task GarantirNomeUnico(int userId, string? name, string? subCategory, int ignorarId)
    {
        if (await _categoryRepository.ExisteNomeAsync(userId, name ?? string.Empty, subCategory ?? string.Empty, ignorarId))
            throw new InvalidOperationException($"Já existe a categoria {name?.Trim()} / {subCategory?.Trim()}.");
    }
}
