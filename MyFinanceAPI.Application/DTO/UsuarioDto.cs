using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Application.DTO
{
    public class UsuarioDto
    {
        [Required]
        public string Login { get; private set; }

        [Required]
        public string Senha { get; private set; }

        public UsuarioDto(string login, string senha)
        {
            Login = login;
            Senha = senha;
        }
    }

}
