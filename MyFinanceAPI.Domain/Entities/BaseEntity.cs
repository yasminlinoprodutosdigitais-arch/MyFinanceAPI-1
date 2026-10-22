using System;

namespace MyFinanceAPI.Domain.Entities;

public class BaseEntity : IEntidade
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    // Navegação opcional: nullable para que DTOs que carregam entidades não exijam "User" no model binding.
    public Usuario? Usuario { get; set; }
}
