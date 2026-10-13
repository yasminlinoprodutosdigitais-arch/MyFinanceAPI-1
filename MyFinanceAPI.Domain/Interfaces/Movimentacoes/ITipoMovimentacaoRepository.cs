using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITipoMovimentacaoRepository : IRepositorioDoUsuario<TipoMovimentacao>
{
    /// <summary>True se o tipo de movimentação é do usuário e está em uso por lançamentos ou pessoas.</summary>
    Task<bool> EmUsoAsync(int id, int userId);
}
