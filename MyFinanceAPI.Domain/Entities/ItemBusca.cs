namespace MyFinanceAPI.Domain.Entities;

/// <summary>Um resultado da busca rápida (card #72). Tipo: planejamento, item, categoria ou conta-fixa.</summary>
public record ItemBusca(string Tipo, int Id, string Titulo, string? Detalhe, int? ListaId);

public static class TiposBusca
{
    public const string Planejamento = "planejamento";
    public const string Item = "item";
    public const string Categoria = "categoria";
    public const string ContaFixa = "conta-fixa";
}
