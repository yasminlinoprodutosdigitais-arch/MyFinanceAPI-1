using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class ListaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int TipoMovimentacao { get; set; }
    public bool Status { get; set; }
        
    public ListaDTO() { } 

    public ListaDTO(string nomeLista, int tipoMovimentacao, bool status)
    {
        Nome = nomeLista;
        TipoMovimentacao = tipoMovimentacao;
        Status = status;
    }

    public ListaDTO(int id, string nomeLista, int tipoMovimentacao, bool status)
    {
        Id = id;
        Nome = nomeLista;
        TipoMovimentacao = tipoMovimentacao;
        Status = status;
    }

}
