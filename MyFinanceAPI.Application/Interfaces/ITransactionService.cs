using System;
using System.Transactions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<AccountGroupingDTO>> GetTransactions(int userId);
    // Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId, int userId);
    Task<TransactionDTO> GetTransactionById(int id, int userId);
    Task<IEnumerable<TransactionDTO>> GetTransactionByDate(DateTime date, int userId);
    Task<IEnumerable<AccountGroupingDTO>> GetTransactionGroupingByDate(DateTime date, int userId);

    Task Add(TransactionDTO TransactionDTO, int userId);
    Task Update(TransactionDTO TransactionDTO, int userId);
    Task Delete(int id, int userId);
}
