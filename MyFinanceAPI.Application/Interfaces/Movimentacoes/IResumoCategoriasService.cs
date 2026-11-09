using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Resumo do mês por categoria (card #76, fase 5).</summary>
public interface IResumoCategoriasService
{
    Task<ResumoCategoriasDTO> ObterAsync(int userId, string? mes);
    /// <summary>Evolução até o mês informado, do mais antigo ao mais recente (card #62).</summary>
    Task<EvolucaoExtratosDTO> ObterEvolucaoAsync(int userId, string? mes, int meses);
}
