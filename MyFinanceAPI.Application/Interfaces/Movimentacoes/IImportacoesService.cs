using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Histórico, cobertura por mês e desfazer das importações (card #75).</summary>
public interface IImportacoesService
{
    /// <param name="meses">Importadas nos últimos N meses; null = todas.</param>
    Task<IReadOnlyList<ImportacaoDTO>> ListarAsync(int userId, int? meses);
    Task<CoberturaImportacoesDTO> CoberturaAsync(int userId, int meses);
    Task<VinculosImportacaoDTO> VinculosAsync(int userId, int id);
    Task DesfazerAsync(int userId, int id, string? motivo);
}
