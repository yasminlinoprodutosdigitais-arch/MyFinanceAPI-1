using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;

public class Category
{ 
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    
    public Category(string name)
    {
        Name = name;
    }

    public Category(ObjectId id, string name)
    {
        Id = id;
        Name = name;
    }
}
