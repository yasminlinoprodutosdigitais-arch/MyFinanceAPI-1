using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IContaRepository
{
    Task<List<Conta>> GetAccounts(int userId);
    Task<List<Conta>> GetContasAtivas(int userId);
    Task<Conta?> GetAccountById(int id, int userId);
    Task<Conta> Create(Conta account);
    Task CreateContaVencimento (List<ContaVencimento> contaVencimento);
    Task<Conta> Update(Conta account, int userId);
    Task Remove(int id, int userId);
    /// <summary>True se a conta é do usuário e tem transações (FK RESTRICT bloqueia o DELETE).</summary>
    Task<bool> EmUsoAsync(int id, int userId);
    Task RemoveContaVencimento(int id, int userId);
    Task<IEnumerable<Conta>> GetAccountsByCategory(int categoryId, int userId);
}
