using System;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto> BuscarUsuario(string login, string senha);
    UsuarioDto CadastrarUsuario(CadastrarUsuarioDto request);
    bool VerificaSeUsuarioExiste(string login, string nomeUsuario);
}
