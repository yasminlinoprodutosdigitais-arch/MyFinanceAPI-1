using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ListaRepository(ContextDB context) : IListaRepository
{
    private readonly ContextDB _context = context;

    public async Task<Lista> Create(Lista Lista)
    {
        await _context.Lista.AddAsync(Lista);
        await _context.SaveChangesAsync();
        return Lista;
    }

    public async Task<IEnumerable<Lista>> GetListaByUserId(int userId)
    {
        var Lista = await _context.Lista.Where(c => c.UserId == userId).OrderBy(c => c.NomeLista).ToListAsync();
        return Lista;
    }

    public async Task<Lista?> GetListaById(int id, int userId)
    {
        var Lista = await _context.Lista
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();

        if (Lista == null)
            return null;

        return Lista;
    }

    public async Task<Lista?> Remove(int id, int userId)
    {
        var Lista = await GetListaById(id, userId);
        if (Lista == null)
            return null;

        _context.Lista.Remove(Lista);
        await _context.SaveChangesAsync();

        return Lista;
    }

    public async Task<Lista?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.Lista
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(Lista incomingLista, int userId)
    {
        var existingLista = await _context.Lista
            .FirstOrDefaultAsync(a => a.Id == incomingLista.Id && a.UserId == userId);

        if (existingLista == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingLista.NomeLista = incomingLista.NomeLista;
        existingLista.TipoMovimentacao = incomingLista.TipoMovimentacao;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Lista>>? GetListas(int userId)
    {
        var movimentacoesDiarias = await _context.Lista
           .Where(a => a.UserId == userId)
           .OrderBy(c => c.NomeLista)
           .ToListAsync();

        return movimentacoesDiarias;
    }
}
