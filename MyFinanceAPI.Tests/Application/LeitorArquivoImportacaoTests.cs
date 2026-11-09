using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #76, fase 4: leitores por formato. Dados inventados, na estrutura dos arquivos reais.</summary>
public class LeitorArquivoImportacaoTests
{
    [Fact]
    public void PicPay_pula_so_cartao_sinaliza_misto_e_le_o_sinal()
    {
        var a = LeitorArquivoImportacao.LerCsv([
            "data,hora,tipo,\"origem / destino\",valor,\"forma de pagamento\"",
            "2026-11-03,16:25,\"Compra realizada\",\"Loja A\",\"−R$ 110,00\",\"Com cartão\"",
            "2026-11-03,11:47,\"Compra realizada\",MERCADO,\"−R$ 1.009,99\",\"Com saldo\"",
            "2026-11-03,11:46,\"Pix recebido\",\"Fulano, Filho\",\"+R$ 10,00\",",
            "2026-10-28,19:07,\"Pix enviado\",\"Beltrano\",\"−R$ 22,56\",\"Com saldo + cartão\"",
            "lixo"
        ]);

        Assert.Equal("PicPay", a.Formato);
        Assert.Equal((1, 1, 1), (a.Puladas, a.Sinalizadas, a.Invalidas));
        Assert.Equal([-1009.99m, 10m, -22.56m], a.Linhas.Select(l => l.Valor));
        Assert.Equal("Fulano, Filho", a.Linhas[1].Nome);
        Assert.NotNull(a.Linhas[2].Observacao);
        Assert.Contains("1 paga(s) com cartão", a.Aviso);
    }

    [Fact]
    public void Nubank_continua_igual_e_csv_desconhecido_e_recusado()
    {
        var a = LeitorArquivoImportacao.LerCsv(["Data,Valor,Identificador,Descrição", "01/10/2026,-12.50,a,Compra - LOJA, FILIAL"]);
        Assert.Equal(("Nubank", "Compra - LOJA, FILIAL"), (a.Formato, a.Linhas[0].Descricao));

        var ex = Assert.Throws<RegraNegocioException>(() => LeitorArquivoImportacao.LerCsv(["a;b;c", "1;2;3"]));
        Assert.Contains("Formato não reconhecido", ex.Message);
    }

    [Fact]
    public void Extrato_Caixa_ignora_saldos_e_distingue_linhas_identicas()
    {
        var a = LeitorArquivoImportacao.LerTextoPdf([
            "Extrato por período",
            "000000  SALDO ANTERIOR  1.023,62 D  0,00 D",
            "01/10/2026  000000  DEB IOF  6,34 D  1.029,96 D",
            "01/10/2026  000000  DEB IOF  6,34 D  1.036,30 D",
            "01/10/2026  000000  SALDO DIA  0,00 C  1.036,30 D",
            "05/10/2026  051709  CRED PIX  2.040,00 C  942,75 C"
        ]);

        Assert.Equal("Extrato Caixa", a.Formato);
        Assert.False(a.EhFatura);
        Assert.Equal([-6.34m, -6.34m, 2040m], a.Linhas.Select(l => l.Valor));
        Assert.Equal(["000000", "000000#2", "051709"], a.Linhas.Select(l => l.Identificador));
    }

    private static readonly string[] FaturaCaixa = [
        "VENCIMENTO", "06/09/2026", "VALOR TOTAL DESTA FATURA",
        "27/07  TOTAL DA FATURA ANTERIOR  501,76D",
        "05/08  OBRIGADO PELO PAGAMENTO  501,76C",
        "MULTA 2,00%    31/07  BarDoisIrmaos  CURITIBA  24,00D",
        "07/03  JOAO BETTEGA  06 DE 10  CURITIBA  49,99D",
        "Valor total desta fatura R$ 73,99 D"
    ];

    [Fact]
    public void Fatura_Caixa_entra_no_mes_do_vencimento_com_parcela_e_sem_cidade()
    {
        var a = LeitorArquivoImportacao.LerTextoPdf(FaturaCaixa);

        Assert.True(a.EhFatura);
        Assert.Equal(3, a.Linhas.Count);
        Assert.Equal((501.76m, "2026-09"), (a.Linhas[0].Valor, a.Linhas[0].NumeroFatura));
        Assert.Equal(("BarDoisIrmaos", -24m), (a.Linhas[1].Nome, a.Linhas[1].Valor));
        var parcelada = a.Linhas[2];
        Assert.Equal(("JOAO BETTEGA", 6, 10, new DateOnly(2026, 3, 7)), (parcelada.Nome, parcelada.ParcelaAtual!.Value, parcelada.QuantidadeParcelas!.Value, parcelada.Data));
    }

    [Fact]
    public void Fatura_que_nao_soma_o_total_nao_entra()
    {
        var erradas = FaturaCaixa.Select(l => l.Replace("73,99", "80,00")).ToArray();
        var ex = Assert.Throws<RegraNegocioException>(() => LeitorArquivoImportacao.LerTextoPdf(erradas));
        Assert.Contains("Nada foi importado", ex.Message);
    }

    [Fact]
    public void Fatura_PicPay_le_duas_colunas_parcela_e_creditos()
    {
        var a = LeitorArquivoImportacao.LerTextoPdf([
            "Vencimento: 05/11/2026 | Fechamento: 29/10/2026",
            "02/07 LOJA X PARC04/04 160,41    03/10 GOOGLE CLOUD 5,20",
            "30/09 PAGAMENTO DE FATURA -97,61",
            "17/10 IFD*IFOOD CLUB 5,95",
            "Total geral dos lançamentos 171,56"
        ]);

        Assert.Equal("Fatura PicPay", a.Formato);
        Assert.Equal(4, a.Linhas.Count);
        Assert.Equal(("LOJA X", -160.41m, 4, 4), (a.Linhas[0].Nome, a.Linhas[0].Valor, a.Linhas[0].ParcelaAtual!.Value, a.Linhas[0].QuantidadeParcelas!.Value));
        Assert.Equal(97.61m, a.Linhas[2].Valor);
        Assert.Equal("2026-11", a.Linhas[3].NumeroFatura);
    }

    [Theory]
    [InlineData("20/12", "2027-01-05", "2026-12-20")]
    [InlineData("02/01", "2027-01-05", "2027-01-02")]
    public void Data_sem_ano_nao_passa_do_vencimento(string diaMes, string vencimento, string esperado)
    {
        Assert.Equal(DateOnly.Parse(esperado), LeitorArquivoImportacao.DataSemAno(diaMes, DateOnly.Parse(vencimento)));
    }

    [Fact]
    public void Fatura_diz_o_mes_do_vencimento_na_primeira_linha()
    {
        Assert.Equal("2026-09", LeitorArquivoImportacao.LerTextoPdf(FaturaCaixa).Linhas.First(l => l.NumeroFatura != null).NumeroFatura);
    }

    [Fact]
    public void Pdf_sem_texto_ou_desconhecido_e_recusado()
    {
        Assert.Contains("foto", Assert.Throws<RegraNegocioException>(() => LeitorArquivoImportacao.LerTextoPdf([])).Message);
        Assert.Contains("Formato não reconhecido", Assert.Throws<RegraNegocioException>(() => LeitorArquivoImportacao.LerTextoPdf(["Boleto qualquer"])).Message);
    }
}
