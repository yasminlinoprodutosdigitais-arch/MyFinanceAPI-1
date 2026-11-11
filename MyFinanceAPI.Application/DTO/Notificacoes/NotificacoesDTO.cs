namespace MyFinanceAPI.Application.DTO.Notificacoes;

/// <summary>GET /notificacoes (card #73): os avisos de agora, calculados, e quantos são novos.</summary>
public class NotificacoesDTO
{
    public List<AvisoDTO> Avisos { get; set; } = [];
    /// <summary>Avisos com <see cref="AvisoDTO.Desde"/> depois do "lido até".</summary>
    public int Novos { get; set; }
    public DateTime? LidasAte { get; set; }
    /// <summary>Preenchido enquanto "Pausar tudo" vale: o sino fica sem avisos até esse dia.</summary>
    public DateOnly? PausadoAte { get; set; }
}

/// <summary>Um aviso do sino. Não é gravado: o <see cref="Id"/> é estável enquanto o aviso valer.</summary>
public class AvisoDTO
{
    /// <summary>Ex.: "vencer:123:2026-11-12", "vencidas:2026-11-11".</summary>
    public string Id { get; set; } = "";
    /// <summary>"contas-a-vencer" | "vencidas" | "fatura" | "agenda" | "acertos".</summary>
    public string Tipo { get; set; } = "";
    /// <summary>Quando o aviso passou a valer (UTC; meia-noite de São Paulo do dia em que começou).</summary>
    public DateTime Desde { get; set; }
    public bool Novo { get; set; }
    public string Titulo { get; set; } = "";
    public string? Detalhe { get; set; }
    public decimal? Valor { get; set; }
    /// <summary>Data de referência: vencimento, fechamento, dia da agenda, compra mais antiga em aberto.</summary>
    public DateOnly? Data { get; set; }
    public int? TransacaoId { get; set; }
    public int? BancoId { get; set; }
    public int? ListaId { get; set; }
    public int? PessoaId { get; set; }
}
