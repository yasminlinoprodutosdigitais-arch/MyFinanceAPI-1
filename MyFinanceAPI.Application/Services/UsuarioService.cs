using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

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

        /// <summary>Busca pelo login e confere a senha contra o hash; null se um dos dois não bate.</summary>
        public async Task<Usuario?> BuscarUsuario(string login, string senha)
        {
            var usuario = await _usuarioRepository.BuscarUsuario(login);
            if (usuario == null)
                return null;

            var resultadoSenha = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, senha);
            return resultadoSenha == PasswordVerificationResult.Success ? usuario : null;
        }

        /// <summary>
        /// Grava o usuário (senha como hash) e devolve o criado. Sem catch (BE-P3-10): uma falha
        /// vai ao ErroHandler, que registra o log — antes virava null e um 422 sem rastro.
        /// </summary>
        public Usuario CadastrarUsuario(CadastrarUsuarioDto request)
        {
            var usuario = new Usuario(
                request.Login,   // Login é atribuído ao UserName
                request.Senha,
                request.NomeUsuario,
                request.Role,
                true,
                DateTime.UtcNow,
                DateTime.UtcNow
            );

            _usuarioRepository.Cadastrar(usuario);
            return usuario;
        }

        public bool VerificaSeUsuarioExiste(string login, string nomeUsuario)
        {
            // Verifica se o usuário já existe no repositório
            return _usuarioRepository.ValidaSeUsuarioExiste(login, nomeUsuario);
        }
    }
}
