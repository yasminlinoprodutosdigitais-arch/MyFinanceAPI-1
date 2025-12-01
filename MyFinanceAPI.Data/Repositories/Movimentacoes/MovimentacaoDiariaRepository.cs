using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class MovimentacaoDiariaRepository(ContextDB context) : IMovimentacaoDiariaRepository
{
    private readonly ContextDB _context = context;

    public async Task<MovimentacaoDiaria> Create(MovimentacaoDiaria MovimentacaoDiaria)
    {
        await _context.MovimentacaoDiaria.AddAsync(MovimentacaoDiaria);
        var ret = await _context.SaveChangesAsync();
        return MovimentacaoDiaria;
    }

    public async Task<IEnumerable<MovimentacaoDiaria>> GetMovimentacaoDiariasByUserId(int userId)
    {
        var MovimentacaoDiarias = await _context.MovimentacaoDiaria.Where(c => c.UserId == userId).OrderBy(c => c.DataMovimentacao).ToListAsync();
        return MovimentacaoDiarias;
    }

    public async Task<MovimentacaoDiaria?> GetMovimentacaoDiariaById(int id, int userId)
    {
        var MovimentacaoDiaria = await _context.MovimentacaoDiaria
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();

        if (MovimentacaoDiaria == null)
            return null;

        return MovimentacaoDiaria;
    }


    public async Task<IEnumerable<MovimentacaoDiaria>> GetMovimentacaoByDate(DateTime dateTime, int userId)
    {
        return await _context.MovimentacaoDiaria
            .Include(m => m.Banco)
            .Include(m => m.TipoCartao)
            .Include(m => m.TipoMovimentacao)
            .Where(c => c.UserId == userId && c.DataMovimentacao.Month == dateTime.Month && c.DataMovimentacao.Year == dateTime.Year)
            .ToListAsync();
    }

    public async Task<MovimentacaoDiaria?> Remove(int id, int userId)
    {
        var MovimentacaoDiaria = await GetMovimentacaoDiariaById(id, userId);
        if (MovimentacaoDiaria == null)
            return null;

        _context.MovimentacaoDiaria.Remove(MovimentacaoDiaria);
        await _context.SaveChangesAsync();

        return MovimentacaoDiaria;
    }

    public async Task<MovimentacaoDiaria?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.MovimentacaoDiaria
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(MovimentacaoDiaria incomingMovimentacaoDiaria, int userId)
    {
        var existingMovimentacaoDiaria = await _context.MovimentacaoDiaria
            .FirstOrDefaultAsync(a => a.Id == incomingMovimentacaoDiaria.Id && a.UserId == userId);

        if (existingMovimentacaoDiaria == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingMovimentacaoDiaria.BancoId = incomingMovimentacaoDiaria.BancoId;
        existingMovimentacaoDiaria.TipoCartaoId = incomingMovimentacaoDiaria.TipoCartaoId;
        existingMovimentacaoDiaria.TipoMovimentacaoId = incomingMovimentacaoDiaria.TipoMovimentacaoId;
        existingMovimentacaoDiaria.Valor = incomingMovimentacaoDiaria.Valor;
        existingMovimentacaoDiaria.DataMovimentacao = incomingMovimentacaoDiaria.DataMovimentacao;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<MovimentacaoDiaria>>? GetMovimentacaoDiaria(int userId)
    {
        var movimentacoesDiarias = await _context.MovimentacaoDiaria
            .Include(m => m.Banco)
            .Include(m => m.TipoCartao)
            .Include(m => m.TipoMovimentacao)
           .Where(a => a.UserId == userId)
           .OrderBy(c => c.DataMovimentacao)
           .ToListAsync();

        return movimentacoesDiarias;
    }

    public async Task<decimal> GetValorLancadoAnteriormente(int id, int userId)
    {
        var ultimoValor = await _context.MovimentacaoDiaria
            .Where(c => c.Id == id && c.UserId == userId)
            .OrderByDescending(c => c.DataMovimentacao)
            .Select(c => c.Valor)               
            .FirstOrDefaultAsync();             

        return ultimoValor;
    }

}
