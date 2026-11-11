using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyFinanceAPI.Application.Email;

/// <summary>
/// Desenvolvimento (card #50): o e-mail não sai da máquina. Vira um .html na pasta configurada, para abrir
/// no navegador e clicar no link, e o link também vai para o log.
/// </summary>
public partial class EnvioEmailArquivo(IOptions<EmailSettings> configuracao, ILogger<EnvioEmailArquivo> log) : IEnvioEmail
{
    public async Task EnviarAsync(MensagemEmail mensagem)
    {
        var pasta = string.IsNullOrWhiteSpace(configuracao.Value.PastaArquivos)
            ? Path.Combine(Path.GetTempPath(), "emails-dev")
            : configuracao.Value.PastaArquivos;
        Directory.CreateDirectory(pasta);

        var nome = $"{DateTime.Now:yyyyMMdd-HHmmss-fff}-{NaoLetra().Replace(mensagem.Assunto.ToLowerInvariant(), "-").Trim('-')}.html";
        var caminho = Path.Combine(pasta, nome);
        await File.WriteAllTextAsync(caminho,
            $"<p style=\"font:13px sans-serif;color:#666\">Para: {WebUtility.HtmlEncode(mensagem.Para)}<br>" +
            $"Assunto: {WebUtility.HtmlEncode(mensagem.Assunto)}</p><hr>{mensagem.Html}");

        log.LogInformation("E-mail (arquivo) \"{Assunto}\" gravado em {Caminho}. Link: {Link}", mensagem.Assunto, caminho, mensagem.Link);
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NaoLetra();
}
