namespace MyFinanceAPI.Application.DTO.Movimentacoes;

/// <summary>Acertos com pessoas (card #45, parte 3): quem me deve, por pessoa, e os totais.</summary>
public class AcertosDTO
{
    public List<AcertoPessoaDTO> Pessoas { get; set; } = [];
    /// <summary>Minhas compras (o "Eu"), à parte: nunca somam nos totais.</summary>
    public AcertoPessoaDTO? Eu { get; set; }
    public TotaisAcertosDTO Totais { get; set; } = new();
}

public class TotaisAcertosDTO
{
    public decimal Devido { get; set; }
    public decimal Recebido { get; set; }
    /// <summary>Soma do que falta de cada pessoa que ainda deve (quem pagou a mais não abate dos outros).</summary>
    public decimal FaltaReceber { get; set; }
    public decimal AindaVaiCobrar { get; set; }
}

public class AcertoPessoaDTO
{
    public int PessoaId { get; set; }
    public string Nome { get; set; } = "";
    public bool EhTitular { get; set; }
    public decimal Devido { get; set; }
    public decimal Recebido { get; set; }
    public decimal Falta { get; set; }
    public bool Quitado { get; set; }
    /// <summary>Parcelas futuras das compras parceladas, à parte do Falta.</summary>
    public decimal AindaVaiCobrar { get; set; }
    public List<AcertoCompraDTO> Compras { get; set; } = [];
    public List<AcertoPagamentoDTO> Pagamentos { get; set; } = [];
    public List<AcertoParcelaFuturaDTO> ProximasParcelas { get; set; } = [];
}

public class AcertoCompraDTO
{
    public int Id { get; set; }
    public DateOnly Data { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public int? BancoId { get; set; }
    public string? BancoNome { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    /// <summary>Soma dos pagamentos que abatem esta compra (de qualquer data).</summary>
    public decimal Pago { get; set; }
    public bool Quitada { get; set; }
}

public class AcertoPagamentoDTO
{
    public int Id { get; set; }
    public DateOnly Data { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public int? BancoId { get; set; }
    public string? BancoNome { get; set; }
    /// <summary>Compra abatida; nulo = saldo geral.</summary>
    public int? AbateItemId { get; set; }
}

public class AcertoParcelaFuturaDTO
{
    /// <summary>"AAAA-MM".</summary>
    public string Mes { get; set; } = "";
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public int Parcela { get; set; }
    public int QuantidadeParcelas { get; set; }
}
