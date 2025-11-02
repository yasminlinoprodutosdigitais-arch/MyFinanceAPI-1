using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class TipoCartao : BaseEntity
{
    public string NomeTipoCartao { get; set; }
    public string? Descricao { get; set; }

    public TipoCartao() { } 

    public TipoCartao(string? nomeTipoCartao, string? descricao)
    {
        NomeTipoCartao = nomeTipoCartao;
        Descricao = descricao;
    }

    public TipoCartao(int id, string nomeTipoCartao, string descricao)
    {
        Id = id;
        NomeTipoCartao = nomeTipoCartao;
        Descricao = descricao;
    }
}
