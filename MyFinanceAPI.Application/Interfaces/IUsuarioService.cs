using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> BuscarUsuario(string login, string senha);
    UsuarioDto CadastrarUsuario(CadastrarUsuarioDto request);
    bool VerificaSeUsuarioExiste(string login, string nomeUsuario);
}
