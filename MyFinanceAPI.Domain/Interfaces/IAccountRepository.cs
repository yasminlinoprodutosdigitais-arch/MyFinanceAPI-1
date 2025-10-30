using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>>? GetAccounts(int userId);
    Task<Account> GetAccountById(int id, int userId);
    Task<List<Account>>? GetAccountByCategory(int categoryid, int userId);
    Task<Account> Create(Account account);
    Task<Account> Update(Account account, int userId);
    Task Remove(int id, int userId);
    Task<IEnumerable<Account>> GetAccountsByCategory(int categoryId, int userId);
}
