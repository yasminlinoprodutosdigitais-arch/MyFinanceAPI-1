using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IItemListaRepository
{
    Task<List<ItemLista>>? GetItemListas(int userId);
    Task<ItemLista> GetItemListaById(int id, int userId);
    Task<ItemLista> Create(ItemLista ItemLista);
    Task<bool> UpdateAsync(ItemLista ItemLista, int userId);
    Task<ItemLista?> Remove(int id, int userId);
}
