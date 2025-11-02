using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITipoCartaoRepository
{
    Task<List<TipoCartao>>? GetTipoCartao(int userId);
    Task<TipoCartao> GetTipoCartaoById(int id, int userId);
    Task<TipoCartao> Create(TipoCartao TipoCartao);
    Task<bool> UpdateAsync(TipoCartao TipoCartao, int userId);
    Task<TipoCartao?> Remove(int id, int userId);
}
