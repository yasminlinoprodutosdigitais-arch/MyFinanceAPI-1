using System.Globalization;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #76, fase 5: onde o dinheiro foi no mês, por categoria, comparado com a média dos 3 meses anteriores que tenham
/// lançamentos. Conta só saídas minhas: o que é de quem entra nos acertos e os pagamentos de fatura/transferências ficam à parte.
/// </summary>
public class ResumoCategoriasService(IExtratoBancarioItemRepository itens) : IResumoCategoriasService
{
    private const int MesesDeComparacao = 3;
    /** Mês com menos lançamentos que isto é histórico incompleto: compará-lo daria percentuais sem sentido. */
    private const int MinimoDeLancamentosNoMes = 5;
    private static readonly string[] CategoriasFora = ["CARTAO DE CREDITO", "TRANSFERENCIA ENTRE CONTAS"];

    private sealed record Pedaco(string Mes, int? CategoriaId, string Nome, decimal Valor, bool DeOutraPessoa, bool Fora);

    public async Task<ResumoCategoriasDTO> ObterAsync(int userId, string? mes)
    {
        if (string.IsNullOrWhiteSpace(mes)
            || !DateOnly.TryParseExact(mes.Trim() + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            throw new RegraNegocioException("Mês inválido: use AAAA-MM.");
        return Montar(await itens.ListarParaResumoAsync(userId), mes.Trim());
    }

    /// <summary>Puro, para testar. Os lançamentos vêm com tipo de cartão, categoria, pessoa e partes carregados.</summary>
    public static ResumoCategoriasDTO Montar(IEnumerable<ExtratoBancarioItem> lancamentos, string mes)
    {
        var pedacos = lancamentos.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Saida).SelectMany(Pedacos).ToList();
        var doMes = pedacos.Where(p => p.Mes == mes).ToList();
        var meus = doMes.Where(p => !p.DeOutraPessoa && !p.Fora).ToList();

        // Só compara com 3 meses anteriores com movimento de verdade; enquanto não houver, não mostra (volta sozinho).
        var anteriores = pedacos.Where(p => string.CompareOrdinal(p.Mes, mes) < 0 && !p.DeOutraPessoa && !p.Fora)
            .GroupBy(p => p.Mes).Where(g => g.Count() >= MinimoDeLancamentosNoMes)
            .OrderByDescending(g => g.Key, StringComparer.Ordinal).Take(MesesDeComparacao).ToList();
        if (anteriores.Count < MesesDeComparacao)
            anteriores = [];
        var comparados = anteriores.Count;
        decimal? Media(Func<Pedaco, bool> filtro) =>
            comparados == 0 ? null : Math.Round(anteriores.Sum(g => g.Where(filtro).Sum(p => p.Valor)) / comparados, 2);

        var total = meus.Sum(p => p.Valor);
        var categorias = meus.GroupBy(p => p.CategoriaId).Select(g =>
        {
            var valor = g.Sum(p => p.Valor);
            var media = Media(p => p.CategoriaId == g.Key);
            return new ResumoCategoriaLinhaDTO
            {
                CategoriaId = g.Key,
                Nome = g.First().Nome,
                Valor = valor,
                Percentual = total > 0 ? Math.Round(valor / total * 100, 1) : 0,
                Media = media,
                Variacao = Variacao(valor, media)
            };
        }).OrderByDescending(c => c.Valor).ToList();

        var mediaTotal = Media(_ => true);
        return new ResumoCategoriasDTO
        {
            Mes = mes,
            Total = total,
            MediaAnterior = mediaTotal,
            VariacaoTotal = Variacao(total, mediaTotal),
            MesesComparados = comparados,
            DeOutrasPessoas = doMes.Where(p => p.DeOutraPessoa).Sum(p => p.Valor),
            ForaDoResumo = doMes.Where(p => p.Fora && !p.DeOutraPessoa).Sum(p => p.Valor),
            Categorias = categorias
        };
    }

    private static decimal? Variacao(decimal valor, decimal? media) =>
        media is > 0 ? Math.Round((valor - media.Value) / media.Value * 100, 0) : null;

    private static IEnumerable<Pedaco> Pedacos(ExtratoBancarioItem i)
    {
        var mes = MesDo(i);
        if (mes == null)
            return [];
        return i.Dividido
            ? i.Partes.Select(p => CriarPedaco(mes, p.CategoriaId, p.Categoria?.Nome, p.Valor, p.PessoaMovimentacao))
            : [CriarPedaco(mes, i.CategoriaId, i.Categoria?.Nome, i.Valor, i.PessoaMovimentacao)];
    }

    private static Pedaco CriarPedaco(string mes, int? categoriaId, string? nome, decimal valor, PessoaMovimentacao? pessoa) =>
        new(mes, categoriaId, nome ?? "Sem categoria", valor, pessoa is { EntraNosAcertos: true, EhTitular: false },
            CategoriasFora.Contains(RegraClassificacao.Normalizar(nome) ?? ""));

    /// <summary>O mês em que o lançamento aparece no Extrato: cartão de crédito pela fatura, o resto pela data.</summary>
    private static string? MesDo(ExtratoBancarioItem i) =>
        i.TipoCartao is { EhCredito: true } ? i.NumeroFatura : i.DataMovimentacao.ToString("yyyy-MM", CultureInfo.InvariantCulture);
}
