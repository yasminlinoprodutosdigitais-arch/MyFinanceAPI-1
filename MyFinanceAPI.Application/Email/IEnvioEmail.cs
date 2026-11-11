namespace MyFinanceAPI.Application.Email;

/// <summary>
/// Um e-mail pronto para sair. <see cref="Link"/> é só para o log do envio em arquivo (desenvolvimento):
/// o link já vai dentro do corpo.
/// </summary>
public sealed record MensagemEmail(string Para, string NomePara, string Assunto, string Html, string Texto, string? Link = null);

/// <summary>
/// Envio de e-mail (card #50). Em desenvolvimento, <see cref="EnvioEmailArquivo"/>; em produção,
/// <see cref="EnvioEmailBrevo"/>. A escolha é pela configuração Email:Provedor.
/// </summary>
public interface IEnvioEmail
{
    Task EnviarAsync(MensagemEmail mensagem);
}
