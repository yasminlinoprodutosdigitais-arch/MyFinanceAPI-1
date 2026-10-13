using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [NotMapped]
    public string? CategoryName { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }  // Relacionamento com Category
    public ICollection<ContaVencimento>? ContaVencimentos { get; set; }   
    public ICollection<Transaction>? Transactions { get; set; } // Relacionamento com MonthlyUpdate

    public Account() { } // Construtor padrão
}
