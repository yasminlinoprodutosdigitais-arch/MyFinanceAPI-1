using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyFinanceAPI.Application.Email;

/// <summary>
/// Produção (card #50): API transacional do Brevo. O remetente precisa estar verificado na conta do Brevo.
/// Recusa vira erro (o ErroHandler registra e devolve 500); o log não leva o destinatário.
/// </summary>
public class EnvioEmailBrevo(HttpClient http, IOptions<EmailSettings> configuracao, ILogger<EnvioEmailBrevo> log) : IEnvioEmail
{
    private const string Endereco = "https://api.brevo.com/v3/smtp/email";

    public async Task EnviarAsync(MensagemEmail mensagem)
    {
        var config = configuracao.Value;
        using var pedido = new HttpRequestMessage(HttpMethod.Post, Endereco)
        {
            Content = JsonContent.Create(new
            {
                sender = new { name = config.NomeRemetente, email = config.Remetente },
                to = new[] { new { email = mensagem.Para, name = mensagem.NomePara } },
                subject = mensagem.Assunto,
                htmlContent = mensagem.Html,
                textContent = mensagem.Texto,
            }),
        };
        pedido.Headers.Add("api-key", config.BrevoApiKey);

        using var resposta = await http.SendAsync(pedido);
        if (!resposta.IsSuccessStatusCode)
        {
            log.LogError("Brevo recusou o e-mail \"{Assunto}\": {Status}", mensagem.Assunto, (int)resposta.StatusCode);
            throw new InvalidOperationException($"O envio de e-mail falhou ({(int)resposta.StatusCode}).");
        }
    }
}
