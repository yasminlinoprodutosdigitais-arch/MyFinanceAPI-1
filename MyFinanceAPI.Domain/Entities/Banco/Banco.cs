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
    public bool? Ativo { get; set; }
    public DateTime DataAlteracao { get; set;}

    public TipoCartao TipoCartao { get; set; } = null!;

    public Banco() { } 
}
