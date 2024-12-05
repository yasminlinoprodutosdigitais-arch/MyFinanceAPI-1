using System;
using System.Net.Http.Headers;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface IAccountService
{
    Task Add(AccountDTO accountDTO);
    Task Update(AccountDTO accountDTO);
    Task Remove(int id);
    Task<IEnumerable<AccountDTO>> GetAccounts();
    Task<AccountDTO> GetAccountById(int id);
    Task<IEnumerable<AccountDTO>> GetAccountByCategory(int categoryId);
}
