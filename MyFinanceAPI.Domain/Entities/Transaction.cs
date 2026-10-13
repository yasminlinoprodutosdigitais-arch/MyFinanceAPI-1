using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;

public class Transaction : BaseEntity
{
    /// <summary>Vencimento: data de calendário, sem fuso (coluna date — DB-P1-02).</summary>
    public DateOnly Date { get; set; }
    public string Name { get; set; }
    public decimal? Value { get; set; }
    public int? IdAccount { get; set; }  // Nome correto da coluna no banco de dados
    [NotMapped]
    public string? AccountName { get; set; }  // Nome correto da coluna no banco de dados
    public int? CategoryId { get; set; }  // Nome correto da coluna no banco de dados
    [NotMapped]
    public string? CategoryName { get; set; }  // Nome correto da coluna no banco de dados
    [NotMapped]
    public string? SubCategoryName { get; set; }  // Nome correto da coluna no banco de dados
    public string Status { get; set; }
    public bool EhParcelado { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public string? Observacao { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }  // Relacionamento com Category
    public Account? Account { get; set; }

    public Transaction()
    {
    }
}
