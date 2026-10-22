using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class BancoDTO
{
    public int Id { get; set; }
    [TextoAte(120)] public string NomeBanco { get; set; }
    [TextoAte(10)] public string? Agencia { get; set; }
    [TextoAte(20)] public string? NumeroConta { get; set; }
    public int TipoCartaoId { get; set; }
    public decimal? SaldoInicial { get; set; }
    public DateTime DataAlteracao { get; set;}

    public bool Ativo { get; set; }

    public TipoCartao? TipoCartao { get; set; }

    public BancoDTO() { } 
}
