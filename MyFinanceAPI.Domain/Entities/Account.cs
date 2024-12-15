using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class Account : BaseEntity
{
     public string Name { get; set; }
    public decimal Value { get; set; }

    public int CategoryId { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }  // Relacionamento com Category
    public ICollection<Transaction>? Transactions { get; set; } // Relacionamento com MonthlyUpdate

    public Account() { } // Construtor padrão

    public Account(string name, decimal value)
    {
        Name = name;
        Value = value;
    }

    public Account(int id, string name, decimal value)
    {
        Id = id;
        Name = name;
        Value = value;
    }
}
