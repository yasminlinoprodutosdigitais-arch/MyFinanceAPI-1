namespace MyFinanceAPI.Application.Email;

/// <summary>
/// Seção "Email" da configuração (card #50). Remetente e chave do Brevo NÃO ficam no appsettings versionado:
/// dev → dotnet user-secrets; produção → variáveis de ambiente Email__Remetente e Email__BrevoApiKey.
/// </summary>
public class EmailSettings
{
    /// <summary>"Arquivo" (padrão, desenvolvimento) ou "Brevo".</summary>
    public string Provedor { get; set; } = "Arquivo";
    public string Remetente { get; set; } = "";
    public string NomeRemetente { get; set; } = "Kanfy";
    /// <summary>Pasta dos .html do envio em arquivo; vazia = "emails-dev" na pasta temporária.</summary>
    public string PastaArquivos { get; set; } = "";
    public string BrevoApiKey { get; set; } = "";
    /// <summary>Endereço do front, sem barra no fim, para montar os links: "http://localhost:4200" (sem "#" desde o card #40).</summary>
    public string UrlFront { get; set; } = "";
}
