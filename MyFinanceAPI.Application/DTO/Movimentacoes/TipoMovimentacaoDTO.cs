using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class TipoMovimentacaoDTO
{
    public int Id { get; set; }
    public string NomeTipoMovimentacao { get; set; }
    public string Descricao { get; set; }
    public decimal ValorMeta { get; set; }
        
    public TipoMovimentacaoDTO() { } 

    public TipoMovimentacaoDTO(string nomeTipoMovimentacao, string descricao, decimal valorMeta)
    {
        NomeTipoMovimentacao = nomeTipoMovimentacao;
        Descricao = descricao;
        ValorMeta = valorMeta;
    }

    public TipoMovimentacaoDTO(int id, string nomeTipoMovimentacao, string descricao, decimal valorMeta)
    {
        Id = id;
        NomeTipoMovimentacao = nomeTipoMovimentacao;
        Descricao = descricao;
        ValorMeta = valorMeta;
    }

}
