using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class MovimentacaoDiariaDTO
{
    public int Id { get; set; }
    public DateOnly DataMovimentacao { get; set; }
    public int BancoId { get; set; }
    public int TipoCartaoId { get; set; }
    public int TipoMovimentacaoId { get; set; }
    public decimal Valor { get; set; }

    public Banco? Banco { get; set; }  
    public TipoCartao? TipoCartao { get; set; } 
    public TipoMovimentacao? TipoMovimentacao { get; set; } 
        
    public MovimentacaoDiariaDTO() { } 

    public MovimentacaoDiariaDTO(DateOnly dataMovimentacao, int bancoId, int tipoCartaoId, int tipoMovimentacaoId, decimal valor)
    {
        DataMovimentacao = dataMovimentacao;
        BancoId = bancoId;
        TipoCartaoId = tipoCartaoId;
        TipoMovimentacaoId = tipoMovimentacaoId;
        Valor = valor;
    }

    public MovimentacaoDiariaDTO(int id, DateOnly dataMovimentacao, int bancoId, int tipoCartaoId, int tipoMovimentacaoId, decimal valor)
    {
        Id = id;
        DataMovimentacao = dataMovimentacao;
        BancoId = bancoId;
        TipoCartaoId = tipoCartaoId;
        TipoMovimentacaoId = tipoMovimentacaoId;
        Valor = valor;
    }

}
