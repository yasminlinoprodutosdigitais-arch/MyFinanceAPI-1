using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class MovimentacaoDiaria : BaseEntity
{
    public DateOnly DataMovimentacao { get; set; }
    public int? BancoId { get; set; }
    public int? TipoCartaoId { get; set; }
    public int? TipoMovimentacaoId { get; set; }

    public decimal Valor { get; set; }
    public string TipoLancamento { get; set; }
    public string? Descricao { get; set; }
    public string? NomePessoaTransacao { get; set; }
    public string? Identificador { get; set; }

    public Banco? Banco { get; set; }  
    public TipoCartao? TipoCartao { get; set; } 
    public TipoMovimentacao? TipoMovimentacao { get; set; } 
        
    public MovimentacaoDiaria() { } 
}
