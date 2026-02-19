using MyFinanceAPI.Domain.Interfaces;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MyFinanceAPI.Data.Repositories;
public class VinculoTipoMovimentacaoRepository : IVinculoTipoMovimentacaoRepository
{
    private readonly ContextDB _context;

    public VinculoTipoMovimentacaoRepository(ContextDB context)
    {
        _context = context;
    }

    public async Task<VinculoTipoMovimentacao?> GetByChaveAsync(
        int userId, string chave)
    {
        return await _context.VinculoTipoMovimentacao
            .FirstOrDefaultAsync(v =>
                v.UserId == userId &&
                v.ChaveDescricao == chave);
    }

    public async Task<VinculoTipoMovimentacao?> GetByIdAsync(int id, int userId)
    {
        return await _context.VinculoTipoMovimentacao
            .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);
    }

    public async Task AddAsync(VinculoTipoMovimentacao vinculo)
    {
        _context.VinculoTipoMovimentacao.Add(vinculo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(VinculoTipoMovimentacao vinculo)
    {
        _context.VinculoTipoMovimentacao.Update(vinculo);
        await _context.SaveChangesAsync();
    }

    public async Task<List<VinculoTipoMovimentacao>> GetPendentesAsync(int userId)
    {
        return await _context.VinculoTipoMovimentacao
            .Where(v => v.UserId == userId && v.TipoMovimentacaoId == null)
            .ToListAsync();
    }
}
