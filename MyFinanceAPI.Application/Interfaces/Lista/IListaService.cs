using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IListaService
{
    Task Add(ListaDTO ListaDTO, int userId);
    Task<IEnumerable<ListaDTO>> GetLista(int userId);
    Task<ListaDTO> GetListaById(int id, int userId);
    Task<bool> UpdateAsync(ListaDTO dto, int userId);
    Task Remove(int id, int userId);
}
