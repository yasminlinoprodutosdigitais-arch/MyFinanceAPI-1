using System;
using System.ComponentModel;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class TransactionDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public int? IdAccount {get; set; }
    public string Status { get; set; }

    public TransactionDTO()
    {
        
    }

    public TransactionDTO(DateTime date, string name, double value,int? idAccount, string status)
    {
        Date = date;
        Name = name;
        Value = value;
        IdAccount = idAccount;
        Status = status;
    }

    public TransactionDTO(int id, DateTime date, string name, int idAccount, double value, string status)
    {
        Id = id;
        Date = date;
        Name = name;
        IdAccount = idAccount;
        Value = value;
        Status = status;
    } 
    
}
