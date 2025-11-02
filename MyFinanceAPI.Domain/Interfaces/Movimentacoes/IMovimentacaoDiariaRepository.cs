using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IMovimentacaoDiariaRepository
{
    Task<List<MovimentacaoDiaria>>? GetMovimentacaoDiaria(int userId);
    Task<MovimentacaoDiaria> GetMovimentacaoDiariaById(int id, int userId);
    Task<MovimentacaoDiaria> Create(MovimentacaoDiaria MovimentacaoDiaria);
    Task<bool> UpdateAsync(MovimentacaoDiaria MovimentacaoDiaria, int userId);
    Task<MovimentacaoDiaria?> Remove(int id, int userId);
}
