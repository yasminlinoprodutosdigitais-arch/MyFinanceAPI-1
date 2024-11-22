using System;
using System.Net.Http.Headers;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITransactionService
{
    Task Add(TransactionDTO TransactionDTO);
    Task Update(TransactionDTO TransactionDTO);
    Task Remove(TransactionDTO TransactionDTO);
    Task<IEnumerable<TransactionDTO>> GetTransactions();
    Task<TransactionDTO> GetTransactionById(ObjectId id);
    Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId);
}
