using System;

namespace MyFinanceAPI.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; set; } = null!;
    public string Subcategoria { get; set; } = null!;
    public int NaturezaOperacao { get; set; }
    public bool Ativo { get; set; }
    /// <summary>Classe do PrimeIcons ("pi pi-car"); nulo = o front escolhe pelo nome.</summary>
    public string? Icone { get; set; }
    /// <summary>Chave da paleta do front ("azul", "verde-agua"…); nulo = a cor da natureza.</summary>
    public string? Cor { get; set; }
    public DateTime DataAlteracao { get; set;}

    public Categoria() {}
}
