using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IItemListaService
{
    Task Add(ItemListaDTO ItemListaDTO, int userId);
    Task<IEnumerable<ItemListaDTO>> GetItemLista(int userId);
    Task<ItemListaDTO> GetItemListaById(int id, int userId);
    Task<bool> UpdateAsync(ItemListaDTO dto, int userId);
    Task Remove(int id, int userId);
}
