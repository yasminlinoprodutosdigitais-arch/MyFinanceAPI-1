using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IUsuarioRepository
{
    IEnumerable<Usuario> BuscarTodos();
    void Cadastrar(Usuario entidade);
    public Task<Usuario> BuscarUsuario(string login, string senha);
    public bool ValidaSeUsuarioExiste(string login, string usuarioNome);
}
