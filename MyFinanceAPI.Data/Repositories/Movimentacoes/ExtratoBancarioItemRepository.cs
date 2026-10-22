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

        public async Task<ExtratoBancarioItem> CreateAsync(ExtratoBancarioItem item)
        {
            try
            {
                await _context.ItensExtratoBancario.AddAsync(item);
                await _context.SaveChangesAsync();
                return item;
                
            } catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }
        }

        public async Task CreateRangeAsync(IEnumerable<ExtratoBancarioItem> itens)
        {try
            {
                await _context.ItensExtratoBancario.AddRangeAsync(itens);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (ex.Message, ex.StackTrace, etc.)
                throw; // Rethrow or handle as needed
            }
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

        // =========================
        //          UPDATE
        // =========================

        public async Task UpdateAsync(ExtratoBancarioItem item)
        {
            _context.ItensExtratoBancario.Update(item);
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
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndMonthAsync(
            int userId,
            DateOnly inicioInclusive,
            DateOnly fimExclusive, string numeroFatura = null, int? bancoId = null)
        {
            return await _context.ItensExtratoBancario.AsNoTracking()
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.Categoria)
                .Include(i => i.TipoMovimentacao)
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
