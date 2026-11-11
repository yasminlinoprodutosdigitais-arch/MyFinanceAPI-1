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
        /// Pelo usuário e, se não achar, pelo e-mail, os dois sem diferenciar maiúsculas (cards #70 e #84; o
        /// Normalized* não é preenchido nos usuários antigos). A senha é conferida no UsuarioService (BE-P3-10).
        /// </summary>
        public async Task<Usuario?> BuscarUsuario(string login)
        {
            var maiusculo = login.ToUpper();
            var porUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToUpper() == maiusculo);
            return porUsuario ?? await BuscarPorEmailAsync(login);
        }

        public Task<bool> LoginEmUsoAsync(string login)
        {
            var maiusculo = login.ToUpper();
            return _context.Usuarios.AnyAsync(u => u.UserName != null && u.UserName.ToUpper() == maiusculo);
        }

        public Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            var maiusculo = email.ToUpper();
            return _context.Usuarios.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToUpper() == maiusculo);
        }

        public Task<Usuario?> ObterPorIdAsync(int id) => _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

        public async Task AtualizarAsync(Usuario usuario)
        {
            usuario.DataAlteracao = DateTime.UtcNow;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
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
