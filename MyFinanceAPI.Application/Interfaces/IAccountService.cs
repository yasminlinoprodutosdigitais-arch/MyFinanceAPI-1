using System;
using System.Net.Http.Headers;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface IAccountService
{
    Task Add(AccountDTO accountDTO, int userId);
    Task Update(AccountDTO accountDTO, int userId);
    Task Remove(int id, int userId);
    Task<IEnumerable<AccountDTO>> GetAccounts(int userId);
    Task<AccountDTO> GetAccountById(int id, int userId);
    Task<IEnumerable<AccountDTO>> GetAccountByCategory(int categoryId, int userId);
}
