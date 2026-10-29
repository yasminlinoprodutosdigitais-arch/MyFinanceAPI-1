using System.Globalization;
using System.Text;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Comparação de duas listas da mesma série (card #67, parte 2; regras no card #68). Pura: recebe as listas,
/// os itens e a corrente de origens da série e devolve a cascata, item a item e destaques.
/// <list type="bullet">
/// <item>Pareamento: mesma raiz da corrente de item_origem_id; o que sobrar, pelo nome normalizado.</item>
/// <item>Par com valor nos dois lados: preço = (va − vp) × qp e quantidade = (qa − qp) × va com as duas listas
/// em UNITARIO; senão toda a diferença é preço.</item>
/// <item>Novos e que saíram entram pelo total. Par com valor só de um lado conta esse lado como novo ou saiu e vai
/// para o aviso de sem valor: assim os totais são os do resumo e a cascata fecha no centavo.</item>
/// </list>
/// </summary>
public static class ComparacaoListas
{
    public const string Novo = "NOVO";
    public const string Saiu = "SAIU";
    public const string PrecoSubiu = "PRECO_SUBIU";
    public const string PrecoCaiu = "PRECO_CAIU";
    public const string Quantidade = "QUANTIDADE";

    public static ComparacaoListasDTO Comparar(
        Lista anterior, IEnumerable<ItemLista> itensAnterior,
        Lista atual, IEnumerable<ItemLista> itensAtual,
        IReadOnlyDictionary<int, int?> origens, bool soFeitos)
    {
        var deAnterior = itensAnterior.Where(i => !soFeitos || i.Feito).ToList();
        var deAtual = itensAtual.Where(i => !soFeitos || i.Feito).ToList();
        var unitario = anterior.ModoValor == ModoValorLista.Unitario && atual.ModoValor == ModoValorLista.Unitario;

        var efeitos = new EfeitosComparacaoDTO();
        var linhas = new List<ItemComparadoDTO>();
        var semValor = new SortedSet<string>(StringComparer.InvariantCulture);
        DestaqueComparacaoDTO? maiorAlta = null;
        decimal maiorAltaRazao = 0;

        foreach (var (p, a) in Parear(deAnterior, deAtual, origens))
        {
            var tp = p is null ? null : Total(p, anterior.ModoValor);
            var ta = a is null ? null : Total(a, atual.ModoValor);
            var linha = new ItemComparadoDTO
            {
                Descricao = (a ?? p)!.Descricao,
                Anterior = p is null ? null : Lado(p, tp),
                Atual = a is null ? null : Lado(a, ta),
                Diferenca = (ta ?? 0) - (tp ?? 0),
                Percentual = tp is > 0 && ta is not null ? Percentual(ta.Value - tp.Value, tp.Value) : null,
            };

            if (p is null)
            {
                linha.Mudancas.Add(Novo);
                if (ta is null) semValor.Add(linha.Descricao); else efeitos.Novos += ta.Value;
            }
            else if (a is null)
            {
                linha.Mudancas.Add(Saiu);
                if (tp is null) semValor.Add(linha.Descricao); else efeitos.Sairam += tp.Value;
            }
            else
            {
                if (p.Valor is { } vp && a.Valor is { } va && va != vp)
                    linha.Mudancas.Add(va > vp ? PrecoSubiu : PrecoCaiu);
                var qp = p.Quantidade ?? 1;
                var qa = a.Quantidade ?? 1;
                if (qa != qp)
                    linha.Mudancas.Add(Quantidade);

                if (tp is null || ta is null)
                {
                    semValor.Add(linha.Descricao);
                    if (tp is not null) efeitos.Sairam += tp.Value;
                    if (ta is not null) efeitos.Novos += ta.Value;
                }
                else if (unitario)
                {
                    efeitos.Preco += (a.Valor!.Value - p.Valor!.Value) * qp;
                    efeitos.Quantidade += (qa - qp) * a.Valor!.Value;
                }
                else
                {
                    efeitos.Preco += ta.Value - tp.Value;
                }

                if (p.Valor is > 0 && a.Valor > p.Valor)
                {
                    var razao = (a.Valor.Value - p.Valor.Value) / p.Valor.Value;
                    if (maiorAlta is null || razao > maiorAltaRazao)
                    {
                        maiorAltaRazao = razao;
                        maiorAlta = new DestaqueComparacaoDTO { Descricao = linha.Descricao, Diferenca = linha.Diferenca, Percentual = Arredondar(razao * 100) };
                    }
                }
            }

            linhas.Add(linha);
        }

        var totalAnterior = deAnterior.Sum(i => Total(i, anterior.ModoValor) ?? 0);
        var totalAtual = deAtual.Sum(i => Total(i, atual.ModoValor) ?? 0);
        var comMudanca = linhas.Where(l => l.Diferenca != 0).ToList();

        return new ComparacaoListasDTO
        {
            Anterior = new ListaComparadaDTO { Id = anterior.Id, Nome = anterior.Nome, Total = totalAnterior },
            Atual = new ListaComparadaDTO { Id = atual.Id, Nome = atual.Nome, Total = totalAtual },
            Diferenca = totalAtual - totalAnterior,
            Percentual = totalAnterior > 0 ? Percentual(totalAtual - totalAnterior, totalAnterior) : null,
            Efeitos = efeitos,
            Itens = linhas.OrderByDescending(l => Math.Abs(l.Diferenca)).ThenBy(l => l.Descricao, StringComparer.InvariantCulture).ToList(),
            Destaques = new DestaquesComparacaoDTO
            {
                MaiorAltaPreco = maiorAlta,
                MaiorImpacto = comMudanca.OrderByDescending(l => Math.Abs(l.Diferenca)).Select(Destaque).FirstOrDefault(),
                MaiorQueda = comMudanca.Where(l => l.Diferenca < 0).OrderBy(l => l.Diferenca).Select(Destaque).FirstOrDefault(),
            },
            SemValor = semValor.ToList(),
        };
    }

    /// <summary>Mesma conta do resumo: UNITARIO = quantidade × valor (sem quantidade conta 1); TOTAL = o valor.</summary>
    private static decimal? Total(ItemLista item, string modoValor) =>
        item.Valor is not { } valor ? null : modoValor == ModoValorLista.Unitario ? (item.Quantidade ?? 1) * valor : valor;

    private static LadoItemComparadoDTO Lado(ItemLista item, decimal? total) =>
        new() { Quantidade = item.Quantidade, Valor = item.Valor, Total = total };

    private static DestaqueComparacaoDTO Destaque(ItemComparadoDTO l) =>
        new() { Descricao = l.Descricao, Diferenca = l.Diferenca, Percentual = l.Percentual };

    private static decimal Percentual(decimal diferenca, decimal base_) => Arredondar(diferenca / base_ * 100);

    private static decimal Arredondar(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);

    /// <summary>Pares (anterior, atual); um lado null = item só na outra lista.</summary>
    private static List<(ItemLista? Anterior, ItemLista? Atual)> Parear(
        List<ItemLista> deAnterior, List<ItemLista> deAtual, IReadOnlyDictionary<int, int?> origens)
    {
        var pares = new List<(ItemLista?, ItemLista?)>();
        var soltosAnterior = deAnterior.OrderBy(i => i.Id).ToList();
        var soltosAtual = new List<ItemLista>();

        var porRaiz = soltosAnterior.GroupBy(i => Raiz(i.Id, origens)).ToDictionary(g => g.Key, g => new Queue<ItemLista>(g));
        foreach (var a in deAtual.OrderBy(i => i.Id))
        {
            if (porRaiz.TryGetValue(Raiz(a.Id, origens), out var fila) && fila.TryDequeue(out var p))
            {
                pares.Add((p, a));
                soltosAnterior.Remove(p);
            }
            else
                soltosAtual.Add(a);
        }

        var porNome = soltosAnterior.GroupBy(i => NomeNormalizado(i.Descricao)).ToDictionary(g => g.Key, g => new Queue<ItemLista>(g));
        foreach (var a in soltosAtual)
        {
            if (porNome.TryGetValue(NomeNormalizado(a.Descricao), out var fila) && fila.TryDequeue(out var p))
                pares.Add((p, a));
            else
                pares.Add((null, a));
        }

        pares.AddRange(porNome.Values.SelectMany(f => f).Select(p => ((ItemLista?)p, (ItemLista?)null)));
        return pares;
    }

    /// <summary>Primeiro item da corrente de origens (para em ciclo ou em origem desconhecida).</summary>
    private static int Raiz(int id, IReadOnlyDictionary<int, int?> origens)
    {
        var vistos = new HashSet<int>();
        while (vistos.Add(id) && origens.TryGetValue(id, out var origem) && origem is { } anterior)
            id = anterior;
        return id;
    }

    /// <summary>Sem acento, minúsculo e sem espaços extras: "  Feijão   Carioca " = "feijao carioca".</summary>
    public static string NomeNormalizado(string nome)
    {
        var semAcento = new StringBuilder();
        foreach (var c in nome.Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                semAcento.Append(c);
        return string.Join(' ', semAcento.ToString().ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
