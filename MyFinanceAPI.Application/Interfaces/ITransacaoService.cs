using System;
using System.Transactions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITransacaoService
{
    // Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId, int userId);
    Task<TransacaoDTO> GetTransactionById(int id, int userId);
    Task<IEnumerable<TransacaoDTO>> GetTransactionByDate(DateTime date, int userId, bool pesquisaDataCompleta = false);
    Task<IEnumerable<TransacaoDTO>> BuscarContasMensais(DateTime date, int userId, bool pesquisaDataCompleta = false);    
    Task<IEnumerable<TransacaoDTO>> GetContaVencida(DateTime hoje, int userId);
    /// <summary>Lançamentos da conta, do mais recente para o mais antigo (card #22).</summary>
    Task<IReadOnlyList<TransacaoDTO>> GetTransacoesDaConta(int contaId, int userId);

    /// <summary>Cria uma transação por vencimento da conta e devolve as criadas.</summary>
    Task<IReadOnlyList<TransacaoDTO>> Add(TransacaoDTO TransactionDTO, int userId);
    Task Update(TransacaoDTO TransactionDTO, int userId);
    Task Delete(int id, int userId);
}
