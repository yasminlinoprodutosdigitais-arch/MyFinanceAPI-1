using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Api.Autorizacao;
using MyFinanceAPI.Api.Controllers;
using MyFinanceAPI.Api.Controllers.Authentication;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// Card #50: quem se cadastra pela tela é "Pessoa" e usa o app como Admin e Empresa (política Usuario);
/// criar usuário pelo /CadastrarUsuario fica só com o Admin (antes Empresa também podia).
/// </summary>
public class PerfisTests
{
    private static readonly Type[] Controllers = typeof(ApiControllerBase).Assembly.GetTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(ControllerBase).IsAssignableFrom(t))
        .ToArray();

    public static TheoryData<Type> ControllersDoApp()
    {
        var dados = new TheoryData<Type>();
        // Login e acesso sem conta (cadastro, recuperar senha) são as únicas rotas sem a política.
        foreach (var controller in Controllers.Where(c => c != typeof(AuthenticationController) && c != typeof(AcessoController)))
            dados.Add(controller);
        return dados;
    }

    private static string[] PerfisDa(string politica)
    {
        var opcoes = new AuthorizationOptions();
        Politicas.Configurar(opcoes);
        return opcoes.GetPolicy(politica)!.Requirements.OfType<RolesAuthorizationRequirement>()
            .Single().AllowedRoles.OrderBy(p => p).ToArray();
    }

    [Fact]
    public void Politica_Usuario_aceita_Admin_Empresa_e_Pessoa()
    {
        Assert.Equal(new[] { Perfis.Admin, Perfis.Empresa, Perfis.Pessoa }, PerfisDa(Politicas.Usuario));
    }

    [Fact]
    public void Politica_Admin_aceita_so_Admin()
    {
        Assert.Equal(new[] { Perfis.Admin }, PerfisDa(Politicas.Admin));
    }

    [Theory]
    [MemberData(nameof(ControllersDoApp))]
    public void Controllers_do_app_usam_a_politica_Usuario(Type controller)
    {
        Assert.Equal(Politicas.Usuario, controller.GetCustomAttribute<AuthorizeAttribute>()?.Policy);
    }

    [Fact]
    public void CadastrarUsuario_exige_Admin()
    {
        var acao = typeof(AuthenticationController).GetMethod(nameof(AuthenticationController.Cadastrar))!;

        Assert.Equal(Politicas.Admin, acao.GetCustomAttribute<AuthorizeAttribute>()?.Policy);
    }
}
