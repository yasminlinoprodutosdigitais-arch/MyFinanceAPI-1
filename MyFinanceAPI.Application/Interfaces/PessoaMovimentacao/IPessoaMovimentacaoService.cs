using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IPessoaMovimentacaoService
{
    Task Add(PessoaMovimentacaoDTO PessoaMovimentacaoDTO, int userId);
    Task<IEnumerable<PessoaMovimentacaoDTO>> GetPessoaMovimentacao(int userId);
    Task<PessoaMovimentacaoDTO> GetPessoaMovimentacaoById(int id, int userId);
    Task<bool> UpdateAsync(PessoaMovimentacaoDTO dto, int userId);
    Task Remove(int id, int userId);
}
