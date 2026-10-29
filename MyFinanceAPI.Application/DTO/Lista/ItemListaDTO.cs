using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

/// <summary>Item de qualquer lista (card #67): os campos opcionais aparecem conforme a lista. <see cref="FeitoEm"/> só sai.</summary>
public class ItemListaDTO
{
    public int Id { get; set; }
    /// <summary>Vem da rota (/listas/{id}/itens); o corpo não decide a lista.</summary>
    public int ListaId { get; set; }
    [Obrigatorio, TextoAte(200)] public string Descricao { get; set; } = null!;
    public bool Feito { get; set; }
    public DateTime? FeitoEm { get; set; }
    [Minimo(1)] public int? Quantidade { get; set; }
    [Minimo(0)] public decimal? Valor { get; set; }
    public DateOnly? Data { get; set; }
    /// <summary>Sem hora = dia todo.</summary>
    public TimeOnly? Hora { get; set; }
    [TextoAte(10), UmDe(RepeticaoItem.Nenhuma, RepeticaoItem.Diaria, RepeticaoItem.Semanal, RepeticaoItem.Mensal)]
    public string Repeticao { get; set; } = RepeticaoItem.Nenhuma;
    /// <summary>1 = segunda … 7 = domingo; só na repetição semanal.</summary>
    public int[]? DiasSemana { get; set; }
    public DateOnly? RepetirAte { get; set; }
    [TextoAte(500)] public string? Observacao { get; set; }
    public int? Ordem { get; set; }
    /// <summary>Item de onde este foi copiado ao duplicar; só sai.</summary>
    public int? ItemOrigemId { get; set; }

    public ItemListaDTO() { }
}
