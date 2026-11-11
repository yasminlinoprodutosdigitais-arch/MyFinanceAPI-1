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
            return porUsuario ?? await BuscarPorEmailAsync(login);
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
