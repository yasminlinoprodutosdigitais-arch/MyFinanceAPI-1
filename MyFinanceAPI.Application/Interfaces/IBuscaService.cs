using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Busca rápida do menu (card #72).</summary>
public interface IBuscaService
{
    Task<IReadOnlyList<BuscaResultadoDTO>> BuscarAsync(int userId, string? texto);
}
