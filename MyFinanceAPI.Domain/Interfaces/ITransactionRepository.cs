using System;
using MongoDB.Bson;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetTransactions();
    Task<Transaction> GetTransactionById(int id);
    Task<IEnumerable<Transaction>> GetTransactionByCategory(int categoryid);
    Task<Transaction> Create(Transaction transaction);
    Task<Transaction> Update(Transaction transaction);
    Task<Transaction> Remove(Transaction transaction);
}
