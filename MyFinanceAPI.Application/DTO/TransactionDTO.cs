using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class TransactionDTO
{
    public int Id { get; set; }
    
    [DisplayName("Name")]
    public string Name { get; set; }

    [DisplayName("Value")]
    public decimal Value { get; set; }

    [DisplayName("DueDate")]
    public DateTime DueDate { get; set; }
    
    [JsonIgnore]
    public Category? Category { get; set; }
    
    [DisplayName("Categories")]
    public int Categoryid { get; set; }

}
