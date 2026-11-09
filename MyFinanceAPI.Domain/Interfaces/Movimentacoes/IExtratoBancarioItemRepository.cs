using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces
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
        Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndMonthAsync(int userId, DateOnly inicioInclusive, DateOnly fimExclusive, string? numeroFatura = null, int? bancoId = null);
        Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndFaturaAsync(int userId, string numeroFatura, int? bancoId = null);
        Task<List<ExtratoBancarioItem>> GetByChaveDescricaoAsync(string chaveDescricao, int userId);

        /// <summary>Chaves de importação já gravadas do usuário para o banco (idempotência do import).</summary>
        Task<HashSet<string>> ObterChavesImportacaoAsync(int userId, int bancoId);

        /// <summary>
        /// Itens importados do usuário no banco cujo valor não tem centavos — candidatos a terem sido
        /// truncados pelo bug BE-P0-07 (importações anteriores a 02/10/2026). Usado para repará-los na reimportação.
        /// </summary>
        Task<List<ExtratoBancarioItem>> ObterCandidatosTruncadosAsync(int userId, int bancoId);

        /// <summary>Lançamentos do usuário com pessoa dos acertos ("Entra nos acertos" ou o "Eu"), com pessoa e banco (card #45).</summary>
        Task<List<ExtratoBancarioItem>> ListarParaAcertosAsync(int userId);

        /// <summary>Todos os lançamentos do usuário, sem rastreio: o histórico e os alvos das regras de classificação (card #76).</summary>
        Task<List<ExtratoBancarioItem>> ListarParaClassificacaoAsync(int userId);
        /// <summary>Grava vários lançamentos de uma vez (o "aplicar regra" do card #76).</summary>
        Task UpdateRangeAsync(IEnumerable<ExtratoBancarioItem> itens);
        /// <summary>Quantos lançamentos do usuário cada pessoa tem (id da pessoa → quantidade; card #76, fase 2).</summary>
        Task<Dictionary<int, int>> ContarPorPessoaAsync(int userId);
        /// <summary>Tira estas pessoas dos lançamentos do usuário; o nome dela fica no lançamento (card #76, fase 2).</summary>
        Task DesligarPessoasAsync(int userId, IReadOnlyCollection<int> pessoaIds);
        /// <summary>Troca as partes do lançamento por estas; vazio desfaz a divisão (card #76, fase 3).</summary>
        Task SubstituirPartesAsync(int itemId, IReadOnlyList<ExtratoItemParte> partes);
        /// <summary>As pessoas dos pagamentos que abatem este lançamento.</summary>
        Task<IEnumerable<int?>> PessoasQueAbatemAsync(int itemId);
        /// <summary>As pessoas usadas em partes de lançamentos do usuário.</summary>
        Task<IEnumerable<int>> PessoasEmPartesAsync(int userId);
        /// <summary>Lançamentos do usuário, sem rastreio, com o necessário para o resumo por categoria (card #76, fase 5).</summary>
        Task<List<ExtratoBancarioItem>> ListarParaResumoAsync(int userId);
    }
}
