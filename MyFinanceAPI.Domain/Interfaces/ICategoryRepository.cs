using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ICategoryRepository : IRepositorioDoUsuario<Category>
{
    Task<IEnumerable<Category>> GetCategoriesByUserIdAtiva(int userId);
    /// <summary>True se a categoria é do usuário e está em uso por transações, pessoas ou itens de extrato.</summary>
    Task<bool> EmUsoAsync(int id, int userId);
    /// <summary>True se o usuário já tem outra categoria com o mesmo nome e subcategoria (ignora maiúsculas e espaços).</summary>
    Task<bool> ExisteNomeAsync(int userId, string name, string subCategory, int ignorarId);
}
