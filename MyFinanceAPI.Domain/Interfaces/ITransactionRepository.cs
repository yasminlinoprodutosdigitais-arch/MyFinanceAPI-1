using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<List<AccountGrouping>> GetTransactions(int userId);
    Task<List<Transaction>> GetTransactionGroupingByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false);
    Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false);
    Task<IEnumerable<Transaction>> GetContaVencida(DateTime hoje, int userId);
    Task<Transaction> GetTransactionById(int id, int userId);
    Task Create(List<Transaction> transaction);
    Task<Transaction> Update(Transaction transaction, int userId);
    Task<Transaction> Remove(int id, int userId);
}
