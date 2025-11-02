using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITipoMovimentacaoService
{
    Task Add(TipoMovimentacaoDTO TipoMovimentacaoDTO, int userId);
    Task<IEnumerable<TipoMovimentacaoDTO>> GetTipoMovimentacao(int userId);
    Task<TipoMovimentacaoDTO> GetTipoMovimentacaoById(int id, int userId);
    Task<bool> UpdateAsync(TipoMovimentacaoDTO dto, int userId);
    Task Remove(int id, int userId);
}
