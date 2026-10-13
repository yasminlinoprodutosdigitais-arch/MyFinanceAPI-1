using System;

namespace MyFinanceAPI.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string SubCategory { get; set; }
    public int NaturezaOperacao { get; set; }
    public int Status { get; set; }
    public DateTime DataAlteracao { get; set;}

    public Category() {}
}
