using System;
using MongoDB.Bson;

namespace MyFinanceAPI.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string SubCategory { get; set; }
    public int NaturezaOperacao { get; set; }
    public int Status { get; set; }

    public Category() {}
    public Category(string name, string subCategory, int naturezaOperacao, int status)
    {
        Name = name;
        SubCategory = subCategory;
        NaturezaOperacao = naturezaOperacao;
        Status = status;
    }

    public Category(int id, string name, string subCategory, int naturezaOperacao, int status)
    {
        Id = id;
        Name = name;
        SubCategory = subCategory;
        NaturezaOperacao = naturezaOperacao;
        Status = status;
    }
}
