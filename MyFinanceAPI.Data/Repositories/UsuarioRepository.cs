using MyFinanceAPI.Domain.Interfaces;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyFinanceAPI.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ContextDB _context;

        public UsuarioRepository(ContextDB context)
        {
            _context = context;
        }

        public IEnumerable<Usuario> BuscarTodos()
        {
            return _context.Usuarios.ToList();
        }

        public async Task<Usuario> BuscarUsuario(string login, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == login);

            return usuario;
        }

        public void Cadastrar(Usuario entidade)
        {
            _context.Usuarios.Add(entidade);
            _context.SaveChanges();
        }

        public bool ValidaSeUsuarioExiste(string login, string usuarioNome)
        {
            return _context.Usuarios
                .Any(u => u.UserName == login || u.NomeUsuario == usuarioNome);
        }
    }
}
