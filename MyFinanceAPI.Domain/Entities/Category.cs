using System;
using MongoDB.Bson;

namespace MyFinanceAPI.Domain.Entities;

public class Category : BaseEntity
{ 
    public string Name { get; set; }

    public Category(string name)
    {
        Name = name;
    }

    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
