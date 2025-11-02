using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TipoCartaoRepository(ContextDB context) : ITipoCartaoRepository
{
    private readonly ContextDB _context = context;

    public async Task<TipoCartao> Create(TipoCartao TipoCartao)
    {
        await _context.TipoCartao.AddAsync(TipoCartao);
        await _context.SaveChangesAsync();
        return TipoCartao;
    }

    public async Task<IEnumerable<TipoCartao>> GetTipoCartaoByUserId(int userId)
    {
        var TipoCartao = await _context.TipoCartao.Where(c => c.UserId == userId).OrderBy(c => c.NomeTipoCartao).ToListAsync();
        return TipoCartao;
    }

    public async Task<TipoCartao?> GetTipoCartaoById(int id, int userId)
    {
        var TipoCartao = await _context.TipoCartao
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();

        if (TipoCartao == null)
            return null;

        return TipoCartao;
    }

    public async Task<TipoCartao?> Remove(int id, int userId)
    {
        var TipoCartao = await GetTipoCartaoById(id, userId);
        if (TipoCartao == null)
            return null;

        _context.TipoCartao.Remove(TipoCartao);
        await _context.SaveChangesAsync();

        return TipoCartao;
    }

    public async Task<TipoCartao?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.TipoCartao
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(TipoCartao incomingTipoCartao, int userId)
    {
        var existingTipoCartao = await _context.TipoCartao
            .FirstOrDefaultAsync(a => a.Id == incomingTipoCartao.Id && a.UserId == userId);

        if (existingTipoCartao == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingTipoCartao.NomeTipoCartao = incomingTipoCartao.NomeTipoCartao;
        existingTipoCartao.Descricao = incomingTipoCartao.Descricao;
        
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<TipoCartao>>? GetTipoCartao(int userId)
    {
        var movimentacoesDiarias = await _context.TipoCartao
           .Where(a => a.UserId == userId)
           .OrderBy(c => c.NomeTipoCartao)
           .ToListAsync();

        return movimentacoesDiarias;
    }
}
