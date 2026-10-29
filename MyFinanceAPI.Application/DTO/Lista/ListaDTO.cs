using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

/// <summary>Lista com os campos que os itens usam (card #67). <see cref="ArquivadaEm"/> e <see cref="Resumo"/> só saem.</summary>
public class ListaDTO
{
    public int Id { get; set; }
    [Obrigatorio, TextoAte(120)] public string Nome { get; set; } = null!;
    [TextoAte(40)] public string? Icone { get; set; }

    public bool UsaQuantidade { get; set; }
    public bool UsaValor { get; set; }
    [TextoAte(10), UmDe(ModoValorLista.Unitario, ModoValorLista.Total)]
    public string ModoValor { get; set; } = ModoValorLista.Total;
    public bool UsaLimite { get; set; }
    [Minimo(0)] public decimal? Limite { get; set; }
    public bool UsaData { get; set; }
    public bool UsaObservacao { get; set; }
    [TextoAte(20), UmDe(ModeloLista.Compras, ModeloLista.Orcamento, ModeloLista.Tarefas, ModeloLista.Agenda, ModeloLista.Branco)]
    public string Modelo { get; set; } = ModeloLista.Branco;

    /// <summary>Série da lista (comparação); muda só por duplicar e por PUT /listas/{id}/serie.</summary>
    public Guid SerieId { get; set; }
    /// <summary>Lista de onde esta foi duplicada.</summary>
    public int? ListaOrigemId { get; set; }

    /// <summary>Muda só por arquivar/desarquivar; o PUT ignora.</summary>
    public DateTime? ArquivadaEm { get; set; }
    /// <summary>Preenchido na listagem; ignorado na entrada.</summary>
    public ResumoListaDTO? Resumo { get; set; }

    public ListaDTO() { }
}

/// <summary>
/// Números do card da lista no Cadastro. Total soma qtd × valor (modo UNITARIO) ou o valor (TOTAL);
/// item sem valor fica fora. <see cref="ProximaData"/> é a menor data de hoje em diante entre os pendentes.
/// </summary>
public class ResumoListaDTO
{
    public int Itens { get; set; }
    public int Feitos { get; set; }
    public decimal Total { get; set; }
    public decimal TotalFeito { get; set; }
    public DateOnly? ProximaData { get; set; }
}
