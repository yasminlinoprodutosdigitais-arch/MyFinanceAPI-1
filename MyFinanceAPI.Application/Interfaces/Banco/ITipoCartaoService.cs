using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITipoCartaoService
{
    Task Add(TipoCartaoDTO TipoCartaoDTO, int userId);
    Task<IEnumerable<TipoCartaoDTO>> GetTipoCartao(int userId);
    Task<TipoCartaoDTO> GetTipoCartaoById(int id, int userId);
    Task<bool> UpdateAsync(TipoCartaoDTO dto, int userId);
    Task Remove(int id, int userId);
}
