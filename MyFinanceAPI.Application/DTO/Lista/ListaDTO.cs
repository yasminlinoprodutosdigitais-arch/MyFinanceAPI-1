using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class ListaDTO
{
    public int Id { get; set; }
    public string NomeLista { get; set; }
    public string TipoMovimentacao { get; set; }
        
    public ListaDTO() { } 

    public ListaDTO(string nomeLista, string tipoMovimentacao)
    {
        NomeLista = nomeLista;
        TipoMovimentacao = tipoMovimentacao;
    }

    public ListaDTO(int id, string nomeLista, string tipoMovimentacao)
    {
        Id = id;
        NomeLista = nomeLista;
        TipoMovimentacao = tipoMovimentacao;
    }

}
