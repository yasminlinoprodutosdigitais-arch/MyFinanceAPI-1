using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MyFinanceAPI.Api.Controllers;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// Rotas dos controllers (BE-P3-03 e BE-P3-04): URLs absolutas em cada ação, sem [Route] de classe,
/// e todo nome de rota usado no 201 existe num GET — um nome errado dava 500 ao criar.
/// </summary>
public class RotasTests
{
    private static readonly Type[] Controllers = typeof(ApiControllerBase).Assembly.GetTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(ControllerBase).IsAssignableFrom(t))
        .ToArray();

    public static TheoryData<Type> TodosOsControllers()
    {
        var dados = new TheoryData<Type>();
        foreach (var controller in Controllers)
            dados.Add(controller);
        return dados;
    }

    private static IEnumerable<MethodInfo> Acoes(Type controller) =>
        controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    /// <summary>Nomes das rotas GET de um controller.</summary>
    public static HashSet<string> NomesDeGet(Type controller) =>
        Acoes(controller)
            .SelectMany(m => m.GetCustomAttributes<HttpGetAttribute>())
            .Select(a => a.Name)
            .OfType<string>()
            .ToHashSet();

    [Fact]
    public void Encontra_os_controllers_da_api()
    {
        Assert.Equal(13, Controllers.Length);
    }

    [Theory]
    [MemberData(nameof(TodosOsControllers))]
    public void Nenhum_controller_tem_Route_de_classe(Type controller)
    {
        Assert.Null(controller.GetCustomAttribute<RouteAttribute>(inherit: true));
    }

    [Theory]
    [MemberData(nameof(TodosOsControllers))]
    public void Toda_acao_tem_rota_absoluta(Type controller)
    {
        foreach (var acao in Acoes(controller))
        {
            var rotas = acao.GetCustomAttributes<HttpMethodAttribute>().ToList();
            Assert.True(rotas.Count > 0, $"{controller.Name}.{acao.Name} não tem verbo HTTP.");
            Assert.All(rotas, r => Assert.StartsWith("/", r.Template));
        }
    }

    [Theory]
    [MemberData(nameof(TodosOsControllers))]
    public void Constante_de_rota_aponta_para_um_GET_do_proprio_controller(Type controller)
    {
        var constantes = controller.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && f.Name.StartsWith("Rota"))
            .Select(f => (string)f.GetRawConstantValue()!);

        var nomes = NomesDeGet(controller);
        Assert.All(constantes, c => Assert.Contains(c, nomes));
    }

    [Fact]
    public void Nomes_de_rota_sao_unicos_na_api()
    {
        var nomes = Controllers.SelectMany(NomesDeGet).ToList();

        Assert.Equal(nomes.Count, nomes.Distinct().Count());
    }
}
