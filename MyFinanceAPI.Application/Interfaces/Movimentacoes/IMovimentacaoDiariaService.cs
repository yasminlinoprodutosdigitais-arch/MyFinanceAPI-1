using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IMovimentacaoDiariaService
{
    Task Add(MovimentacaoDiariaDTO MovimentacaoDiariaDTO, int userId);
    Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoDiaria(int userId);
    Task<MovimentacaoDiariaDTO> GetMovimentacaoDiariaById(int id, int userId);
    Task<bool> UpdateAsync(MovimentacaoDiariaDTO dto, int userId);
    Task Remove(int id, int userId);
}
