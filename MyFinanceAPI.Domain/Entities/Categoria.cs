using System;

namespace MyFinanceAPI.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; set; } = null!;
    public string Subcategoria { get; set; } = null!;
    public int NaturezaOperacao { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataAlteracao { get; set;}

    public Categoria() {}
}
