using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITransacaoRepository
{
    Task<List<Transacao>> BuscarContasMensais(DateTime dateTime, int userId, bool pesquisaDataCompleta = false);
    Task<IEnumerable<Transacao>> GetTransactionByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false);
    Task<IEnumerable<Transacao>> GetContaVencida(DateTime hoje, int userId);
    Task<Transacao> GetTransactionById(int id, int userId);
    Task<List<int>> GetParcelasLancadas(int accountId, int userId);
    Task Create(List<Transacao> transaction);
    Task<Transacao> Update(Transacao transaction, int userId);
    Task<Transacao> Remove(int id, int userId);
}
