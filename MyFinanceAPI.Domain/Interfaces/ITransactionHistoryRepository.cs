using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITransactionHistoryRepository
{
    Task<IEnumerable<TransactionHistory>> GetTransactionHistory();
    Task<IEnumerable<TransactionHistory>> GetTransactionByDate(DateTime dateTime);
    Task<TransactionHistory> GetTransactionHistoryById(int id);
    Task<IEnumerable<TransactionHistory>> GetTransactionHistoryByCategory(int categoryid);
    Task<TransactionHistory> Create(TransactionHistory transactionHistory);
    Task<TransactionHistory> Update(TransactionHistory transactionHistory);
    Task<TransactionHistory> Remove(TransactionHistory transactionHistory);
}
