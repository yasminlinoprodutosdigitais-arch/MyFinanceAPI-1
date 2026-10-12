using Microsoft.Extensions.Configuration;
using MyFinanceAPI.Ioc;

namespace MyFinanceAPI.Tests.Ioc;

/// <summary>Origens do CORS vindas de Cors:AllowedOrigins (env Cors__AllowedOrigins) — BE-P1-10.</summary>
public class CorsOriginsTests
{
    private static IConfiguration Config(string? origens) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Cors:AllowedOrigins"] = origens })
            .Build();

    [Fact]
    public void Le_a_lista_separada_por_virgula_ignorando_espacos_e_vazios()
    {
        var origens = CorsOrigins.Ler(Config(" http://localhost:4200 , https://yasminlino.github.io ,"), ambienteLocal: false);

        Assert.Equal(new[] { "http://localhost:4200", "https://yasminlino.github.io" }, origens);
    }

    /// <summary>A origem é só esquema + host (+ porta): a URL do GitHub Pages com caminho também serve.</summary>
    [Theory]
    [InlineData("https://yasminlino.github.io/my-finance-front/", "https://yasminlino.github.io")]
    [InlineData("https://yasminlino.github.io/", "https://yasminlino.github.io")]
    [InlineData("http://localhost:4200/", "http://localhost:4200")]
    public void Reduz_a_url_para_a_origem(string configurada, string esperada)
    {
        Assert.Equal(new[] { esperada }, CorsOrigins.Ler(Config(configurada), ambienteLocal: false));
    }

    [Fact]
    public void Repetidas_depois_de_reduzidas_entram_uma_vez()
    {
        var origens = CorsOrigins.Ler(
            Config("https://yasminlino.github.io/my-finance-front/,https://yasminlino.github.io"), ambienteLocal: false);

        Assert.Equal(new[] { "https://yasminlino.github.io" }, origens);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" , ")]
    public void Sem_configuracao_fora_do_ambiente_local_impede_a_api_de_subir(string? origens)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => CorsOrigins.Ler(Config(origens), ambienteLocal: false));

        Assert.Contains("Cors__AllowedOrigins", ex.Message);
    }

    [Fact]
    public void Sem_configuracao_no_ambiente_local_libera_o_ng_serve()
    {
        Assert.Equal(new[] { "http://localhost:4200" }, CorsOrigins.Ler(Config(null), ambienteLocal: true));
    }

    [Fact]
    public void Configuracao_vale_tambem_no_ambiente_local()
    {
        Assert.Equal(new[] { "http://localhost:5173" }, CorsOrigins.Ler(Config("http://localhost:5173"), ambienteLocal: true));
    }

    [Theory]
    [InlineData("yasminlino.github.io")]
    [InlineData("ftp://servidor")]
    public void Origem_invalida_impede_a_api_de_subir(string origem)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => CorsOrigins.Ler(Config(origem), ambienteLocal: false));

        Assert.Contains(origem, ex.Message);
    }
}
