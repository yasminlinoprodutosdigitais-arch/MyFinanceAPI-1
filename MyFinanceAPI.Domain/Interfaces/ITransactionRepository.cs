using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<List<AccountGrouping>> GetTransactions();
    Task<List<AccountGrouping>> GetTransactionGroupingByDate(DateTime dateTime);
    Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime);
    Task<Transaction> GetTransactionById(int id);
    Task<IEnumerable<Transaction>> GetTransactionByCategory(int categoryid);
    Task<Transaction> Create(Transaction transaction);
    Task<Transaction> Update(Transaction transaction);
    Task<Transaction> Remove(int id);
}
