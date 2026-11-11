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

        /// <summary>
        /// Pelo usuário e, se não achar, pelo e-mail sem diferenciar maiúsculas (card #70; o NormalizedEmail
        /// não é preenchido pelo cadastro). A senha é conferida no UsuarioService, contra o hash (BE-P3-10).
        /// </summary>
        public async Task<Usuario?> BuscarUsuario(string login)
        {
            var porUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UserName == login);
            if (porUsuario != null)
                return porUsuario;

            var email = login.ToUpper();
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToUpper() == email);
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
