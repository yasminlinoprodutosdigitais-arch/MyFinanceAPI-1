using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
public class BancoDTO
{
    public int Id { get; set; }
    public string NomeBanco { get; set; }
    public string? Agencia { get; set; }
    public string? NumeroConta { get; set; }
    public int TipoCartaoId { get; set; }
    public decimal? SaldoInicial { get; set; }
    public DateTime DataAlteracao { get; set;}

    public bool Ativo { get; set; }

    public TipoCartao? TipoCartao { get; set; }

    public BancoDTO() { } 
}
