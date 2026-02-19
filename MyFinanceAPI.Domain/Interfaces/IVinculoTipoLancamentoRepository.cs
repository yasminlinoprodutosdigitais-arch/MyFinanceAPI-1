using System;

namespace MyFinanceAPI.Domain.Entities;
public interface IVinculoTipoMovimentacaoRepository
{
    Task<VinculoTipoMovimentacao?> GetByChaveAsync(int userId, string chave);
    Task<VinculoTipoMovimentacao?> GetByIdAsync(int id, int userId);

    Task AddAsync(VinculoTipoMovimentacao vinculo);
    Task UpdateAsync(VinculoTipoMovimentacao vinculo);

    Task<List<VinculoTipoMovimentacao>> GetPendentesAsync(int userId);
}
