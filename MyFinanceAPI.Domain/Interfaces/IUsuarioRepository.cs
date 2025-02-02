using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    public Task<Usuario> BuscarUsuario(string login, string senha);
    public bool ValidaSeUsuarioExiste(string login, string usuarioNome);
}
