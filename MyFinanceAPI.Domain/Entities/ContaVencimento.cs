using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class ContaVencimento : BaseEntity
{
    public int ContaId { get; set; }
    public int Dia { get; set; }

    [JsonIgnore]
    public Account? Account { get; set; }  // Relacionamento com Category
    public ContaVencimento() { } // Construtor padrão

    public ContaVencimento(int contaId, int dia)
    {
        ContaId = contaId;
        Dia = dia;
    }
    public ContaVencimento(int id, int contaId, int dia)
    {
        Id = id;
        ContaId = contaId;
        Dia = dia;
    }

}
