using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using MyFinanceAPI.Api.Autorizacao;
using MyFinanceAPI.Api.Controllers;
using MyFinanceAPI.Api.Controllers.Authentication;

namespace MyFinanceAPI.Tests.Api;

/// <summary>Card #50: login, cadastro e recuperação de senha com limite por IP; o excesso é 429 com { message }.</summary>
public class LimitesAcessoTests
{
    [Fact]
    public void Rotas_sem_login_tem_limite()
    {
        Assert.Equal(LimitesAcesso.Politica, typeof(AcessoController).GetCustomAttribute<EnableRateLimitingAttribute>()?.PolicyName);
        var autenticar = typeof(AuthenticationController).GetMethod(nameof(AuthenticationController.Autenticar))!;
        Assert.Equal(LimitesAcesso.Politica, autenticar.GetCustomAttribute<EnableRateLimitingAttribute>()?.PolicyName);
    }

    [Fact]
    public async Task Excesso_responde_429_com_a_mensagem()
    {
        var opcoes = new RateLimiterOptions();
        LimitesAcesso.Configurar(opcoes);
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();

        await opcoes.OnRejected!(new OnRejectedContext { HttpContext = contexto, Lease = null! }, CancellationToken.None);

        Assert.Equal(StatusCodes.Status429TooManyRequests, contexto.Response.StatusCode);
        contexto.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(contexto.Response.Body);
        Assert.Equal(LimitesAcesso.Mensagem, json.RootElement.GetProperty("message").GetString());
    }
}
