using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITokenService
{
    /// <summary>JWT para um usuário já validado (login e senha conferidos pelo IUsuarioService).</summary>
    TokenDto CreateToken(Usuario usuario);
}
