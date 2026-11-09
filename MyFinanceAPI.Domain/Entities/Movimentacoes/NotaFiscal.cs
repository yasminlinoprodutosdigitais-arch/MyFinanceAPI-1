namespace MyFinanceAPI.Domain.Entities;

/// <summary>Nota fiscal lançada num lançamento (card #76, fase 6): uma por lançamento, para consulta e classificação.</summary>
public class NotaFiscal : BaseEntity
{
    public int ExtratoBancarioItemId { get; set; }
    public ExtratoBancarioItem? ExtratoBancarioItem { get; set; }
    public string Estabelecimento { get; set; } = "";
    public decimal Total { get; set; }
    /// <summary>Endereço da consulta na Fazenda.</summary>
    public string? Link { get; set; }
    public List<NotaFiscalItem> Itens { get; set; } = new();
}

public class NotaFiscalItem : BaseEntity
{
    public int NotaFiscalId { get; set; }
    public NotaFiscal? NotaFiscal { get; set; }
    public string Nome { get; set; } = "";
    public decimal Quantidade { get; set; }
    public decimal Valor { get; set; }
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public int Ordem { get; set; }
}
