using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>>? GetAccounts();
    Task<Account> GetAccountById(int id);
    Task<Account> GetAccountByCategory(int categoryid);
    Task<Account> Create(Account account);
    Task<Account> Update(Account account);
    Task<Account> Remove(int id);
}
