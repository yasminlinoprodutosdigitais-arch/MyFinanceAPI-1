using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class TipoMovimentacao : BaseEntity
{
    public string NomeTipoMovimentacao { get; set; }
    public string Descricao { get; set; }
    public decimal ValorMeta { get; set; }
        
    public TipoMovimentacao() { } 

    public TipoMovimentacao(string nomeTipoMovimentacao, string descricao, decimal valorMeta)
    {
        NomeTipoMovimentacao = nomeTipoMovimentacao;
        Descricao = descricao;
        ValorMeta = valorMeta;
    }

    public TipoMovimentacao(int id, string nomeTipoMovimentacao, string descricao, decimal valorMeta)
    {
        Id = id;
        NomeTipoMovimentacao = nomeTipoMovimentacao;
        Descricao = descricao;
        ValorMeta = valorMeta;
    }

}
