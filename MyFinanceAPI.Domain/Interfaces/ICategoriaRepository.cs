using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ICategoriaRepository : IRepositorioDoUsuario<Categoria>
{
    Task<IEnumerable<Categoria>> GetCategoriesByUserIdAtiva(int userId);
    /// <summary>True se a categoria é do usuário e está em uso por transações, pessoas ou itens de extrato.</summary>
    Task<bool> EmUsoAsync(int id, int userId);
    /// <summary>True se o usuário já tem outra categoria com o mesmo nome e subcategoria (ignora maiúsculas e espaços).</summary>
    Task<bool> ExisteNomeAsync(int userId, string name, string subCategory, int ignorarId);
    /// <summary>Contas do usuário por categoria (categoriaId → quantidade); categoria sem conta não aparece.</summary>
    Task<Dictionary<int, int>> ContarContasPorCategoriaAsync(int userId);
}
