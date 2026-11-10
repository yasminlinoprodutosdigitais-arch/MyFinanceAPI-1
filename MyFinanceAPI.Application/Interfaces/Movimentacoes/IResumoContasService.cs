using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Números por conta no mês (card #77).</summary>
public interface IResumoContasService
{
    Task<ResumoContasDTO> ObterAsync(int userId, string? mes);
}
