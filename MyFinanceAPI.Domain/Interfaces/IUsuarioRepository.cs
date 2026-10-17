using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IUsuarioRepository
{
    void Cadastrar(Usuario entidade);
    /// <summary>Usuário pelo login (UserName), ou null. Não confere senha.</summary>
    public Task<Usuario?> BuscarUsuario(string login);
    public bool ValidaSeUsuarioExiste(string login, string usuarioNome);
}
