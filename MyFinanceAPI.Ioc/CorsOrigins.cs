using Microsoft.Extensions.Configuration;

namespace MyFinanceAPI.Ioc;

/// <summary>
/// Origens liberadas no CORS (BE-P1-10), lidas de Cors:AllowedOrigins — em produção a variável
/// Cors__AllowedOrigins, separada por vírgula.
/// </summary>
public static class CorsOrigins
{
    public const string Chave = "Cors:AllowedOrigins";
    private const string PadraoLocal = "http://localhost:4200"; // ng serve

    public static string[] Ler(IConfiguration configuration, bool ambienteLocal)
    {
        var configuradas = (configuration[Chave] ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (configuradas.Length == 0)
        {
            if (ambienteLocal)
                return new[] { PadraoLocal };

            throw new InvalidOperationException(
                "Cors:AllowedOrigins não configurada. Defina a variável de ambiente Cors__AllowedOrigins " +
                "com as origens do front separadas por vírgula (ex.: https://yasminlino.github.io).");
        }

        return configuradas.Select(Origem).Distinct().ToArray();
    }

    // O navegador manda só esquema + host (+ porta) no Origin; a URL do GitHub Pages traz um caminho.
    private static string Origem(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new InvalidOperationException($"Origem inválida em Cors__AllowedOrigins: '{url}'. Use http(s)://host.");

        return uri.GetLeftPart(UriPartial.Authority);
    }
}
