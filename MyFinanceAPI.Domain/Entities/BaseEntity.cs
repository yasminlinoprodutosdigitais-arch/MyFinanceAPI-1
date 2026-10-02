using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    // Navegação opcional: nullable para que DTOs que carregam entidades não exijam "User" no model binding.
    public Usuario? User { get; set; }
}
