using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Busca rápida do menu (card #72): de 2 letras em diante, até 5 de cada tipo.</summary>
public class BuscaService(IBuscaRepository repositorio) : IBuscaService
{
    public const int MinimoDeLetras = 2;
    public const int PorTipo = 5;
    private const int MaximoDeLetras = 100;

    public async Task<IReadOnlyList<BuscaResultadoDTO>> BuscarAsync(int userId, string? texto)
    {
        var termo = (texto ?? "").Trim();
        if (termo.Length < MinimoDeLetras)
            return [];
        if (termo.Length > MaximoDeLetras)
            termo = termo[..MaximoDeLetras];

        var achados = await repositorio.BuscarAsync(userId, termo.ToUpperInvariant(), PorTipo);
        return achados.Select(a => new BuscaResultadoDTO
        {
            Tipo = a.Tipo, Id = a.Id, Titulo = a.Titulo, Detalhe = string.IsNullOrWhiteSpace(a.Detalhe) ? null : a.Detalhe, ListaId = a.ListaId
        }).ToList();
    }
}
