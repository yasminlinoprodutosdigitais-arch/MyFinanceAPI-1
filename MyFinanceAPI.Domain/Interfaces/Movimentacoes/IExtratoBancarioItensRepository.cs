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
        Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndMonthAsync(int userId, DateOnly inicioInclusive, DateOnly fimExclusive, string numeroFatura = null, int? bancoId = null);
        Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndFaturaAsync(int userId, string numeroFatura, int? bancoId = null);
        Task<List<ExtratoBancarioItem>> GetByChaveDescricaoAsync(string chaveDescricao, int userId);

        /// <summary>Chaves de importação já gravadas do usuário para o banco (idempotência do import).</summary>
        Task<HashSet<string>> ObterChavesImportacaoAsync(int userId, int bancoId);

        /// <summary>
        /// Itens importados do usuário no banco cujo valor não tem centavos — candidatos a terem sido
        /// truncados pelo bug BE-P0-07 (importações anteriores a 02/10/2026). Usado para repará-los na reimportação.
        /// </summary>
        Task<List<ExtratoBancarioItem>> ObterCandidatosTruncadosAsync(int userId, int bancoId);
    }
}
