using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Infra.Data.Repositories
{
    public class ExtratoBancarioRepository : IExtratoBancarioRepository
    {
        private readonly ContextDB _context;

        public ExtratoBancarioRepository(ContextDB context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<ExtratoBancario> CreateAsync(ExtratoBancario extrato)
        {
            if (extrato == null)
                throw new ArgumentNullException(nameof(extrato));

            extrato.Itens ??= new List<ExtratoBancarioItem>();
            foreach (var item in extrato.Itens)
                item.UserId = extrato.UserId;

            await _context.ExtratoBancario.AddAsync(extrato);
            await _context.SaveChangesAsync();
            return extrato;
        }

        public async Task UpdateAsync(ExtratoBancario extrato)
        {
            _context.ExtratoBancario.Update(extrato);
            await _context.SaveChangesAsync();
        }

        // =========================
        //          DELETE
        // =========================

        // Versão usada pelo service (sem userId)
        public async Task RemoveAsync(int id)
        {
            var extrato = await _context.ExtratoBancario
                .FirstOrDefaultAsync(e => e.Id == id);

            if (extrato == null)
                return;

            _context.ExtratoBancario.Remove(extrato);
            await _context.SaveChangesAsync();
        }

        // Versão com segurança por usuário
        public async Task DeleteAsync(int id, int userId)
        {
            var extrato = await _context.ExtratoBancario
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (extrato == null)
                return;

            _context.ExtratoBancario.Remove(extrato);
            await _context.SaveChangesAsync();
        }

        // =========================
        //            GET
        // =========================

        // Usado pelo service (sem userId)
        public async Task<ExtratoBancario?> GetByIdAsync(int id)
        {
            return await _context.ExtratoBancario
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Versão com userId (se quiser usar em outros fluxos)
        public async Task<ExtratoBancario?> GetByIdAsync(int id, int userId)
        {
            return await _context.ExtratoBancario
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        // Service chama GetByUserIdAsync
        public async Task<IEnumerable<ExtratoBancario>> GetByUserIdAsync(int userId)
        {
            return await _context.ExtratoBancario
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.DataImportacao)
                .ToListAsync();
        }

        // Alias / reaproveitamento
        public async Task<IEnumerable<ExtratoBancario>> GetByUserAsync(int userId)
        {
            return await GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<ExtratoBancario>> GetByPeriodoAsync(
            int userId,
            DateOnly? dataInicio,
            DateOnly? dataFim)
        {
            var query = _context.ExtratoBancario
                .Where(e => e.UserId == userId)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(e =>
                    e.DataInicioPeriodo == null ||
                    e.DataInicioPeriodo >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(e =>
                    e.DataFimPeriodo == null ||
                    e.DataFimPeriodo <= dataFim.Value);

            return await query
                .OrderByDescending(e => e.DataImportacao)
                .ToListAsync();
        }
    }
}
