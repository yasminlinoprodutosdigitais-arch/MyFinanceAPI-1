using System;

namespace MyFinanceAPI.Domain.Entities;

public class TransactionHistory : BaseEntity
{   
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public int IdCategory { get; set; }
    public string Status { get; set; }

    public TransactionHistory()
    {
        
    }

    public TransactionHistory(DateTime date, string name, double value, int idCategory, string status)
    {
        Date = date;
        Name = name;
        Value = value;
        IdCategory = idCategory;
        Status = status;
    }

    public TransactionHistory(int id, DateTime date, string name, double value, string status)
    {
        Id = id;
        Date = date;
        Name = name;
        Value = value;
        Status = status;
    } 
}
