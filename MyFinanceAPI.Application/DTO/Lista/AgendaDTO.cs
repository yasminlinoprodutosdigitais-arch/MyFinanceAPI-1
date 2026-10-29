using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

/// <summary>
/// Uma ocorrência na agenda (GET /listas/{id}/itens?de=&amp;ate=, card #67 parte 3). <see cref="Data"/> e
/// <see cref="Hora"/> já vêm com a mudança de "só esta"; <see cref="DataOriginal"/> identifica a ocorrência.
/// </summary>
public class OcorrenciaDTO
{
    public int ItemId { get; set; }
    public DateOnly DataOriginal { get; set; }
    public DateOnly Data { get; set; }
    /// <summary>Null = dia todo.</summary>
    public TimeOnly? Hora { get; set; }
    public bool Feito { get; set; }
    public DateTime? FeitoEm { get; set; }
    public bool Repete { get; set; }
    public string Descricao { get; set; } = null!;
    public string? Observacao { get; set; }
    public int? Quantidade { get; set; }
    public decimal? Valor { get; set; }
}

/// <summary>
/// "Só esta": PUT /listas/{id}/itens/{itemId}/ocorrencias/{dataOriginal}. Campo null = não muda.
/// <see cref="Data"/> igual à original desfaz a mudança de dia.
/// </summary>
public class AlterarOcorrenciaDTO
{
    public bool? Feito { get; set; }
    public DateOnly? Data { get; set; }
    public TimeOnly? Hora { get; set; }
    /// <summary>true = exclui só esta ocorrência.</summary>
    public bool? Pulada { get; set; }
}

/// <summary>"Esta e as próximas": POST /listas/{id}/itens/{itemId}/dividir.</summary>
public class DividirItemDTO
{
    public DateOnly APartirDe { get; set; }
    /// <summary>Como o item fica de <see cref="APartirDe"/> em diante; sem data, começa nela.</summary>
    [Obrigatorio] public ItemListaDTO Item { get; set; } = null!;
}
