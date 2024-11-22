using System;
using MongoDB.Bson;

namespace MyFinanceAPI.Application.DTO;

public class CategoryDTO
{
    public ObjectId Id {get; set;}
    public string Name { get; set; }

    public CategoryDTO() { }

    public CategoryDTO(string name)
    {
        Name = name;
    }

    public CategoryDTO(ObjectId id, string name)
    {
        Id = id;
        Name = name;
    }
}