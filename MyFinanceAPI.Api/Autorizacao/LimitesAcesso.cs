using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using MyFinanceAPI.Api.Erros;

namespace MyFinanceAPI.Api.Autorizacao;

/// <summary>
/// Limite de pedidos nas rotas sem login (card #50): login, cadastro e recuperação de senha. Por IP
/// (o Cloud Run informa o original em X-Forwarded-For), janela de 1 minuto; passou, 429 com { message }.
/// </summary>
public static class LimitesAcesso
{
    public const string Politica = "acesso";
    public const int PedidosPorMinuto = 10;
    public const string Mensagem = "Muitas tentativas. Aguarde um minuto e tente de novo.";

    public static void Configurar(RateLimiterOptions opcoes)
    {
        opcoes.AddPolicy(Politica, contexto => RateLimitPartition.GetFixedWindowLimiter(
            contexto.Connection.RemoteIpAddress?.ToString() ?? "sem-ip",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = PedidosPorMinuto,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));

        opcoes.OnRejected = async (contexto, cancelamento) =>
        {
            contexto.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await contexto.HttpContext.Response.WriteAsJsonAsync(new MensagemErro(Mensagem), cancelamento);
        };
    }
}
