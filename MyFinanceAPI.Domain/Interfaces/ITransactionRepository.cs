using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<List<AccountGrouping>> GetTransactions(int userId);
    Task<List<AccountGrouping>> GetTransactionGroupingByDate(DateTime dateTime, int userId);
    Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime, int userId);
    Task<Transaction> GetTransactionById(int id, int userId);
    Task<Transaction> Create(Transaction transaction);
    Task<Transaction> Update(Transaction transaction, int userId);
    Task<Transaction> Remove(int id, int userId);
}
