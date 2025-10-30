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
    public int DataOperacao { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }  // Relacionamento com Category
    public ICollection<Transaction>? Transactions { get; set; } // Relacionamento com MonthlyUpdate

    public Account() { } // Construtor padrão

    public Account(string name, decimal value, int categoryId, int dataOperacao)
    {
        Name = name;
        Value = value;
        CategoryId = categoryId;
        DataOperacao = dataOperacao;
    }

    public Account(int id, string name, decimal value, int categoryId, int dataOperacao)
    {
        Id = id;
        Name = name;
        Value = value;
        CategoryId = categoryId;
        DataOperacao = dataOperacao;
    }
}
