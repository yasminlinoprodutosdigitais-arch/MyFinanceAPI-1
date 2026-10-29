namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Lista com campos configuráveis (card #67): cada lista liga os campos que os itens usam.
/// Desligar um campo só esconde; os dados dos itens continuam gravados.
/// </summary>
public class Lista : BaseEntity
{
    public string Nome { get; set; } = null!;
    public string? Icone { get; set; }
    /// <summary>Chave de <see cref="PaletaCores"/>; null = o front deriva a cor dos campos (card #63).</summary>
    public string? Cor { get; set; }

    public bool UsaQuantidade { get; set; }
    public bool UsaValor { get; set; }
    /// <summary><see cref="ModoValorLista"/>.</summary>
    public string ModoValor { get; set; } = ModoValorLista.Total;
    /// <summary>Só vale com <see cref="UsaValor"/> (CHECK no banco).</summary>
    public bool UsaLimite { get; set; }
    public decimal? Limite { get; set; }
    public bool UsaData { get; set; }
    public bool UsaObservacao { get; set; }
    /// <summary><see cref="ModeloLista"/>; só informativo.</summary>
    public string Modelo { get; set; } = ModeloLista.Branco;

    /// <summary>Listas da mesma série se comparam (card #67, parte 2). Duplicar mantém a série; o banco gera uma nova ao criar.</summary>
    public Guid SerieId { get; set; }
    /// <summary>Lista de onde esta foi duplicada; null se foi criada do zero ou se a original foi excluída.</summary>
    public int? ListaOrigemId { get; set; }

    /// <summary>Null = lista em uso; preenchida = está na pasta Arquivadas.</summary>
    public DateTime? ArquivadaEm { get; set; }

    public Lista() { }
}
