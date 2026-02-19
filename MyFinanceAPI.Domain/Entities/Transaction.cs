using System;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;

public class Transaction : BaseEntity
{
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public decimal? Value { get; set; }
    public int? IdAccount { get; set; }  // Nome correto da coluna no banco de dados
    public int? CategoryId { get; set; }  // Nome correto da coluna no banco de dados
    public string Status { get; set; }

    public Account? Account { get; set; }

    public Transaction()
    {

    }

    public Transaction(DateTime date, string name, decimal value, int? idAccount, int? categoryId, string status)
    {
        Date = date;
        Name = name;
        Value = value;
        IdAccount = idAccount;
        CategoryId = categoryId;
        Status = status;
    }

    public Transaction(int id, DateTime date, string name, int idAccount, int? categoryId, decimal value, string status)
    {
        Id = id;
        Date = date;
        Name = name;
        IdAccount = idAccount;
        CategoryId = categoryId;
        Value = value;
        Status = status;
    }
}
