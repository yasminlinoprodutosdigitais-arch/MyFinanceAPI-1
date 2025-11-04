using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IBancoService
{
    Task Add(BancoDTO BancoDTO, int userId);
    Task<IEnumerable<BancoDTO>> GetBanco(int userId);
    Task<BancoDTO> GetBancoById(int id, int userId);
    Task<bool> UpdateAsync(BancoDTO dto, int userId);
    Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId);
    Task Remove(int id, int userId);
}
