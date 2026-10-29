namespace MyFinanceAPI.Application.DTO;

/// <summary>
/// GET /listas/{id}/comparar (card #67, parte 2). A cascata fecha no centavo:
/// Preco + Quantidade + Novos − Sairam = Atual.Total − Anterior.Total.
/// </summary>
public class ComparacaoListasDTO
{
    public ListaComparadaDTO Anterior { get; set; } = null!;
    public ListaComparadaDTO Atual { get; set; } = null!;
    public decimal Diferenca { get; set; }
    /// <summary>Null quando o total anterior é zero.</summary>
    public decimal? Percentual { get; set; }
    public EfeitosComparacaoDTO Efeitos { get; set; } = new();
    /// <summary>Por maior impacto (|diferença|), depois pela descrição.</summary>
    public List<ItemComparadoDTO> Itens { get; set; } = [];
    public DestaquesComparacaoDTO Destaques { get; set; } = new();
    /// <summary>Itens sem valor em algum dos lados: ficam fora da conta.</summary>
    public List<string> SemValor { get; set; } = [];
}

public class ListaComparadaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public decimal Total { get; set; }
}

public class EfeitosComparacaoDTO
{
    public decimal Preco { get; set; }
    public decimal Quantidade { get; set; }
    public decimal Novos { get; set; }
    public decimal Sairam { get; set; }
}

public class ItemComparadoDTO
{
    public string Descricao { get; set; } = null!;
    /// <summary>Null = o item não existe nessa lista (NOVO ou SAIU).</summary>
    public LadoItemComparadoDTO? Anterior { get; set; }
    public LadoItemComparadoDTO? Atual { get; set; }
    public decimal Diferenca { get; set; }
    public decimal? Percentual { get; set; }
    /// <summary>NOVO, SAIU, PRECO_SUBIU, PRECO_CAIU, QUANTIDADE.</summary>
    public List<string> Mudancas { get; set; } = [];
}

public class LadoItemComparadoDTO
{
    public int? Quantidade { get; set; }
    public decimal? Valor { get; set; }
    /// <summary>Null quando o item não tem valor.</summary>
    public decimal? Total { get; set; }
}

public class DestaquesComparacaoDTO
{
    /// <summary>Maior alta do valor (%), entre itens com valor nas duas listas.</summary>
    public DestaqueComparacaoDTO? MaiorAltaPreco { get; set; }
    /// <summary>Maior diferença em R$, para mais ou para menos.</summary>
    public DestaqueComparacaoDTO? MaiorImpacto { get; set; }
    public DestaqueComparacaoDTO? MaiorQueda { get; set; }
}

public class DestaqueComparacaoDTO
{
    public string Descricao { get; set; } = null!;
    public decimal Diferenca { get; set; }
    public decimal? Percentual { get; set; }
}
