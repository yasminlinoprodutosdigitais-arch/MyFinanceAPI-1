using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class ItemLista : BaseEntity
{
    public int ListaId { get; set; }
    public string Descricao { get; set; }
    public int Quantidade { get; set; }
    public string Status { get; set; }

    public Lista? Lista { get; set; }  
        
    public ItemLista() { } 

    public ItemLista( int listaId, string descricao, int quantidade, string status)
    {
        ListaId = listaId;
        Descricao = descricao;
        Quantidade = quantidade;
        Status = status;
    }

    public ItemLista(int id, int listaId, string descricao, int quantidade, string status)
    {
        Id = id;
        ListaId = listaId;
        Descricao = descricao;
        Quantidade = quantidade;
        Status = status;
    }

}
