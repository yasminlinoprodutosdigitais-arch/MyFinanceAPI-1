using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;

public class Transacao : BaseEntity
{
    /// <summary>Vencimento: data de calendário, sem fuso (coluna date — DB-P1-02).</summary>
    public DateOnly Data { get; set; }
    public string Nome { get; set; }
    public decimal? Valor { get; set; }
    public int? ContaId { get; set; }  // Nome correto da coluna no banco de dados
    [NotMapped]
    public string? ContaNome { get; set; }  // Nome correto da coluna no banco de dados
    public int? CategoriaId { get; set; }  // Nome correto da coluna no banco de dados
    [NotMapped]
    public string? CategoriaNome { get; set; }  // Nome correto da coluna no banco de dados
    [NotMapped]
    public string? SubcategoriaNome { get; set; }  // Nome correto da coluna no banco de dados
    public string Situacao { get; set; }
    public bool EhParcelado { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public string? Observacao { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }  // Relacionamento com Category
    public Conta? Conta { get; set; }

    public Transacao()
    {
    }
}
