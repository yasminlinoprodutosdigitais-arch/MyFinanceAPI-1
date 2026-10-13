using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IBancoRepository : IRepositorioDoUsuario<Banco>
{
    Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId);
    /// <summary>True se o banco é do usuário e tem lançamentos ou extratos (FKs RESTRICT bloqueiam o DELETE).</summary>
    Task<bool> EmUsoAsync(int id, int userId);
    /// <summary>True se o usuário já tem outro banco com o mesmo nome (ignora maiúsculas e espaços).</summary>
    Task<bool> ExisteNomeAsync(int userId, string nome, int ignorarId);
}
