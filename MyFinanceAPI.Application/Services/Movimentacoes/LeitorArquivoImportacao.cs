using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Uma linha lida do arquivo. Valor com sinal: negativo = saída.</summary>
public sealed record LinhaImportada(DateOnly Data, decimal Valor, string Identificador, string Descricao)
{
    /// <summary>Nome do estabelecimento ou pessoa; null = a importação tira da descrição, como no Nubank.</summary>
    public string? Nome { get; init; }
    public int? ParcelaAtual { get; init; }
    public int? QuantidadeParcelas { get; init; }
    /// <summary>Mês da fatura (AAAA-MM).</summary>
    public string? NumeroFatura { get; init; }
    public string? Observacao { get; init; }
}

public sealed class ArquivoLido
{
    public string Formato { get; init; } = "";
    public bool EhFatura { get; init; }
    public List<LinhaImportada> Linhas { get; init; } = [];
    /// <summary>Linhas com data ou valor inválido.</summary>
    public int Invalidas { get; init; }
    /// <summary>PicPay "Com cartão": entram pela fatura.</summary>
    public int Puladas { get; init; }
    /// <summary>PicPay "Com saldo + cartão": entram, mas parte pode estar na fatura.</summary>
    public int Sinalizadas { get; init; }
    public bool Vazio => Formato == "";

    public string Aviso =>
        (Puladas > 0 ? $" {Puladas} paga(s) com cartão não entraram: vêm pela fatura." : "")
        + (Sinalizadas > 0 ? $" {Sinalizadas} paga(s) com saldo + cartão: confira se não está(ão) também na fatura." : "");
}

/// <summary>
/// Card #76, fase 4: reconhece o formato pelo conteúdo (CSV Nubank e PicPay; PDF de extrato Caixa, fatura Caixa e fatura
/// PicPay) e devolve as linhas. Formato desconhecido é recusado, e fatura cuja soma não bate com o total impresso também.
/// </summary>
public static partial class LeitorArquivoImportacao
{
    private const string NaoReconhecido = "Formato não reconhecido. Por enquanto: CSV do Nubank e do PicPay, PDF do extrato Caixa e das faturas Caixa e PicPay.";
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static ArquivoLido Ler(byte[] conteudo)
    {
        if (conteudo.Length == 0)
            return new ArquivoLido();
        if (conteudo.Length > 4 && Encoding.ASCII.GetString(conteudo, 0, 4) == "%PDF")
            return LerTextoPdf(LinhasDoPdf(conteudo));

        var texto = Encoding.UTF8.GetString(conteudo).TrimStart('﻿');
        return LerCsv(texto.Split('\n').Select(l => l.TrimEnd('\r')).ToList());
    }

    private static List<string> LinhasDoPdf(byte[] conteudo)
    {
        try
        {
            using var pdf = PdfDocument.Open(conteudo);
            var linhas = new List<string>();
            foreach (var pagina in pdf.GetPages())
            {
                // Palavras na mesma altura formam a linha; distância grande vira dois espaços (separa as colunas).
                foreach (var grupo in pagina.GetWords().GroupBy(w => Math.Round(w.BoundingBox.Bottom / 3)).OrderByDescending(g => g.Key))
                {
                    var sb = new StringBuilder();
                    double? fim = null;
                    foreach (var palavra in grupo.OrderBy(w => w.BoundingBox.Left))
                    {
                        if (fim != null)
                            sb.Append(palavra.BoundingBox.Left - fim > 8 ? "  " : " ");
                        sb.Append(palavra.Text);
                        fim = palavra.BoundingBox.Right;
                    }
                    linhas.Add(sb.ToString());
                }
            }
            return linhas;
        }
        catch (Exception e) when (e is not RegraNegocioException)
        {
            throw new RegraNegocioException(NaoReconhecido);
        }
    }

    public static ArquivoLido LerCsv(IReadOnlyList<string> linhas)
    {
        var cheias = linhas.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        if (cheias.Count == 0)
            return new ArquivoLido();
        if (cheias[0].StartsWith("data,hora,tipo", StringComparison.OrdinalIgnoreCase))
            return LerPicPayCsv(cheias);
        if (cheias[0].StartsWith("Data,Valor", StringComparison.OrdinalIgnoreCase))
            return LerNubankCsv(cheias);
        throw new RegraNegocioException(NaoReconhecido);
    }

    public static ArquivoLido LerTextoPdf(IReadOnlyList<string> linhas)
    {
        var texto = string.Join('\n', linhas);
        if (string.IsNullOrWhiteSpace(texto))
            throw new RegraNegocioException("Este PDF não tem texto (parece uma foto). " + NaoReconhecido);
        if (Regex.IsMatch(texto, "Total geral dos lan", RegexOptions.IgnoreCase))
            return LerFaturaPicPay(linhas, texto);
        if (Regex.IsMatch(texto, "Valor total desta fatura", RegexOptions.IgnoreCase))
            return LerFaturaCaixa(linhas, texto);
        if (Regex.IsMatch(texto, "Extrato por per", RegexOptions.IgnoreCase))
            return LerExtratoCaixa(linhas);
        throw new RegraNegocioException(NaoReconhecido);
    }

    private static ArquivoLido LerNubankCsv(List<string> linhas)
    {
        var lidas = new List<LinhaImportada>();
        var invalidas = 0;
        foreach (var linha in linhas.Skip(1))
        {
            var p = linha.Split(',', 4);
            if (p.Length < 4
                || !DateOnly.TryParseExact(p[0].Trim(), "dd/MM/yyyy", PtBr, DateTimeStyles.None, out var data)
                || !decimal.TryParse(p[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var valor))
            {
                invalidas++;
                continue;
            }
            lidas.Add(new LinhaImportada(data, valor, p[2].Trim(), p[3].Trim()));
        }
        return new ArquivoLido { Formato = "Nubank", Linhas = lidas, Invalidas = invalidas };
    }

    private static ArquivoLido LerPicPayCsv(List<string> linhas)
    {
        var lidas = new List<LinhaImportada>();
        int invalidas = 0, puladas = 0, sinalizadas = 0;
        foreach (var linha in linhas.Skip(1))
        {
            var c = ColunasCsv(linha);
            if (c.Count < 5
                || !DateOnly.TryParseExact(c[0].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data)
                || !ValorPicPay(c[4], out var valor))
            {
                invalidas++;
                continue;
            }

            var forma = c.Count > 5 ? c[5].Trim() : "";
            if (forma.Equals("Com cartão", StringComparison.OrdinalIgnoreCase))
            {
                puladas++;
                continue;
            }
            var misto = forma.Contains("cartão", StringComparison.OrdinalIgnoreCase);
            if (misto)
                sinalizadas++;

            var nome = c[3].Trim();
            lidas.Add(new LinhaImportada(data, valor, c[1].Trim(), $"{c[2].Trim()} - {nome}")
            {
                Nome = nome,
                Observacao = misto ? "Pago com saldo + cartão: parte pode estar também na fatura." : null
            });
        }
        return new ArquivoLido { Formato = "PicPay", Linhas = Distinguir(lidas), Invalidas = invalidas, Puladas = puladas, Sinalizadas = sinalizadas };
    }

    private static bool ValorPicPay(string texto, out decimal valor)
    {
        var limpo = texto.Replace('−', '-').Replace("R$", "").Replace(" ", "").Replace(" ", "");
        return decimal.TryParse(limpo, NumberStyles.Number | NumberStyles.AllowLeadingSign, PtBr, out valor);
    }

    private static List<string> ColunasCsv(string linha)
    {
        var colunas = new List<string>();
        var atual = new StringBuilder();
        var aspas = false;
        foreach (var ch in linha)
        {
            if (ch == '"') aspas = !aspas;
            else if (ch == ',' && !aspas) { colunas.Add(atual.ToString()); atual.Clear(); }
            else atual.Append(ch);
        }
        colunas.Add(atual.ToString());
        return colunas;
    }

    private static ArquivoLido LerExtratoCaixa(IReadOnlyList<string> linhas)
    {
        var lidas = new List<LinhaImportada>();
        foreach (var linha in linhas)
        {
            var m = LinhaExtratoCaixa().Match(linha);
            if (!m.Success || m.Groups["hist"].Value.Contains("SALDO", StringComparison.OrdinalIgnoreCase))
                continue;
            var data = DateOnly.ParseExact(m.Groups["data"].Value, "dd/MM/yyyy", PtBr);
            var valor = Numero(m.Groups["valor"].Value) * (m.Groups["dc"].Value == "D" ? -1 : 1);
            lidas.Add(new LinhaImportada(data, valor, m.Groups["doc"].Value, Espacos().Replace(m.Groups["hist"].Value, " ")));
        }
        if (lidas.Count == 0)
            throw new RegraNegocioException(NaoReconhecido);
        return new ArquivoLido { Formato = "Extrato Caixa", Linhas = Distinguir(lidas) };
    }

    private static ArquivoLido LerFaturaCaixa(IReadOnlyList<string> linhas, string texto)
    {
        var vencimento = Vencimento(texto);
        var total = Total(TotalCaixa().Match(texto));
        var lidas = new List<LinhaImportada>();
        decimal soma = 0;
        foreach (var linha in linhas)
        {
            foreach (Match m in LinhaFaturaCaixa().Matches(linha))
            {
                var bruto = m.Groups["desc"].Value;
                if (bruto.Contains("FATURA ANTERIOR", StringComparison.OrdinalIgnoreCase))
                    continue;
                var valor = Numero(m.Groups["valor"].Value);
                var credito = m.Groups["dc"].Value == "C";
                var pagamento = bruto.Contains("PAGAMENTO", StringComparison.OrdinalIgnoreCase) && credito;
                if (!pagamento)
                    soma += credito ? -valor : valor;
                lidas.Add(LinhaFatura(m.Groups["data"].Value, bruto, credito ? valor : -valor, vencimento, ParcelaDe().Match(bruto)));
            }
        }
        Conferir(soma, total);
        return new ArquivoLido { Formato = "Fatura Caixa", EhFatura = true, Linhas = Distinguir(lidas) };
    }

    private static ArquivoLido LerFaturaPicPay(IReadOnlyList<string> linhas, string texto)
    {
        var vencimento = Vencimento(texto);
        var total = Total(TotalPicPay().Match(texto));
        var lidas = new List<LinhaImportada>();
        decimal soma = 0;
        foreach (var linha in linhas)
        {
            foreach (Match m in LinhaFaturaPicPay().Matches(linha))
            {
                var valor = Numero(m.Groups["valor"].Value);
                // O "Total geral" do PicPay soma só as despesas: pagamentos e estornos (negativos) ficam de fora dele.
                if (valor > 0)
                    soma += valor;
                lidas.Add(LinhaFatura(m.Groups["data"].Value, m.Groups["desc"].Value, -valor, vencimento, ParcelaParc().Match(m.Groups["desc"].Value)));
            }
        }
        Conferir(soma, total);
        return new ArquivoLido { Formato = "Fatura PicPay", EhFatura = true, Linhas = Distinguir(lidas) };
    }

    private static LinhaImportada LinhaFatura(string diaMes, string bruto, decimal valor, DateOnly vencimento, Match parcela)
    {
        // Colunas separadas por dois espaços: a primeira é o nome; a cidade fica de fora.
        var semParcela = parcela.Success ? bruto.Remove(parcela.Index, parcela.Length) : bruto;
        var nome = Espacos().Replace(ColunaLarga().Split(semParcela.Trim())[0], " ").Trim();
        var parc = parcela.Success ? $"{parcela.Groups["a"].Value}/{parcela.Groups["t"].Value}" : "";
        return new LinhaImportada(DataSemAno(diaMes, vencimento), valor, parc, nome)
        {
            Nome = nome,
            NumeroFatura = vencimento.ToString("yyyy-MM", CultureInfo.InvariantCulture),
            ParcelaAtual = parcela.Success ? int.Parse(parcela.Groups["a"].Value) : null,
            QuantidadeParcelas = parcela.Success ? int.Parse(parcela.Groups["t"].Value) : null
        };
    }

    /// <summary>O ano do vencimento; data depois dele é do ano anterior.</summary>
    public static DateOnly DataSemAno(string diaMes, DateOnly vencimento)
    {
        var p = diaMes.Split('/');
        var dia = int.Parse(p[0]);
        var mes = int.Parse(p[1]);
        var ano = new DateOnly(vencimento.Year, mes, 1).AddDays(dia - 1) > vencimento ? vencimento.Year - 1 : vencimento.Year;
        return new DateOnly(ano, mes, Math.Min(dia, DateTime.DaysInMonth(ano, mes)));
    }

    private static DateOnly Vencimento(string texto)
    {
        var m = VencimentoRegex().Match(texto);
        if (!m.Success)
            throw new RegraNegocioException("Não achei o vencimento da fatura: " + NaoReconhecido);
        return DateOnly.ParseExact(m.Groups[1].Value, "dd/MM/yyyy", PtBr);
    }

    private static decimal Total(Match m) =>
        m.Success ? Numero(m.Groups[1].Value) : throw new RegraNegocioException("Não achei o total da fatura: " + NaoReconhecido);

    private static void Conferir(decimal soma, decimal total)
    {
        if (soma != total)
            throw new RegraNegocioException($"A soma das compras lidas ({soma.ToString("N2", PtBr)}) não bate com o total da fatura ({total.ToString("N2", PtBr)}). Nada foi importado.");
    }

    private static decimal Numero(string texto) => decimal.Parse(texto, NumberStyles.Number | NumberStyles.AllowLeadingSign, PtBr);

    /// <summary>Linhas idênticas no mesmo arquivo são lançamentos diferentes: a repetição ganha um número no identificador.</summary>
    private static List<LinhaImportada> Distinguir(List<LinhaImportada> linhas)
    {
        var vistas = new Dictionary<(DateOnly, decimal, string, string), int>();
        return linhas.Select(l =>
        {
            var chave = (l.Data, l.Valor, l.Identificador, l.Descricao);
            var n = vistas[chave] = vistas.GetValueOrDefault(chave) + 1;
            return n == 1 ? l : l with { Identificador = $"{l.Identificador}#{n}" };
        }).ToList();
    }

    [GeneratedRegex(@"(?<data>\d{2}/\d{2}/\d{4})\s+(?<doc>\d{6})\s+(?<hist>.+?)\s+(?<valor>[\d.]+,\d{2})\s*(?<dc>[DC])\s+[\d.]+,\d{2}\s*[DC]\s*$")]
    private static partial Regex LinhaExtratoCaixa();
    [GeneratedRegex(@"(?<![\d/])(?<data>\d{2}/\d{2})(?![/\d])\s+(?<desc>.+?)\s+(?<valor>[\d.]+,\d{2})\s*(?<dc>[DC])(?![A-Za-z])")]
    private static partial Regex LinhaFaturaCaixa();
    [GeneratedRegex(@"(?<![\d/])(?<data>\d{2}/\d{2})(?![/\d])\s+(?<desc>.+?)\s+(?<valor>-?[\d.]+,\d{2})(?![\d%])")]
    private static partial Regex LinhaFaturaPicPay();
    [GeneratedRegex(@"\b(?<a>\d{2})\s+DE\s+(?<t>\d{2})\b")]
    private static partial Regex ParcelaDe();
    [GeneratedRegex(@"\s*PARC(?<a>\d{2})/(?<t>\d{2})")]
    private static partial Regex ParcelaParc();
    [GeneratedRegex(@"VENCIMENTO\D{0,80}?(\d{2}/\d{2}/\d{4})", RegexOptions.IgnoreCase)]
    private static partial Regex VencimentoRegex();
    [GeneratedRegex(@"Valor total desta fatura\s*R\$\s*([\d.]+,\d{2})", RegexOptions.IgnoreCase)]
    private static partial Regex TotalCaixa();
    [GeneratedRegex(@"Total geral dos lan\S*\s+([\d.]+,\d{2})", RegexOptions.IgnoreCase)]
    private static partial Regex TotalPicPay();
    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex ColunaLarga();
    [GeneratedRegex(@"\s+")]
    private static partial Regex Espacos();
}
