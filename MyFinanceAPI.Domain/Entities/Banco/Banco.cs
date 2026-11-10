using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class Banco : BaseEntity
{
    public string NomeBanco { get; set; } = null!;
    public string? Agencia { get; set; }
    public string? NumeroConta { get; set; }
    public int TipoCartaoId { get; set; }
    public decimal SaldoInicial { get; set; }
    /** Dia do vencimento da fatura (1 a 31), só para cartão de crédito. */
    public int? DiaVencimento { get; set; }
    /** Dia do fechamento da fatura (1 a 31), só para cartão de crédito (card #77). */
    public int? DiaFechamento { get; set; }
    /** Outras pessoas usam este cartão (card #77). */
    public bool Compartilhado { get; set; }
    /** Respondeu "Agora não" ao convite de compartilhar: o convite não volta (card #77). */
    public bool ConviteCompartilharDispensado { get; set; }
    public bool? Ativo { get; set; }
    /** Quem usa o cartão (card #77). Fora do JSON: o DTO expõe só os ids. */
    [JsonIgnore] public List<BancoPessoa> Pessoas { get; set; } = [];
    public DateTime DataAlteracao { get; set;}

    public TipoCartao TipoCartao { get; set; } = null!;

    public Banco() { } 
}
