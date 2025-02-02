using MyFinanceAPI.Domain.Interfaces;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using System.Linq.Expressions;
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

        public void AlterarStatusAtivo(int codigoId, bool statusAtivo)
        {
            throw new NotImplementedException();
        }

        public void Atualizar(Usuario entidade, int codigoId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> BuscarPorExpressao(Expression<Func<Usuario, bool>> filtro)
        {
            throw new NotImplementedException();
        }

        public Usuario BuscarPorId(int codigoId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Usuario> BuscarTodos()
        {
            // Retorna todos os usuários
            return _context.Usuarios.ToList();
        }

        // Implementação dos métodos do repositório, por exemplo:
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

        public void Deletar(int codigoId)
        {
            throw new NotImplementedException();
        }

        public bool ValidaSeUsuarioExiste(string login, string usuarioNome)
        {
            // Verifica se já existe um usuário com o login ou o nome de usuário fornecido
            return _context.Usuarios
                .Any(u => u.UserName == login || u.NomeUsuario == usuarioNome);
        }

        // Outros métodos necessários
    }
}
