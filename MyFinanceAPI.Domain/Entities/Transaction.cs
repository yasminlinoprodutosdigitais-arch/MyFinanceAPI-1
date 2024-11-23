using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;

public class Transaction : BaseEntity
{
    public string Name { get; set; }
    public double Value { get; set; }
    // public DateTime DueDate { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }

    public Transaction(string name, double value, DateTime dueDate, Category category, int categoryId)
    {
        Name = name;
        Value = value;
        // DueDate = dueDate;
        Category = category;
        CategoryId = categoryId;
    }
    public Transaction(int id, string name, double value, DateTime dueDate, Category category, int categoryId)
    {
        Id = id;
        Name = name;
        Value = value;
        // DueDate = dueDate;
        Category = category;
        CategoryId = categoryId;
    }
}
