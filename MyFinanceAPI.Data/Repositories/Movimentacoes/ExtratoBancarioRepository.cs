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
        public async Task<IReadOnlyList<ImportacaoResumo>> ListarImportacoesAsync(int userId, DateTime? desde) =>
            await _context.ExtratosBancarios.AsNoTracking()
                .Where(e => e.UsuarioId == userId && (desde == null || e.DataImportacao >= desde))
                .OrderByDescending(e => e.DataImportacao)
                .Select(e => new ImportacaoResumo(
                    e.Id, e.BancoId, e.DataImportacao, e.NomeArquivoOrigem, e.MesReferencia, e.QuantidadeDuplicados,
                    e.DesfeitoEm, e.MotivoDesfazer, e.DataInicioPeriodo, e.Itens.Count(),
                    e.Itens.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Entrada).Sum(i => (decimal?)i.Valor) ?? 0,
                    e.Itens.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Saida).Sum(i => (decimal?)i.Valor) ?? 0,
                    e.Itens.Count(i => i.CategoriaId == null && !i.Partes.Any()),
                    e.Itens.Where(i => i.NumeroFatura != null).Select(i => i.NumeroFatura).FirstOrDefault()))
                .ToListAsync();

        public async Task<VinculosImportacao?> ObterVinculosAsync(int id, int userId)
        {
            if (!await _context.ExtratosBancarios.AnyAsync(e => e.Id == id && e.UsuarioId == userId))
                return null;

            var itens = _context.ItensExtratoBancario.AsNoTracking().Where(i => i.ExtratoBancarioId == id);
            var ids = itens.Select(i => i.Id);
            var pessoasDiretas = itens.Where(i => !i.Partes.Any() && i.PessoaMovimentacao != null && i.PessoaMovimentacao.EntraNosAcertos).Select(i => i.PessoaMovimentacao!.NomePessoa);
            var pessoasDasPartes = itens.SelectMany(i => i.Partes).Where(p => p.PessoaMovimentacao != null && p.PessoaMovimentacao.EntraNosAcertos).Select(p => p.PessoaMovimentacao!.NomePessoa);
            return new VinculosImportacao(
                await itens.CountAsync(),
                await itens.CountAsync(i => i.Partes.Any()),
                await itens.CountAsync(i => i.NotaFiscal != null),
                await _context.ItensExtratoBancario.CountAsync(i => i.AbateItemId != null && ids.Contains(i.AbateItemId.Value) && i.ExtratoBancarioId != id),
                (await pessoasDiretas.Union(pessoasDasPartes).ToListAsync()).OrderBy(n => n).ToList());
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
                .Where(e => e.UsuarioId == userId && e.DesfeitoEm == null
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
