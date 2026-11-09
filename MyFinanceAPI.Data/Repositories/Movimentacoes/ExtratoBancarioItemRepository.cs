using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories
{
    public class ExtratoBancarioItemRepository : IExtratoBancarioItemRepository
    {
        private readonly ContextDB _context;

        public ExtratoBancarioItemRepository(ContextDB context)
        {
            _context = context;
        }

        // Sem try/catch: a falha chega ao ErroHandler (BE-P4-07). Antes este método engolia a exceção e
        // devolvia null, e o controller quebrava em created.Id — o 500 genérico escondia o erro real.
        public async Task<ExtratoBancarioItem> CreateAsync(ExtratoBancarioItem item)
        {
            await _context.ItensExtratoBancario.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task CreateRangeAsync(IEnumerable<ExtratoBancarioItem> itens)
        {
            await _context.ItensExtratoBancario.AddRangeAsync(itens);
            await _context.SaveChangesAsync();
        }

        public async Task<HashSet<string>> ObterChavesImportacaoAsync(int userId, int bancoId)
        {
            var chaves = await _context.ItensExtratoBancario
                .Where(i => i.UsuarioId == userId && i.BancoId == bancoId && i.ChaveImportacao != null)
                .Select(i => i.ChaveImportacao!)
                .ToListAsync();
            return chaves.ToHashSet();
        }

        public async Task<List<ExtratoBancarioItem>> ObterCandidatosTruncadosAsync(int userId, int bancoId)
        {
            return await _context.ItensExtratoBancario
                .Where(i => i.UsuarioId == userId && i.BancoId == bancoId
                            && i.ExtratoBancarioId != null && i.Identificador != null
                            && i.Valor == Math.Truncate(i.Valor))
                .ToListAsync();
        }

        public async Task<List<ExtratoBancarioItem>> ListarParaAcertosAsync(int userId)
        {
            return await _context.ItensExtratoBancario.AsNoTracking()
                .Include(i => i.PessoaMovimentacao)
                .Include(i => i.Banco)
                .Include(i => i.Partes).ThenInclude(p => p.PessoaMovimentacao)
                .Where(i => i.UsuarioId == userId
                            && ((!i.Partes.Any() && i.PessoaMovimentacao != null
                                    && (i.PessoaMovimentacao.EntraNosAcertos || i.PessoaMovimentacao.EhTitular))
                                || i.Partes.Any(p => p.PessoaMovimentacao != null
                                    && (p.PessoaMovimentacao.EntraNosAcertos || p.PessoaMovimentacao.EhTitular))))
                .OrderBy(i => i.DataMovimentacao)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }

        // =========================
        //          UPDATE
        // =========================

        public async Task UpdateAsync(ExtratoBancarioItem item)
        {
            _context.ItensExtratoBancario.Update(item);
            await _context.SaveChangesAsync();
        }

        public Task<List<ExtratoBancarioItem>> ListarParaClassificacaoAsync(int userId) =>
            // Com a categoria: a prévia mostra o nome nos exemplos. O "aplicar" só grava os sem categoria (nav vazia).
            _context.ItensExtratoBancario.AsNoTracking().Include(i => i.Categoria).Include(i => i.Partes).Where(i => i.UsuarioId == userId).ToListAsync();

        public async Task UpdateRangeAsync(IEnumerable<ExtratoBancarioItem> itens)
        {
            _context.ItensExtratoBancario.UpdateRange(itens);
            await _context.SaveChangesAsync();
        }

        public async Task SubstituirPartesAsync(int itemId, IReadOnlyList<ExtratoItemParte> partes)
        {
            _context.ExtratoItemPartes.RemoveRange(_context.ExtratoItemPartes.Where(p => p.ExtratoBancarioItemId == itemId));
            await _context.ExtratoItemPartes.AddRangeAsync(partes);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TemCategoriaForaDoUsuarioAsync(IReadOnlyCollection<int> categoriaIds, int userId)
        {
            var ids = categoriaIds.Distinct().ToList();
            return ids.Count > 0 && await _context.Set<Categoria>().CountAsync(c => ids.Contains(c.Id) && c.UsuarioId == userId) != ids.Count;
        }

        public async Task<IEnumerable<int?>> PessoasQueAbatemAsync(int itemId) =>
            await _context.ItensExtratoBancario.Where(i => i.AbateItemId == itemId).Select(i => i.PessoaMovimentacaoId).ToListAsync();

        public Task<List<ExtratoBancarioItem>> ListarParaResumoAsync(int userId) =>
            _context.ItensExtratoBancario.AsNoTracking()
                .Include(i => i.TipoCartao).Include(i => i.Categoria).Include(i => i.PessoaMovimentacao)
                .Include(i => i.Partes).ThenInclude(p => p.Categoria)
                .Include(i => i.Partes).ThenInclude(p => p.PessoaMovimentacao)
                .Where(i => i.UsuarioId == userId)
                .ToListAsync();

        public async Task<IEnumerable<int>> PessoasEmPartesAsync(int userId) =>
            await _context.ExtratoItemPartes.Where(p => p.UsuarioId == userId && p.PessoaMovimentacaoId != null)
                .Select(p => p.PessoaMovimentacaoId!.Value).Distinct().ToListAsync();

        public Task<Dictionary<int, int>> ContarPorPessoaAsync(int userId) =>
            _context.ItensExtratoBancario
                .Where(i => i.UsuarioId == userId && i.PessoaMovimentacaoId != null)
                .GroupBy(i => i.PessoaMovimentacaoId!.Value)
                .Select(g => new { g.Key, Quantidade = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Quantidade);

        public async Task DesligarPessoasAsync(int userId, IReadOnlyCollection<int> pessoaIds)
        {
            var ids = pessoaIds.ToList();
            var ligados = await _context.ItensExtratoBancario
                .Include(i => i.PessoaMovimentacao)
                .Where(i => i.UsuarioId == userId && i.PessoaMovimentacaoId != null && ids.Contains(i.PessoaMovimentacaoId.Value))
                .ToListAsync();
            foreach (var item in ligados)
            {
                // O nome continua no lançamento (o estabelecimento); só o vínculo com o cadastro sai.
                if (string.IsNullOrWhiteSpace(item.NomePessoaTransacao))
                    item.NomePessoaTransacao = item.PessoaMovimentacao?.NomePessoa;
                item.PessoaMovimentacaoId = null;
                item.PessoaMovimentacao = null;
            }
            await _context.SaveChangesAsync();
        }

        // =========================
        //          DELETE
        // =========================

        public async Task RemoveAsync(int id)
        {
            var item = await _context.ItensExtratoBancario
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return;

            _context.ItensExtratoBancario.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveByExtratoIdAsync(int extratoId)
        {
            var itens = await _context.ItensExtratoBancario
                .Where(i => i.ExtratoBancarioId == extratoId)
                .ToListAsync();

            if (!itens.Any())
                return;

            _context.ItensExtratoBancario.RemoveRange(itens);
            await _context.SaveChangesAsync();
        }

        // =========================
        //           GET
        // =========================

        public async Task<ExtratoBancarioItem?> GetByIdAsync(int id)
        {
            return await _context.ItensExtratoBancario
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.TipoMovimentacao)
                .Include(i => i.Partes)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndMonthAsync(
            int userId,
            DateOnly inicioInclusive,
            DateOnly fimExclusive, string? numeroFatura = null, int? bancoId = null)
        {
            return await _context.ItensExtratoBancario.AsNoTracking()
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.Categoria)
                .Include(i => i.TipoMovimentacao)
                .Include(i => i.Partes).ThenInclude(p => p.Categoria)
                .Include(i => i.Partes).ThenInclude(p => p.PessoaMovimentacao)
                .Include(i => i.NotaFiscal)
                .Where(ExtratoBancarioItemFiltros.DoMesOuDaFatura(userId, inicioInclusive, fimExclusive, numeroFatura, bancoId))
                .OrderBy(i => i.DataMovimentacao)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndFaturaAsync(
            int userId,
            string numeroFatura, int? bancoId = null)
        {
            return await _context.ItensExtratoBancario
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.Categoria)
                .Include(i => i.TipoMovimentacao)
                .Where(i =>
                    i.UsuarioId == userId &&
                    i.NumeroFatura == numeroFatura &&
                    (bancoId == null || i.BancoId == bancoId))
                .OrderBy(i => i.DataMovimentacao)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<List<ExtratoBancarioItem>> GetByChaveDescricaoAsync (string ChaveDescricao, int userId)
        {
            var vinculo = await _context.ItensExtratoBancario
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Where(v => v.ChaveDescricao == ChaveDescricao && v.UsuarioId == userId)
                .ToListAsync();

            if (vinculo == null)
                return new List<ExtratoBancarioItem>();

            return vinculo;
        }
    }
}
