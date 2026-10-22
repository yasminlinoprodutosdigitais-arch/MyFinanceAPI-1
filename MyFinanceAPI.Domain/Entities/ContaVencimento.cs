using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class ContaVencimento : BaseEntity
{
    public int ContaId { get; set; }
    public int Dia { get; set; }

    [JsonIgnore]
    public Conta? Conta { get; set; }  // Relacionamento com Category
    public ContaVencimento() { } // Construtor padrão
}
