using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TipoMovimentacaoRepository(ContextDB context) : ITipoMovimentacaoRepository
{
    private readonly ContextDB _context = context;

    public async Task<TipoMovimentacao> Create(TipoMovimentacao TipoMovimentacao)
    {
        await _context.TipoMovimentacao.AddAsync(TipoMovimentacao);
        await _context.SaveChangesAsync();
        return TipoMovimentacao;
    }

    public async Task<IEnumerable<TipoMovimentacao>> GetTipoMovimentacaoByUserId(int userId)
    {
        var TipoMovimentacao = await _context.TipoMovimentacao.Where(c => c.UserId == userId).OrderBy(c => c.NomeTipoMovimentacao).ToListAsync();
        return TipoMovimentacao;
    }

    public async Task<TipoMovimentacao?> GetTipoMovimentacaoById(int id, int userId)
    {
        var TipoMovimentacao = await _context.TipoMovimentacao
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();

        if (TipoMovimentacao == null)
            return null;

        return TipoMovimentacao;
    }

    public async Task<TipoMovimentacao?> Remove(int id, int userId)
    {
        var TipoMovimentacao = await GetTipoMovimentacaoById(id, userId);
        if (TipoMovimentacao == null)
            return null;

        _context.TipoMovimentacao.Remove(TipoMovimentacao);
        await _context.SaveChangesAsync();

        return TipoMovimentacao;
    }

    public async Task<TipoMovimentacao?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.TipoMovimentacao
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(TipoMovimentacao incomingTipoMovimentacao, int userId)
    {
        var existingTipoMovimentacao = await _context.TipoMovimentacao
            .FirstOrDefaultAsync(a => a.Id == incomingTipoMovimentacao.Id && a.UserId == userId);

        if (existingTipoMovimentacao == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingTipoMovimentacao.NomeTipoMovimentacao = incomingTipoMovimentacao.NomeTipoMovimentacao;
        existingTipoMovimentacao.Descricao = incomingTipoMovimentacao.Descricao;
        existingTipoMovimentacao.ValorMeta = incomingTipoMovimentacao.ValorMeta;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<TipoMovimentacao>>? GetTipoMovimentacao(int userId)
    {
        var movimentacoesDiarias = await _context.TipoMovimentacao
           .Where(a => a.UserId == userId)
           .OrderBy(c => c.NomeTipoMovimentacao)
           .ToListAsync();

        return movimentacoesDiarias;
    }
}
