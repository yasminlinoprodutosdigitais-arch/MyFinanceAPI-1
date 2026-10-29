namespace MyFinanceAPI.Domain.Entities;

public class ItemLista : BaseEntity
{
    public int ListaId { get; set; }
    public string Descricao { get; set; } = null!;
    public bool Feito { get; set; }
    /// <summary>Quando foi marcado como feito; null enquanto pendente.</summary>
    public DateTime? FeitoEm { get; set; }
    public int? Quantidade { get; set; }
    public decimal? Valor { get; set; }
    /// <summary>Na agenda, é a data do item; num item que repete, a primeira ocorrência.</summary>
    public DateOnly? Data { get; set; }
    /// <summary>Sem hora = dia todo.</summary>
    public TimeOnly? Hora { get; set; }
    /// <summary><see cref="RepeticaoItem"/>.</summary>
    public string Repeticao { get; set; } = RepeticaoItem.Nenhuma;
    /// <summary>Dias da repetição semanal: 1 = segunda … 7 = domingo.</summary>
    public int[]? DiasSemana { get; set; }
    /// <summary>Última data em que a repetição vale; null = sem fim.</summary>
    public DateOnly? RepetirAte { get; set; }
    public string? Observacao { get; set; }
    public int? Ordem { get; set; }
    /// <summary>Item de onde este foi copiado ao duplicar a lista: pareia os itens na comparação.</summary>
    public int? ItemOrigemId { get; set; }

    public Lista? Lista { get; set; }

    public ItemLista() { }
}
