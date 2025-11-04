using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class MovimentacaoDiaria : BaseEntity
{
    public DateOnly DataMovimentacao { get; set; }
    public int? BancoId { get; set; }
    public int? TipoCartaoId { get; set; }
    public int TipoMovimentacaoId { get; set; }
    public decimal Valor { get; set; }
    public string TipoLancamento { get; set; }
    public string? Descricao { get; set; }

    public Banco? Banco { get; set; }  
    public TipoCartao? TipoCartao { get; set; } 
    public TipoMovimentacao? TipoMovimentacao { get; set; } 
        
    public MovimentacaoDiaria() { } 

    public MovimentacaoDiaria(DateOnly dataMovimentacao, int? bancoId, int? tipoCartaoId, int tipoMovimentacaoId, decimal valor, string tipoLancamento, string? descricao)
    {
        DataMovimentacao = dataMovimentacao;
        BancoId = bancoId;
        TipoCartaoId = tipoCartaoId;
        TipoMovimentacaoId = tipoMovimentacaoId;
        Valor = valor;
        TipoLancamento = tipoLancamento;
        Descricao = descricao;
    }

    public MovimentacaoDiaria(int id, DateOnly dataMovimentacao, int? bancoId, int? tipoCartaoId, int tipoMovimentacaoId, decimal valor, string tipoLancamento, string? descricao)
    {
        Id = id;
        DataMovimentacao = dataMovimentacao;
        BancoId = bancoId;
        TipoCartaoId = tipoCartaoId;
        TipoMovimentacaoId = tipoMovimentacaoId;
        Valor = valor;
        TipoLancamento = tipoLancamento;
        Descricao = descricao;
    }

}
