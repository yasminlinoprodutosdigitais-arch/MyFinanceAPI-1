using System;
using System.Text.Json.Serialization;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SubCategory { get; set; }
    public int NaturezaOperacao { get; set; }
    public int Status { get; set; }
    public DateTime DataAlteracao { get; set;}
    
    [JsonIgnore]
    public ICollection<Account>? Accounts { get; set; }

    public CategoryDTO() { }
}