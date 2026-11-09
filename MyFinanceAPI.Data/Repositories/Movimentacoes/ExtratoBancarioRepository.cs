using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories
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
                item.UsuarioId = extrato.UsuarioId;

            await _context.ExtratosBancarios.AddAsync(extrato);
            await _context.SaveChangesAsync();
            return extrato;
        }

        public async Task UpdateAsync(ExtratoBancario extrato)
        {
            _context.ExtratosBancarios.Update(extrato);
            await _context.SaveChangesAsync();
        }

        // =========================
        //          DELETE
        // =========================

        // Versão com segurança por usuário
        public async Task DeleteAsync(int id, int userId)
        {
            var extrato = await _context.ExtratosBancarios
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);

            if (extrato == null)
                return;

            _context.ExtratosBancarios.Remove(extrato);
            await _context.SaveChangesAsync();
        }

        // =========================
        //            GET
        // =========================

        public async Task<ExtratoBancario?> GetByIdAsync(int id, int userId)
        {
            return await _context.ExtratosBancarios
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);
        }

        // Service chama GetByUserIdAsync
        public async Task<IEnumerable<ExtratoBancario>> GetByUserIdAsync(int userId, string month)
        {
            // month = "2026-02"
            var parts = month.Split('-');
            var year = int.Parse(parts[0]);
            var mon = int.Parse(parts[1]);

            var start = new DateOnly(year, mon, 1);
            var end = start.AddMonths(1);
            var mesFatura = start.ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);

            var extratos = await _context.ExtratosBancarios.AsNoTracking()
                // Fatura aparece no mês dela (card #76, fase 4): a primeira compra pode ser de meses antes.
                .Where(e => e.UsuarioId == userId
                    && (e.Itens.Any(i => i.NumeroFatura == mesFatura)
                        || (!e.Itens.Any(i => i.NumeroFatura != null)
                            && e.DataInicioPeriodo.HasValue
                            && e.DataInicioPeriodo.Value >= start
                            && e.DataInicioPeriodo.Value < end)))
                .Include(e => e.Itens)
                .OrderByDescending(e => e.DataImportacao)
                .ToListAsync();

            return extratos;
        }


        public async Task<IEnumerable<ExtratoBancario>> GetByPeriodoAsync(
            int userId,
            DateOnly? dataInicio,
            DateOnly? dataFim)
        {
            var query = _context.ExtratosBancarios
                .Where(e => e.UsuarioId == userId)
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
