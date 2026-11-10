using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>Busca rápida (card #72): até <paramref name="limite"/> de cada tipo, só do usuário.</summary>
public interface IBuscaRepository
{
    Task<IReadOnlyList<ItemBusca>> BuscarAsync(int userId, string termoMaiusculo, int limite);
}
