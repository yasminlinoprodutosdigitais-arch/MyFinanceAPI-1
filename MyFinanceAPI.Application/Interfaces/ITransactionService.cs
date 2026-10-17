using System;
using System.Transactions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITransactionService
{
    // Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId, int userId);
    Task<TransactionDTO> GetTransactionById(int id, int userId);
    Task<IEnumerable<TransactionDTO>> GetTransactionByDate(DateTime date, int userId, bool pesquisaDataCompleta = false);
    Task<IEnumerable<TransactionDTO>> BuscarContasMensais(DateTime date, int userId, bool pesquisaDataCompleta = false);    
    Task<IEnumerable<TransactionDTO>> GetContaVencida(DateTime hoje, int userId);

    /// <summary>Cria uma transação por vencimento da conta e devolve as criadas.</summary>
    Task<IReadOnlyList<TransactionDTO>> Add(TransactionDTO TransactionDTO, int userId);
    Task Update(TransactionDTO TransactionDTO, int userId);
    Task Delete(int id, int userId);
}
