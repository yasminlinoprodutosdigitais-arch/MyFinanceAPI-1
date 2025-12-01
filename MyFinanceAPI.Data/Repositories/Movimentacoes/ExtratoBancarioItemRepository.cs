using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Infra.Data.Repositories
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
            await _context.ExtratoBancarioItens.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task CreateRangeAsync(IEnumerable<ExtratoBancarioItem> itens)
        {
            await _context.ExtratoBancarioItens.AddRangeAsync(itens);
            await _context.SaveChangesAsync();
        }

        // =========================
        //          UPDATE
        // =========================

        public async Task UpdateAsync(ExtratoBancarioItem item)
        {
            _context.ExtratoBancarioItens.Update(item);
            await _context.SaveChangesAsync();
        }

        // =========================
        //          DELETE
        // =========================

        public async Task RemoveAsync(int id)
        {
            var item = await _context.ExtratoBancarioItens
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return;

            _context.ExtratoBancarioItens.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveByExtratoIdAsync(int extratoId)
        {
            var itens = await _context.ExtratoBancarioItens
                .Where(i => i.ExtratoBancarioId == extratoId)
                .ToListAsync();

            if (!itens.Any())
                return;

            _context.ExtratoBancarioItens.RemoveRange(itens);
            await _context.SaveChangesAsync();
        }

        // =========================
        //           GET
        // =========================

        public async Task<ExtratoBancarioItem?> GetByIdAsync(int id)
        {
            return await _context.ExtratoBancarioItens
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.TipoMovimentacao)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ExtratoBancarioItem>> GetByExtratoAsync(int extratoBancarioId)
        {
            return await _context.ExtratoBancarioItens
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.TipoMovimentacao)
                .Where(i => i.ExtratoBancarioId == extratoBancarioId)
                .OrderBy(i => i.DataMovimentacao)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExtratoBancarioItem>> GetByUserAndMonthAsync(
            int userId,
            DateOnly inicioInclusive,
            DateOnly fimExclusive)
        {
            return await _context.ExtratoBancarioItens
                .Include(i => i.Banco)
                .Include(i => i.TipoCartao)
                .Include(i => i.TipoMovimentacao)
                .Where(i =>
                    i.UserId == userId &&
                    i.DataMovimentacao >= inicioInclusive &&
                    i.DataMovimentacao < fimExclusive)
                .OrderBy(i => i.DataMovimentacao)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }
    }
}
