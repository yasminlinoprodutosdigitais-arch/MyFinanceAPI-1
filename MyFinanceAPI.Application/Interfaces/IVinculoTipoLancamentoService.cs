using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;
public interface IVinculoTipoMovimentacaoService
{
    Task<VinculoTipoMovimentacao> CriarSeNaoExistirAsync(int userId, string chave, string descricaoOriginal);
    Task<List<VinculoTipoMovimentacao>> ObterPendentesAsync(int userId);
    Task AtualizarVinculoAsync(int id, VinculoUpdateDTO dto, int userId);
}
