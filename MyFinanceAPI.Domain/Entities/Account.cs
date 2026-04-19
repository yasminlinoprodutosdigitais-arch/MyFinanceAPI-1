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
    public int Status { get; set; }
    public bool EhParcelado { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public int? ParcelaAtual { get; set; }


    public int CategoryId { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }  // Relacionamento com Category
    public ICollection<ContaVencimento>? ContaVencimentos { get; set; }   
    public ICollection<Transaction>? Transactions { get; set; } // Relacionamento com MonthlyUpdate

    public Account() { } // Construtor padrão

    public Account(string name, decimal value, int categoryId, int status, bool ehParcelada, int quantidadeParcelas, int parcelaAtual)
    {
        Name = name;
        Value = value;
        CategoryId = categoryId;
        Status = status;
        EhParcelado = ehParcelada;
        QuantidadeParcelas = quantidadeParcelas;
        ParcelaAtual = parcelaAtual;
    }

    public Account(int id, string name, decimal value, int categoryId, int status, bool ehParcelada, int quantidadeParcelas, int parcelaAtual)
    {
        Id = id;
        Name = name;
        Value = value;
        CategoryId = categoryId;
        Status = status;
        EhParcelado = ehParcelada;
        QuantidadeParcelas = quantidadeParcelas;
        ParcelaAtual = parcelaAtual;
    }
}
