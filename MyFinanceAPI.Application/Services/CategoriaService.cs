using MyFinanceAPI.Application.Utils;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class CategoriaService(ICategoriaRepository categoryRepository, IContaRepository accountRepository, IMapper mapper)
    : ServicoCadastro<Categoria, CategoriaDTO>(categoryRepository, mapper), ICategoriaService
{
    /// <summary>NaturezaOperacao 0 = "Rendimento / Entrada": a única da Receita, e nunca de Despesa.</summary>
    private const int RendimentoEntrada = 0;

    /// <summary>Chaves da paleta do front (shared/ui/aparencia.ts); a cor em si fica nos tokens do front.</summary>
    private static readonly HashSet<string> Paleta =
        ["azul", "roxo", "verde-agua", "verde", "laranja", "indigo", "rosa", "vermelho", "amarelo", "cinza"];

    public async Task<IEnumerable<CategoriaDTO>> GetCategoriasAtivas(int userId) =>
        Mapper.Map<IEnumerable<CategoriaDTO>>(await categoryRepository.GetCategoriesByUserIdAtiva(userId));

    public override async Task<IEnumerable<CategoriaDTO>> ListarAsync(int userId)
    {
        var categorias = (await base.ListarAsync(userId)).ToList();
        var contas = await categoryRepository.ContarContasPorCategoriaAsync(userId);
        foreach (var categoria in categorias)
            categoria.QuantidadeContas = contas.GetValueOrDefault(categoria.Id);
        return categorias;
    }

    protected override Categoria ParaEntidade(CategoriaDTO dto)
    {
        var category = base.ParaEntidade(dto);
        category.Nome = dto.Nome?.Trim() ?? string.Empty;
        category.Subcategoria = dto.Subcategoria?.Trim() ?? string.Empty;
        category.Icone = string.IsNullOrWhiteSpace(dto.Icone) ? null : dto.Icone.Trim();
        category.Cor = string.IsNullOrWhiteSpace(dto.Cor) ? null : dto.Cor.Trim();
        return category;
    }

    // UX_Categories_UserId_Name_SubCategory é exata; aqui a regra também ignora maiúsculas e espaços.
    protected override async Task ValidarGravacaoAsync(CategoriaDTO dto, int userId, int ignorarId)
    {
        var receita = string.Equals(dto.Subcategoria?.Trim(), "Receita", StringComparison.OrdinalIgnoreCase);
        if (receita != (dto.NaturezaOperacao == RendimentoEntrada))
            throw new RegraNegocioException(receita
                ? "Receita só pode ter a natureza Rendimento / Entrada."
                : "Despesa não pode ter a natureza Rendimento / Entrada.");

        if (!string.IsNullOrWhiteSpace(dto.Cor) && !Paleta.Contains(dto.Cor.Trim()))
            throw new RegraNegocioException($"Cor inválida: {dto.Cor}.");

        if (!IconePrimeIcons.Valido(dto.Icone))
            throw new RegraNegocioException($"Ícone inválido: {dto.Icone}.");

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
