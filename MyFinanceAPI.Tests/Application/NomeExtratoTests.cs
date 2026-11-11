using MyFinanceAPI.Application.Services;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #79 (10/11/2026): o nome que vem no extrato ("CIRCUITO") e quando dois nomes são o mesmo lugar escrito de jeitos
/// diferentes (base da classificação por semelhança e do "Juntar parecidos").
/// </summary>
public class NomeExtratoTests
{
    [Theory]
    [InlineData("Compra no débito - Circuito", null, "CIRCUITO")]
    [InlineData("Pix", "Padaria São José", "PADARIA SAO JOSE")]
    [InlineData("IFOOD *RESTAURANTE 123", null, "IFOOD")]
    [InlineData(null, "Eu", "")]
    [InlineData(null, null, "")]
    public void Chave_eh_o_texto_sugerido_normalizado(string? descricao, string? pessoa, string esperado) =>
        Assert.Equal(esperado, NomeExtrato.Chave(descricao, pessoa));

    [Theory]
    [InlineData("BAR DOIS IRMAOS", "BARDOISIRMAOS")]
    [InlineData("FARMACIA LED FARMA LTDA", "FARMACIALEDFARMA")]
    [InlineData("Farmácia Led Farma Lt", "FARMACIALEDFARMA")]
    [InlineData("AUTO POSTO COPA OURO L", "AUTOPOSTOCOPAOURO")]
    [InlineData("12.345.678/0001-90 MERCADO X", "MERCADOX")]
    [InlineData("123.456.789-00 JOAO EIRELI", "JOAO")]
    [InlineData("LOJA ME", "LOJA")]
    [InlineData("", "")]
    public void Compacta_tira_documento_sufixo_e_espacos(string texto, string esperado) =>
        Assert.Equal(esperado, NomeExtrato.Compacta(texto));

    [Theory]
    [InlineData("BAR DOIS IRMAOS", "BARDOISIRMAOS", true)]
    [InlineData("FARMACIA LED FARMA LT", "FARMACIA LED FARMA LTDA", true)]
    [InlineData("TELEFONICA", "TELEFONICA BRAS", true)]
    [InlineData("SABOR DA VILA RESTAURA", "SABOR DA VILA RESTAURANTE LTDA", true)]
    [InlineData("GOOGLE CLOUD", "GOOGLE CLOUD BRASIL", true)]
    [InlineData("PIX", "PIZZA", false)]
    [InlineData("UBER", "UBER EATS", false)]
    [InlineData("MERCADO X", "MERCADO Y", false)]
    [InlineData("", "", false)]
    public void Parecidos(string a, string b, bool esperado)
    {
        Assert.Equal(esperado, NomeExtrato.Parecidos(a, b));
        Assert.Equal(esperado, NomeExtrato.Parecidos(b, a));
    }
}
