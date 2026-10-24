using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class Conta : BaseEntity
{
    public string Nome { get; set; } = null!;
    public decimal Valor { get; set; }
    public int Ativo { get; set; }
    public bool EhParcelado { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public int? ParcelaAtual { get; set; }

    public int CategoriaId { get; set; }
    [NotMapped]
    public string? CategoriaNome { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }  // Relacionamento com Category
    public ICollection<ContaVencimento>? Vencimentos { get; set; }   
    public ICollection<Transacao>? Transacoes { get; set; } // Relacionamento com MonthlyUpdate

    public Conta() { } // Construtor padrão
}
