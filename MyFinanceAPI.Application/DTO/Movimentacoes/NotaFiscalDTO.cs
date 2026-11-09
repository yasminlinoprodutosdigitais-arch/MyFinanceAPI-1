namespace MyFinanceAPI.Application.DTO.Movimentacoes;

/// <summary>Nota do lançamento, lida pelo QR Code (card #76, fase 6).</summary>
public class NotaFiscalDTO
{
    public string Estabelecimento { get; set; } = "";
    public decimal Total { get; set; }
    /// <summary>Endereço da consulta na Fazenda.</summary>
    public string? Link { get; set; }
    public List<NotaFiscalItemDTO> Itens { get; set; } = [];
}

public class NotaFiscalItemDTO
{
    public string Nome { get; set; } = "";
    public decimal Quantidade { get; set; }
    public decimal Valor { get; set; }
    /// <summary>Sugerida pelas regras de classificação; null = sem sugestão.</summary>
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
}

/// <summary>Resultado de salvar a nota: se classificou o lançamento e o que dizer na tela.</summary>
public class NotaSalvaDTO
{
    public bool Classificado { get; set; }
    public string Mensagem { get; set; } = "";
}

public class LerNotaFiscalDTO
{
    public string? Link { get; set; }
}
