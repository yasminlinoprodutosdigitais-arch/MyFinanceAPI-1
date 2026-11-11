using System.Net;

namespace MyFinanceAPI.Application.Email;

/// <summary>
/// Textos dos e-mails (card #50), em HTML simples com estilo inline (cliente de e-mail ignora CSS externo)
/// e em texto puro. O nome vem do cadastro: sempre escapado.
/// </summary>
public static class ModelosEmail
{
    public static MensagemEmail ConfirmarEmail(string para, string nome, string link) => Montar(
        para, nome, "Confirme seu e-mail no Kanfy", link,
        "Falta só confirmar seu e-mail para começar a usar o Kanfy.",
        "Confirmar e-mail",
        "O link vale por 24 horas. Se você não criou uma conta no Kanfy, ignore este e-mail.");

    public static MensagemEmail RedefinirSenha(string para, string nome, string link) => Montar(
        para, nome, "Crie uma senha nova no Kanfy", link,
        "Recebemos um pedido para criar uma senha nova para a sua conta.",
        "Criar senha nova",
        "O link vale por 30 minutos e só pode ser usado uma vez. Se não foi você, ignore este e-mail: sua senha continua a mesma.");

    /// <summary>Cadastro com um e-mail que já tem conta: avisa o dono, sem revelar nada a quem tentou.</summary>
    public static MensagemEmail ContaJaExiste(string para, string nome, string linkLogin, string linkRecuperar) => Montar(
        para, nome, "Você já tem uma conta no Kanfy", linkLogin,
        "Alguém tentou criar uma conta no Kanfy com este e-mail, mas ele já tem cadastro.",
        "Entrar",
        $"Esqueceu a senha? Crie uma nova em {linkRecuperar}. Se não foi você, ignore este e-mail.",
        linkRecuperar);

    private static MensagemEmail Montar(string para, string nome, string assunto, string link, string texto,
        string botao, string rodape, string? linkRodape = null)
    {
        var saudacao = $"Olá, {nome}!";
        var rodapeHtml = WebUtility.HtmlEncode(rodape);
        if (linkRodape != null)
            rodapeHtml = rodapeHtml.Replace(WebUtility.HtmlEncode(linkRodape), $"<a href=\"{WebUtility.HtmlEncode(linkRodape)}\">{WebUtility.HtmlEncode(linkRodape)}</a>");

        var html = $"""
            <div style="font-family:Arial,sans-serif;max-width:480px;margin:0 auto;color:#1f2937">
              <p style="font-size:20px;font-weight:bold;color:#0b1f4b;margin:0 0 24px">Kanfy</p>
              <p style="font-size:16px">{WebUtility.HtmlEncode(saudacao)}</p>
              <p style="font-size:15px;line-height:1.5">{WebUtility.HtmlEncode(texto)}</p>
              <p style="margin:28px 0">
                <a href="{WebUtility.HtmlEncode(link)}" style="background:#3b82f6;color:#ffffff;padding:12px 24px;border-radius:6px;text-decoration:none;font-weight:bold">{WebUtility.HtmlEncode(botao)}</a>
              </p>
              <p style="font-size:13px;color:#6b7280;line-height:1.5">{rodapeHtml}</p>
              <p style="font-size:12px;color:#9ca3af;word-break:break-all">Se o botão não abrir, copie este endereço no navegador:<br>{WebUtility.HtmlEncode(link)}</p>
            </div>
            """;
        var textoPuro = $"{saudacao}\n\n{texto}\n\n{botao}: {link}\n\n{rodape}";

        return new MensagemEmail(para, nome, assunto, html, textoPuro, link);
    }
}
