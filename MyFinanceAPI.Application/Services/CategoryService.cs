using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class CategoryService(ICategoryRepository categoryRepository, IAccountRepository accountRepository, IMapper mapper)
    : ServicoCadastro<Category, CategoryDTO>(categoryRepository, mapper), ICategoryService
{
    public async Task<IEnumerable<CategoryDTO>> GetCategoriasAtivas(int userId) =>
        Mapper.Map<IEnumerable<CategoryDTO>>(await categoryRepository.GetCategoriesByUserIdAtiva(userId));

    protected override Category ParaEntidade(CategoryDTO dto)
    {
        var category = base.ParaEntidade(dto);
        category.Name = dto.Name?.Trim() ?? string.Empty;
        category.SubCategory = dto.SubCategory?.Trim() ?? string.Empty;
        return category;
    }

    // UX_Categories_UserId_Name_SubCategory é exata; aqui a regra também ignora maiúsculas e espaços.
    protected override async Task ValidarGravacaoAsync(CategoryDTO dto, int userId, int ignorarId)
    {
        if (await categoryRepository.ExisteNomeAsync(userId, dto.Name ?? string.Empty, dto.SubCategory ?? string.Empty, ignorarId))
            throw new RegraNegocioException($"Já existe a categoria {dto.Name?.Trim()} / {dto.SubCategory?.Trim()}.");
    }

    protected override async Task ValidarExclusaoAsync(int id, int userId)
    {
        var contas = await accountRepository.GetAccountsByCategory(id, userId);
        if (contas.Any())
            throw new RegraNegocioException("Não é possível excluir essa categoria, pois ela possui contas cadastradas.");

        if (await categoryRepository.EmUsoAsync(id, userId))
            throw new RegraNegocioException("Não é possível excluir essa categoria, pois ela está em uso por transações, pessoas ou itens de extrato.");
    }
}
