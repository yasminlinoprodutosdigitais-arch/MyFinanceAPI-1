using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MyFinanceAPI.Application.Email;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #50: em desenvolvimento o e-mail vira um .html numa pasta local; em produção vai pelo Brevo
/// (remetente e chave vêm da configuração, nunca do código).
/// </summary>
public sealed class EnvioEmailTests : IDisposable
{
    private readonly string _pasta = Path.Combine(Path.GetTempPath(), "envio-email-testes-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_pasta))
            Directory.Delete(_pasta, recursive: true);
    }

    private static readonly MensagemEmail Mensagem = new("ana@exemplo.com", "Ana", "Confirme seu e-mail", "<p>Olá</p>", "Olá", "https://app/#/x?token=abc");

    private static IOptions<EmailSettings> Config(string pasta = "") => Options.Create(new EmailSettings
    {
        Remetente = "remetente@exemplo.com",
        NomeRemetente = "Kanfy",
        PastaArquivos = pasta,
        BrevoApiKey = "chave-teste",
    });

    // ---------- Arquivo ----------

    [Fact]
    public async Task Arquivo_grava_um_html_com_destinatario_assunto_e_corpo()
    {
        await new EnvioEmailArquivo(Config(_pasta), NullLogger<EnvioEmailArquivo>.Instance).EnviarAsync(Mensagem);

        var arquivo = Assert.Single(Directory.GetFiles(_pasta, "*.html"));
        var conteudo = await File.ReadAllTextAsync(arquivo);
        Assert.Contains("ana@exemplo.com", conteudo);
        Assert.Contains("Confirme seu e-mail", conteudo);
        Assert.Contains("<p>Olá</p>", conteudo);
    }

    // ---------- Brevo ----------

    private sealed class HttpFalso(HttpStatusCode status) : HttpMessageHandler
    {
        public HttpRequestMessage? Pedido { get; private set; }
        public string? Corpo { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Pedido = request;
            Corpo = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(status) { Content = new StringContent("{}") };
        }
    }

    [Fact]
    public async Task Brevo_monta_o_pedido_com_chave_remetente_e_destinatario()
    {
        var http = new HttpFalso(HttpStatusCode.Created);

        await new EnvioEmailBrevo(new HttpClient(http), Config(), NullLogger<EnvioEmailBrevo>.Instance).EnviarAsync(Mensagem);

        Assert.Equal(HttpMethod.Post, http.Pedido!.Method);
        Assert.Equal("https://api.brevo.com/v3/smtp/email", http.Pedido.RequestUri!.ToString());
        Assert.Equal("chave-teste", http.Pedido.Headers.GetValues("api-key").Single());
        using var json = JsonDocument.Parse(http.Corpo!);
        var raiz = json.RootElement;
        Assert.Equal("remetente@exemplo.com", raiz.GetProperty("sender").GetProperty("email").GetString());
        Assert.Equal("Kanfy", raiz.GetProperty("sender").GetProperty("name").GetString());
        Assert.Equal("ana@exemplo.com", raiz.GetProperty("to")[0].GetProperty("email").GetString());
        Assert.Equal("Confirme seu e-mail", raiz.GetProperty("subject").GetString());
        Assert.Equal("<p>Olá</p>", raiz.GetProperty("htmlContent").GetString());
        Assert.Equal("Olá", raiz.GetProperty("textContent").GetString());
    }

    [Fact]
    public async Task Brevo_recusando_vira_erro()
    {
        var envio = new EnvioEmailBrevo(new HttpClient(new HttpFalso(HttpStatusCode.Unauthorized)), Config(), NullLogger<EnvioEmailBrevo>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => envio.EnviarAsync(Mensagem));
    }

    // ---------- Modelos ----------

    [Fact]
    public void Modelos_levam_o_link_e_escapam_o_nome()
    {
        var confirmar = ModelosEmail.ConfirmarEmail("ana@exemplo.com", "<Ana>", "https://app/#/login/confirmar-email?token=abc");
        var redefinir = ModelosEmail.RedefinirSenha("ana@exemplo.com", "Ana", "https://app/#/login/redefinir-senha?token=xyz");
        var jaExiste = ModelosEmail.ContaJaExiste("ana@exemplo.com", "Ana", "https://app/#/login", "https://app/#/login/recuperar-senha");

        Assert.Contains("https://app/#/login/confirmar-email?token=abc", confirmar.Html);
        Assert.Contains("https://app/#/login/confirmar-email?token=abc", confirmar.Texto);
        Assert.Contains("&lt;Ana&gt;", confirmar.Html);
        Assert.DoesNotContain("<Ana>", confirmar.Html);
        Assert.Contains("30 minutos", redefinir.Html);
        Assert.Contains("https://app/#/login/recuperar-senha", jaExiste.Html);
        Assert.All(new[] { confirmar, redefinir, jaExiste }, m => Assert.Equal("ana@exemplo.com", m.Para));
    }
}
