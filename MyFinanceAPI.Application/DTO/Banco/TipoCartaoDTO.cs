using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class TipoCartaoDTO 
{
    public int Id { get; set; }
    public string NomeTipoCartao { get; set; }
    public string? Descricao { get; set; }

    public TipoCartaoDTO() { } 

    public TipoCartaoDTO(string? nomeTipoCartao, string? descricao)
    {
        NomeTipoCartao = nomeTipoCartao;
        Descricao = descricao;
    }

    public TipoCartaoDTO(int id, string nomeTipoCartao, string descricao)
    {
        Id = id;
        NomeTipoCartao = nomeTipoCartao;
        Descricao = descricao;
    }
}
