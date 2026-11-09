using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class NotaFiscalRepository(ContextDB context) : INotaFiscalRepository
{
    public Task<NotaFiscal?> ObterPorItemAsync(int itemId, int userId) =>
        context.NotasFiscais.AsNoTracking()
            .Include(n => n.Itens.OrderBy(i => i.Ordem)).ThenInclude(i => i.Categoria)
            .FirstOrDefaultAsync(n => n.ExtratoBancarioItemId == itemId && n.UsuarioId == userId);

    public async Task SubstituirAsync(NotaFiscal nota)
    {
        var atual = await context.NotasFiscais.FirstOrDefaultAsync(n => n.ExtratoBancarioItemId == nota.ExtratoBancarioItemId && n.UsuarioId == nota.UsuarioId);
        if (atual != null)
        {
            context.NotasFiscais.Remove(atual);
            await context.SaveChangesAsync();
        }
        await context.NotasFiscais.AddAsync(nota);
        await context.SaveChangesAsync();
    }

    public async Task RemoverPorItemAsync(int itemId, int userId)
    {
        var atual = await context.NotasFiscais.FirstOrDefaultAsync(n => n.ExtratoBancarioItemId == itemId && n.UsuarioId == userId);
        if (atual == null)
            return;
        context.NotasFiscais.Remove(atual);
        await context.SaveChangesAsync();
    }
}
