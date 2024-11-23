using System;
using MongoDB.Bson;

namespace MyFinanceAPI.Application.DTO;

public class CategoryDTO
{
    public int Id {get; set;}
    public string Name { get; set; }

    public CategoryDTO() { }

    public CategoryDTO(string name)
    {
        Name = name;
    }

    public CategoryDTO(int id, string name)
    {
        Id = id;
        Name = name;
    }
}