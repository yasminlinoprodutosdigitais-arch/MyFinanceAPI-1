using System;
using System.Transactions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITransactionHistoryService
{
    Task<IEnumerable<TransactionHistoryDTO>> GetTransactionHistory();
    Task<IEnumerable<TransactionHistoryDTO>> GetTransactionHistoryByCategory(int categoryId);
    Task<TransactionHistoryDTO> GetTransactionHistoryById(int id);
    Task<IEnumerable<TransactionHistoryDTO>> GetTransactionHistoryByDate(DateTime date);

    Task Add(TransactionHistoryDTO transactionHistoryDTO);
    Task Update(TransactionHistoryDTO transactionHistoryDTO);
    Task Delete(TransactionHistoryDTO transactionHistoryDTO);
}
