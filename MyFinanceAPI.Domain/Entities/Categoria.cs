using System;

namespace MyFinanceAPI.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; set; }
    public string Subcategoria { get; set; }
    public int NaturezaOperacao { get; set; }
    public int Ativo { get; set; }
    public DateTime DataAlteracao { get; set;}

    public Categoria() {}
}
