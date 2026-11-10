using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class BancoDTO
{
    public int Id { get; set; }
    [Obrigatorio, TextoAte(120)] public string NomeBanco { get; set; } = null!;
    [TextoAte(10)] public string? Agencia { get; set; }
    [TextoAte(20)] public string? NumeroConta { get; set; }
    public int TipoCartaoId { get; set; }
    public decimal? SaldoInicial { get; set; }
    /// <summary>Dia do vencimento da fatura (1 a 31), só para cartão de crédito.</summary>
    public int? DiaVencimento { get; set; }
    /// <summary>Dia do fechamento da fatura (1 a 31), só para cartão de crédito (card #77).</summary>
    public int? DiaFechamento { get; set; }
    /// <summary>Outras pessoas usam este cartão (card #77).</summary>
    public bool Compartilhado { get; set; }
    public bool ConviteCompartilharDispensado { get; set; }
    /// <summary>Pessoas cadastradas que usam o cartão; o "Eu" não entra.</summary>
    public List<int> PessoasIds { get; set; } = [];
    public DateTime DataAlteracao { get; set;}

    public bool Ativo { get; set; }

    public TipoCartao? TipoCartao { get; set; }

    public BancoDTO() { } 
}
