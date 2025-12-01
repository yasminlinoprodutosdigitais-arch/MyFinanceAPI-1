using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces.Repositories
{
    public interface IExtratoBancarioItemRepository
    {
        // CREATE
        Task<ExtratoBancarioItem> CreateAsync(ExtratoBancarioItem item);
        Task CreateRangeAsync(IEnumerable<ExtratoBancarioItem> itens); // usado pelo service

        // UPDATE
        Task UpdateAsync(ExtratoBancarioItem item);

        // DELETE
        Task RemoveAsync(int id);                     // remove item isolado
        Task RemoveByExtratoIdAsync(int extratoId);   // remove todos os itens de um extrato

        // GET
        Task<ExtratoBancarioItem?> GetByIdAsync(int id);
        Task<IEnumerable<ExtratoBancarioItem>> GetByExtratoAsync(int extratoBancarioId);
        Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndMonthAsync(int userId, DateOnly inicioInclusive, DateOnly fimExclusive);

    }
}
