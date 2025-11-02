using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IListaRepository
{
    Task<List<Lista>>? GetListas(int userId);
    Task<Lista> GetListaById(int id, int userId);
    Task<Lista> Create(Lista Lista);
    Task<bool> UpdateAsync(Lista Lista, int userId);
    Task<Lista?> Remove(int id, int userId);
}
