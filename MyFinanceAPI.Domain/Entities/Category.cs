using System;
using MongoDB.Bson;

namespace MyFinanceAPI.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string SubCategory { get; set; }

    public Category() {}
    public Category(string name, string subCategory)
    {
        Name = name;
        SubCategory = subCategory;
    }

    public Category(int id, string name, string subCategory)
    {
        Id = id;
        Name = name;
        SubCategory = subCategory;
    }
}
