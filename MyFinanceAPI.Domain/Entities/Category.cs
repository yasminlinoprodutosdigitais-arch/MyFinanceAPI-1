using System;
using MongoDB.Bson;

namespace MyFinanceAPI.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string SubCategory { get; set; }
    public int NaturezaOperacao { get; set; }
    public int Status { get; set; }
    public DateTime DataAlteracao { get; set;}

    public Category() {}
    public Category(string name, string subCategory, int naturezaOperacao, int status, DateTime dataAlteracao)
    {
        Name = name;
        SubCategory = subCategory;
        NaturezaOperacao = naturezaOperacao;
        Status = status;
        DataAlteracao = dataAlteracao;
    }

    public Category(int id, string name, string subCategory, int naturezaOperacao, int status, DateTime dataAlteracao)
    {
        Id = id;
        Name = name;
        SubCategory = subCategory;
        NaturezaOperacao = naturezaOperacao;
        Status = status;
        DataAlteracao = dataAlteracao;
    }
}
