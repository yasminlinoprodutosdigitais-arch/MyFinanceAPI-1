namespace MyFinanceAPI.Application.DTO;

/// <summary>Um resultado da busca rápida (card #72).</summary>
public class BuscaResultadoDTO
{
    /// <summary>planejamento, item, categoria ou conta-fixa.</summary>
    public string Tipo { get; set; } = "";
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    /// <summary>Planejamento do item, subcategoria, categoria da conta fixa ou "Arquivado".</summary>
    public string? Detalhe { get; set; }
    /// <summary>Planejamento onde o item está (só para item).</summary>
    public int? ListaId { get; set; }
}
