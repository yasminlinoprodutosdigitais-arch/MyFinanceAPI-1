using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class CategoriaService(ICategoriaRepository categoryRepository, IContaRepository accountRepository, IMapper mapper)
    : ServicoCadastro<Categoria, CategoriaDTO>(categoryRepository, mapper), ICategoriaService
{
    public async Task<IEnumerable<CategoriaDTO>> GetCategoriasAtivas(int userId) =>
        Mapper.Map<IEnumerable<CategoriaDTO>>(await categoryRepository.GetCategoriesByUserIdAtiva(userId));

    protected override Categoria ParaEntidade(CategoriaDTO dto)
    {
        var category = base.ParaEntidade(dto);
        category.Nome = dto.Nome?.Trim() ?? string.Empty;
        category.Subcategoria = dto.Subcategoria?.Trim() ?? string.Empty;
        return category;
    }

    // UX_Categories_UserId_Name_SubCategory é exata; aqui a regra também ignora maiúsculas e espaços.
    protected override async Task ValidarGravacaoAsync(CategoriaDTO dto, int userId, int ignorarId)
    {
        if (await categoryRepository.ExisteNomeAsync(userId, dto.Nome ?? string.Empty, dto.Subcategoria ?? string.Empty, ignorarId))
            throw new RegraNegocioException($"Já existe a categoria {dto.Nome?.Trim()} / {dto.Subcategoria?.Trim()}.");
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
