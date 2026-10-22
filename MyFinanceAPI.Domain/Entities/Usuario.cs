using Microsoft.AspNetCore.Identity;
using System;

namespace MyFinanceAPI.Domain.Entities
{
    // Herdando de IdentityUser para integração com o ASP.NET Core Identity
    public class Usuario : IdentityUser<int> // 'int' é o tipo da chave primária
    {
        // Propriedades adicionais para o usuário
        public string NomeUsuario { get; set; }
        public bool Ativo { get; set; }
        public string Perfil { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public Usuario()
        {
        }

        // Construtor da classe Usuario
        public Usuario(string userName,
               string senha,
               string nomeUsuario,
               string perfil,
               bool ativo,
               DateTime dataCriacao,
               DateTime dataAlteracao)
        {
            this.UserName = userName;
            this.Email = userName;
            this.NomeUsuario = nomeUsuario;
            this.Perfil = perfil;
            this.Ativo = ativo;
            this.DataCriacao = dataCriacao;
            this.DataAlteracao = dataAlteracao;

            SetSenhaHash(senha);
        }

        public void SetSenhaHash(string senha)
        {
            var passwordHasher = new PasswordHasher<Usuario>();
            this.PasswordHash = passwordHasher.HashPassword(this, senha);
        }
    }
}
