using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IBancoRepository
{
    Task<List<Banco>>? GetBancos(int userId);
    Task<Banco> GetBancoById(int id, int userId);
    Task<Banco> Create(Banco Banco);
    Task<bool> UpdateAsync(Banco Banco, int userId);
    Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId);
    Task<Banco?> Remove(int id, int userId);
}
