using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IPessoaMovimentacaoRepository
{
    Task<List<PessoaMovimentacao>>? GetPessoaMovimentacao(int userId);
    Task<IEnumerable<PessoaMovimentacao>>? VerificaPossuiPessoa(string nomePessoa, int userId);
    Task<PessoaMovimentacao> GetPessoaMovimentacaoById(int id, int userId);
    Task<PessoaMovimentacao> Create(PessoaMovimentacao PessoaMovimentacao, int userId);
    Task<bool> UpdateAsync(PessoaMovimentacao PessoaMovimentacao, int userId);
    Task<PessoaMovimentacao?> Remove(int id, int userId);
}
