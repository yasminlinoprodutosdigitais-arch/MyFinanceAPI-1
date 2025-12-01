using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class BancoRepository(ContextDB context) : IBancoRepository
{
    private readonly ContextDB _context = context;

    public async Task<Banco> Create(Banco Banco)
    {
        await _context.Banco.AddAsync(Banco);
        await _context.SaveChangesAsync();
        return Banco;
    }

    public async Task<IEnumerable<Banco>> GetBancoByUserId(int userId)
    {
        var Banco = await _context.Banco.Where(c => c.UserId == userId).OrderBy(c => c.NomeBanco).ToListAsync();
        return Banco;
    }

    public async Task<Banco?> GetBancoById(int id, int userId)
    {
        var Banco = await _context.Banco
            .Where(c => c.UserId == userId && c.Id == id)
            .Include(c => c.TipoCartao)
            .FirstOrDefaultAsync();

        if (Banco == null)
            return null;

        return Banco;
    }

    public async Task<Banco?> Remove(int id, int userId)
    {
        var Banco = await GetBancoById(id, userId);
        if (Banco == null)
            return null;

        _context.Banco.Remove(Banco);
        await _context.SaveChangesAsync();

        return Banco;
    }

    public async Task<Banco?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.Banco
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(Banco incomingBanco, int userId)
    {
        var existingBanco = await _context.Banco
            .FirstOrDefaultAsync(a => a.Id == incomingBanco.Id && a.UserId == userId);

        if (existingBanco == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingBanco.NomeBanco = incomingBanco.NomeBanco;
        existingBanco.Agencia = incomingBanco.Agencia;
        existingBanco.NumeroConta = incomingBanco.NumeroConta;
        existingBanco.TipoCartaoId = incomingBanco.TipoCartaoId;
        existingBanco.SaldoInicial = incomingBanco.SaldoInicial;
        existingBanco.Ativo = incomingBanco.Ativo;      
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateSaldo(int bancoId, decimal SaldoAtual, int userId)
    {
        var existingBanco = await _context.Banco
            .FirstOrDefaultAsync(a => a.Id == bancoId && a.UserId == userId);

        if (existingBanco == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingBanco.SaldoInicial = SaldoAtual;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Banco>>? GetBancos(int userId)
    {
        var movimentacoesDiarias = await _context.Banco
           .Where(a => a.UserId == userId)
           .OrderBy(c => c.NomeBanco)
           .Include(c => c.TipoCartao)
           .ToListAsync();

        return movimentacoesDiarias;
    }
}
