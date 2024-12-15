using System;
using System.Transactions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<AccountGroupingDTO>> GetTransactions();
    Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId);
    Task<TransactionDTO> GetTransactionById(int id);
    Task<IEnumerable<TransactionDTO>> GetTransactionByDate(DateTime date);
    Task<IEnumerable<AccountGroupingDTO>> GetTransactionGroupingByDate(DateTime date);

    Task Add(TransactionDTO TransactionDTO);
    Task Update(TransactionDTO TransactionDTO);
    Task Delete(int id);
}
