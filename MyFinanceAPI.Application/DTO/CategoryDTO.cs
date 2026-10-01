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
    public int NaturezaOperacao { get; set; }
    public int Status { get; set; }
    public DateTime DataAlteracao { get; set;}
    
    [JsonIgnore]
    public ICollection<Account>? Accounts { get; set; }

    public CategoryDTO() { }

    public CategoryDTO(string name, string subCategory, int naturezaOperacao, int status, ICollection<Account> accounts, DateTime dataAlteracao)
    {
        Name = name;
        SubCategory = subCategory;
        NaturezaOperacao = naturezaOperacao;
        Status = status;
        Accounts = accounts; 
        DataAlteracao = dataAlteracao;
    }

    public CategoryDTO(int id, string name, string subCategory, int naturezaOperacao, int status, ICollection<Account> accounts, DateTime dataAlteracao)
    {
        Id = id;
        Name = name;
        SubCategory = subCategory;
        NaturezaOperacao = naturezaOperacao;
        Accounts = accounts; 
        DataAlteracao = dataAlteracao;
    }
}