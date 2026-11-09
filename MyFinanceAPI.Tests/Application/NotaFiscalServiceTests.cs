using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #76, fase 6: NFC-e do PR. HTML inventado, na estrutura da página real.</summary>
public class NotaFiscalServiceTests
{
    private static string Linha(int n, string nome, string qtd, string valor) => $"""
        <tr id="Item + {n}"><td valign="top">
          <span class="txtTit2">{nome}</span><span class="RCod">(Código: 1)</span><br></br>
          <span class="Rqtd">
            <strong>Qtde.:</strong>{qtd}
          </span><span class="RUN"><strong>UN: </strong>Un</span>
          <span class="RvlUnit"><strong>Vl. Unit.:</strong>&nbsp;1,00</span></td>
          <td class="txtTit3 noWrap">Vl. Total<br></br><span class="valor">{valor}</span></td></tr>
        """;

    private static string Pagina(params string[] linhas) => $"""
        <div id="u20" class="txtTopo">MERCADO &amp; CIA</div>
        <table id="tabResult">{string.Concat(linhas)}</table>
        <div id="linhaTotal" class="linhaShade"><label>Valor a pagar R$:</label>
          <span class="totalNumb txtMax">1.024,40</span></div>
        """;

    [Fact]
    public void Le_estabelecimento_itens_e_total()
    {
        var nota = NotaFiscalService.Interpretar(Pagina(Linha(1, "SAB LIQ 1 4L", "1", "1.000,50"), Linha(2, "CHOCOLATE 40G", "2", "23,90")));

        Assert.Equal("MERCADO & CIA", nota.Estabelecimento);
        Assert.Equal(1024.40m, nota.Total);
        Assert.Equal([("SAB LIQ 1 4L", 1m, 1000.50m), ("CHOCOLATE 40G", 2m, 23.90m)], nota.Itens.Select(i => (i.Nome, i.Quantidade, i.Valor)));
    }

    [Fact]
    public void Pagina_sem_itens_e_recusada()
    {
        Assert.Throws<RegraNegocioException>(() => NotaFiscalService.Interpretar("<html>Nota não encontrada</html>"));
    }

    [Fact]
    public void Aceita_o_link_da_Fazenda_e_sempre_consulta_em_https()
    {
        var uri = NotaFiscalService.EnderecoDaConsulta("http://www.fazenda.pr.gov.br/nfce/qrcode?p=123|2|1|1|ABC");

        Assert.Equal("https", uri.Scheme);
        Assert.Equal("www.fazenda.pr.gov.br", uri.Host);
        Assert.Contains("p=123", uri.Query);
    }

    [Theory]
    [InlineData("https://evil.example/nfce/qrcode?p=1")]
    [InlineData("https://www.fazenda.pr.gov.br.evil.example/nfce/qrcode?p=1")]
    [InlineData("https://www.fazenda.pr.gov.br@evil.example/nfce/qrcode?p=1")]
    [InlineData("https://www.fazenda.pr.gov.br/outra?p=1")]
    [InlineData("https://www.sefaz.rs.gov.br/nfce/qrcode?p=1")]
    [InlineData("nao e link")]
    [InlineData(null)]
    public void Recusa_qualquer_outro_destino(string? link)
    {
        Assert.Throws<RegraNegocioException>(() => NotaFiscalService.EnderecoDaConsulta(link));
    }
}
