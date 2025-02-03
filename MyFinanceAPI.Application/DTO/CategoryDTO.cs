using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SubCategory { get; set; }
    
    [JsonIgnore]
    public ICollection<Account>? Accounts { get; set; }

    public CategoryDTO() { }

    public CategoryDTO(string name, string subCategory, ICollection<Account> accounts)
    {
        Name = name;
        SubCategory = subCategory;
        Accounts = accounts; 
    }

    public CategoryDTO(int id, string name, string subCategory, ICollection<Account> accounts)
    {
        Id = id;
        Name = name;
        SubCategory = subCategory;
        Accounts = accounts; 
    }
}