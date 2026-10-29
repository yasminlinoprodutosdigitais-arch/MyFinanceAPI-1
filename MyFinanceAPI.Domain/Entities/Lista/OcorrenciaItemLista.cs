namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Uma ocorrência de item que repete, só quando difere da regra (card #67, parte 3): feita, movida
/// ("só esta" em outro dia ou hora) ou pulada (excluída só esta). <see cref="Data"/> é a data original.
/// </summary>
public class OcorrenciaItemLista : BaseEntity
{
    public int ItemId { get; set; }
    public DateOnly Data { get; set; }
    public bool Feito { get; set; }
    public DateTime? FeitoEm { get; set; }
    public DateOnly? NovaData { get; set; }
    public TimeOnly? NovaHora { get; set; }
    public bool Pulada { get; set; }

    public OcorrenciaItemLista() { }
}
