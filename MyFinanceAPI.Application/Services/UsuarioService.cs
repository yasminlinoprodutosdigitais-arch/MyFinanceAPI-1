using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using MyFinanceAPI.Application.Utils;
using Microsoft.Extensions.Options;

namespace MyFinanceAPI.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<UsuarioDto> BuscarUsuario(string login, string senha)
        {
            // Busca o usuário de forma assíncrona
            Usuario usuario = await _usuarioRepository.BuscarUsuario(login, senha);

            // Se o usuário não for encontrado, retorna null
            if (usuario == null)
                return null;

            // Compara a senha fornecida com o hash armazenado
            var resultadoSenha = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, senha);

            // Se a senha não for válida, retorna null
            if (resultadoSenha != PasswordVerificationResult.Success)
                return null;

            // Caso o usuário exista e a senha seja válida, retorna um DTO com o UserName (login) e a senha
            return new UsuarioDto(usuario.UserName, senha);
        }



        public UsuarioDto CadastrarUsuario(CadastrarUsuarioDto request)
        {
            try
            {
                // Obtem o último código de ID cadastrado, agora usando a propriedade Id
                int ultimoCodigoIdCadastrado = _usuarioRepository.BuscarTodos().Any() ? _usuarioRepository.BuscarTodos().Max(x => x.Id) : 0;

                // Criação de um novo usuário
                Usuario usuario = new Usuario(
                    request.Login,   // Login é atribuído ao UserName
                    request.Senha,
                    request.NomeUsuario,
                    request.Role,
                    true,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                );

                // Cadastra o novo usuário no repositório
                _usuarioRepository.Cadastrar(usuario);

                // Retorna o DTO com o UserName (login) e a senha do usuário cadastrado
                return new UsuarioDto(request.Login, request.Senha);
            }
            catch
            {
                // Em caso de erro, retorna null (você pode querer lançar uma exceção aqui para tratar o erro de forma mais adequada)
                return null;
            }
        }

        public bool VerificaSeUsuarioExiste(string login, string nomeUsuario)
        {
            // Verifica se o usuário já existe no repositório
            return _usuarioRepository.ValidaSeUsuarioExiste(login, nomeUsuario);
        }
    }
}
