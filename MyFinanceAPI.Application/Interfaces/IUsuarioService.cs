using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IUsuarioService
{
    /// <summary>Usuário com login e senha válidos, ou null.</summary>
    Task<Usuario?> BuscarUsuario(string login, string senha);
    /// <summary>Devolve o usuário criado (com o id gerado).</summary>
    Usuario CadastrarUsuario(CadastrarUsuarioDto request);
    bool VerificaSeUsuarioExiste(string login, string nomeUsuario);
}
