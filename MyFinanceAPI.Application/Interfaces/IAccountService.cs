using System;
using System.Net.Http.Headers;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface IAccountService
{
    Task<AccountDTO> Add(AccountDTO accountDTO, int userId);
    Task Update(AccountDTO accountDTO, int userId);
    Task UpdateParcela(AccountDTO accountDTO, int userId);
    Task Remove(int id, int userId);
    Task<IEnumerable<AccountDTO>> GetAccounts(int userId);
    Task<IEnumerable<AccountDTO>> GetContasAtivas(int userId);
    Task<AccountDTO> GetAccountById(int id, int userId);
}
