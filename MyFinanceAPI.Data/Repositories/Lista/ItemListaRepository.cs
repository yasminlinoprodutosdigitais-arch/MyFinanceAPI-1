using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ItemListaRepository(ContextDB context) : IItemListaRepository
{
    private readonly ContextDB _context = context;

    public async Task<ItemLista> Create(ItemLista ItemLista)
    {
        await _context.ItemLista.AddAsync(ItemLista);
        await _context.SaveChangesAsync();
        return ItemLista;
    }

     public async Task<IEnumerable<ItemLista>> GetItemListasByUserId(int userId)
    {
        var ItemListas = await _context.ItemLista.Where(c => c.UserId == userId).OrderBy(c => c.Lista).ToListAsync();
        return ItemListas;
    }

    public async Task<ItemLista?> GetItemListaById(int id, int userId)
    {
        var ItemLista = await _context.ItemLista
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();

        if (ItemLista == null)
            return null;

        return ItemLista;
    }

    public async Task<ItemLista?> Remove(int id, int userId)
    {
        var ItemLista = await GetItemListaById(id, userId);
        if (ItemLista == null)
            return null;

        _context.ItemLista.Remove(ItemLista);
        await _context.SaveChangesAsync();

        return ItemLista;
    }

    public async Task<ItemLista?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.ItemLista
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(ItemLista incomingItemLista, int userId)
    {
        var existingItemLista = await _context.ItemLista
            .FirstOrDefaultAsync(a => a.Id == incomingItemLista.Id && a.UserId == userId);

        if (existingItemLista == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        existingItemLista.ListaId = incomingItemLista.ListaId;
        existingItemLista.Descricao = incomingItemLista.Descricao;
        existingItemLista.Quantidade = incomingItemLista.Quantidade;
        existingItemLista.Status = incomingItemLista.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ItemLista>>? GetItemListas(int userId)
    {
        var movimentacoesDiarias = await _context.ItemLista
           .Where(a => a.UserId == userId)
           .OrderBy(c => c.Lista)
           .ToListAsync();

        return movimentacoesDiarias;
    }
}
