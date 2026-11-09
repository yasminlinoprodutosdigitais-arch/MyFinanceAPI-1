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
    public bool? Ativo { get; set; }
    public DateTime DataAlteracao { get; set;}

    public TipoCartao TipoCartao { get; set; } = null!;

    public Banco() { } 
}
