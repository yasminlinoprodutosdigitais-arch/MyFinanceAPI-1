namespace MyFinanceAPI.Application.DTO.Movimentacoes;

/// <summary>Resumo do mês por categoria (card #76, fase 5).</summary>
public class ResumoCategoriasDTO
{
    public string Mes { get; set; } = "";
    public decimal Total { get; set; }
    /// <summary>Média dos meses comparados; null sem histórico.</summary>
    public decimal? MediaAnterior { get; set; }
    /// <summary>% em relação à média (positivo = acima); null sem histórico.</summary>
    public decimal? VariacaoTotal { get; set; }
    public int MesesComparados { get; set; }
    /// <summary>Saídas de quem entra nos acertos: não são gasto meu.</summary>
    public decimal DeOutrasPessoas { get; set; }
    /// <summary>Pagamento de fatura e transferência entre contas: contariam em dobro.</summary>
    public decimal ForaDoResumo { get; set; }
    public List<ResumoCategoriaLinhaDTO> Categorias { get; set; } = [];
}

/// <summary>Entradas × saídas mês a mês, sem pagamento de fatura e transferência entre contas (card #62).</summary>
public class EvolucaoExtratosDTO
{
    public List<EvolucaoExtratosMesDTO> Meses { get; set; } = [];
}

public class EvolucaoExtratosMesDTO
{
    public string Mes { get; set; } = "";
    public decimal Entradas { get; set; }
    public decimal Saidas { get; set; }
    public decimal Resultado { get; set; }
}

public class ResumoCategoriaLinhaDTO
{
    /// <summary>null = sem categoria.</summary>
    public int? CategoriaId { get; set; }
    public string Nome { get; set; } = "";
    public decimal Valor { get; set; }
    /// <summary>% do total do mês.</summary>
    public decimal Percentual { get; set; }
    public decimal? Media { get; set; }
    public decimal? Variacao { get; set; }
}
