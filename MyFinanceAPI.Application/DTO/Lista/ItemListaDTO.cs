using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class ItemListaDTO
{
    public int Id { get; set; }
    public int ListaId { get; set; }
    public string Descricao { get; set; }
    public int Quantidade { get; set; }
    public string Status { get; set; }

    public Lista? Lista { get; set; }  
        
    public ItemListaDTO() { } 

    public ItemListaDTO( int listaId, string descricao, int quantidade, string status)
    {
        ListaId = listaId;
        Descricao = descricao;
        Quantidade = quantidade;
        Status = status;
    }

    public ItemListaDTO(int id, int listaId, string descricao, int quantidade, string status)
    {
        Id = id;
        ListaId = listaId;
        Descricao = descricao;
        Quantidade = quantidade;
        Status = status;
    }

}
