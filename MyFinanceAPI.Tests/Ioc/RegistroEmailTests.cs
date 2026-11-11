using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinanceAPI.Application.Email;
using MyFinanceAPI.Ioc;

namespace MyFinanceAPI.Tests.Ioc;

/// <summary>Card #50: o provedor de e-mail sai da seção "Email"; Brevo incompleto não deixa a API subir.</summary>
public class RegistroEmailTests
{
    private static ServiceProvider Registrar(Dictionary<string, string?> valores)
    {
        var configuracao = new ConfigurationBuilder().AddInMemoryCollection(valores).Build();
        var services = new ServiceCollection().AddLogging();
        DependencyInjection.RegistrarEmail(services, configuracao);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Sem_configuracao_grava_em_arquivo()
    {
        using var provider = Registrar(new());

        Assert.IsType<EnvioEmailArquivo>(provider.GetRequiredService<IEnvioEmail>());
    }

    [Fact]
    public void Brevo_completo_usa_o_Brevo()
    {
        using var provider = Registrar(new()
        {
            ["Email:Provedor"] = "Brevo",
            ["Email:Remetente"] = "remetente@exemplo.com",
            ["Email:BrevoApiKey"] = "chave",
            ["Email:UrlFront"] = "https://app/#",
        });

        Assert.IsType<EnvioEmailBrevo>(provider.GetRequiredService<IEnvioEmail>());
    }

    [Fact]
    public void Brevo_sem_chave_nao_sobe_e_diz_o_que_falta()
    {
        var erro = Assert.Throws<InvalidOperationException>(() => Registrar(new()
        {
            ["Email:Provedor"] = "Brevo",
            ["Email:Remetente"] = "remetente@exemplo.com",
            ["Email:UrlFront"] = "https://app/#",
        }));

        Assert.Contains("Email__BrevoApiKey", erro.Message);
        Assert.DoesNotContain("Email__Remetente", erro.Message);
    }
}
