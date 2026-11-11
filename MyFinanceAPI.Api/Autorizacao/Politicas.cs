using Microsoft.AspNetCore.Authorization;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Autorizacao;

/// <summary>
/// Políticas de acesso (card #50). <see cref="Usuario"/> vale para o app inteiro: qualquer perfil logado,
/// cada um vendo só os próprios dados (RepositorioDoUsuario). <see cref="Admin"/> é só para administrar
/// o sistema (criar usuário pelo /CadastrarUsuario).
/// </summary>
public static class Politicas
{
    public const string Usuario = "Usuario";
    public const string Admin = "Admin";

    public static void Configurar(AuthorizationOptions opcoes)
    {
        opcoes.AddPolicy(Usuario, p => p.RequireRole(Perfis.Admin, Perfis.Empresa, Perfis.Pessoa));
        opcoes.AddPolicy(Admin, p => p.RequireRole(Perfis.Admin));
    }
}
