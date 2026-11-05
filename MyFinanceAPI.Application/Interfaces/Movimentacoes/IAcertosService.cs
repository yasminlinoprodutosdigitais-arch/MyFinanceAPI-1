using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Acertos com pessoas (card #45, parte 3).</summary>
public interface IAcertosService
{
    /// <summary>inicio e fim em "AAAA-MM" (vazios = sem limite); bancoId filtra as compras da conta e os pagamentos que as abatem.</summary>
    Task<AcertosDTO> ObterAsync(int userId, string? inicio, string? fim, int? bancoId);
}
