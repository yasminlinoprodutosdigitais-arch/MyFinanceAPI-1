using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;

public class Transaction
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public decimal Value { get; set; }
    public DateTime DueDate { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }

    public Transaction(string name, decimal value, DateTime dueDate, Category category)
    {
        Name = name;
        Value = value;
        DueDate = dueDate;
        Category = category;
    }
    public Transaction(ObjectId id, string name, decimal value, DateTime dueDate, Category category)
    {
        Id = id;
        Name = name;
        Value = value;
        DueDate = dueDate;
        Category = category;
    }
}
