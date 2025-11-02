using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface ITipoMovimentacaoRepository
{
    Task<List<TipoMovimentacao>>? GetTipoMovimentacao(int userId);
    Task<TipoMovimentacao> GetTipoMovimentacaoById(int id, int userId);
    Task<TipoMovimentacao> Create(TipoMovimentacao TipoMovimentacao);
    Task<bool> UpdateAsync(TipoMovimentacao TipoMovimentacao, int userId);
    Task<TipoMovimentacao?> Remove(int id, int userId);
}
