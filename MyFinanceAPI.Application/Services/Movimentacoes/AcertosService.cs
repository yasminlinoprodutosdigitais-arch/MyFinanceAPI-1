using System.Globalization;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Acertos com pessoas (card #45, parte 3). Regras (decisões de 05/11/2026):
/// <list type="bullet">
/// <item>Dívida = saídas no cartão e na conta corrente de uma pessoa com "Entra nos acertos"; Recebido = entradas dela.</item>
/// <item>Pagamento pode abater uma compra (AbateItemId); o Pago da compra soma os pagamentos dela de qualquer data.</item>
/// <item>"Ainda vai cobrar": parcelas futuras, à parte do Falta; a mesma compra (descrição sem números + valor + total) usa a maior parcela importada.</item>
/// <item>Período filtra Devido e Recebido pela data. Com conta, entram as compras dela e os pagamentos que as abatem.</item>
/// <item>O "Eu" vem à parte e nunca soma nos totais.</item>
/// </list>
/// </summary>
public class AcertosService(IExtratoBancarioItemRepository itens) : IAcertosService
{
    public async Task<AcertosDTO> ObterAsync(int userId, string? inicio, string? fim, int? bancoId)
    {
        var de = Mes(inicio, nameof(inicio));
        var ate = Mes(fim, nameof(fim));
        if (de != null && ate != null && ate < de)
            throw new RegraNegocioException("O fim do período não pode ser antes do início.");

        var lista = await itens.ListarParaAcertosAsync(userId);
        return Montar(lista, de, ate?.AddMonths(1), bancoId);
    }

    private static DateOnly? Mes(string? mes, string campo)
    {
        if (string.IsNullOrWhiteSpace(mes))
            return null;
        if (!DateOnly.TryParseExact(mes.Trim() + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            throw new RegraNegocioException($"Mês inválido em {campo}: use AAAA-MM.");
        return data;
    }

    /// <summary>Monta os acertos a partir dos lançamentos com pessoa (com a pessoa carregada). Puro, para testar.</summary>
    public static AcertosDTO Montar(IEnumerable<ExtratoBancarioItem> lancamentos, DateOnly? inicio, DateOnly? fimExclusive, int? bancoId)
    {
        var todos = lancamentos
            .Where(i => i.PessoaMovimentacao is { } p && (p.EntraNosAcertos || p.EhTitular))
            .ToList();

        var compras = todos.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Saida).ToList();
        var pagamentos = todos.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Entrada).ToList();
        var pagoPorCompra = pagamentos.Where(p => p.AbateItemId != null)
            .GroupBy(p => p.AbateItemId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Valor));

        bool NoPeriodo(ExtratoBancarioItem i) =>
            (inicio == null || i.DataMovimentacao >= inicio) && (fimExclusive == null || i.DataMovimentacao < fimExclusive);

        var comprasDaConta = compras.Where(c => bancoId == null || c.BancoId == bancoId).ToList();
        var idsDaConta = comprasDaConta.Select(c => c.Id).ToHashSet();
        // Com conta, só os pagamentos que abatem compras dela; o saldo geral não tem conta de origem.
        var pagamentosDaConta = pagamentos.Where(p => bancoId == null || (p.AbateItemId != null && idsDaConta.Contains(p.AbateItemId.Value))).ToList();

        var pessoas = todos.Select(i => i.PessoaMovimentacao!).DistinctBy(p => p.Id).ToList();
        var resultado = new AcertosDTO();

        foreach (var pessoa in pessoas)
        {
            var dela = comprasDaConta.Where(c => c.PessoaMovimentacaoId == pessoa.Id).ToList();
            var pagos = pagamentosDaConta.Where(p => p.PessoaMovimentacaoId == pessoa.Id).ToList();
            var comprasNoPeriodo = dela.Where(NoPeriodo).OrderByDescending(c => c.DataMovimentacao).ThenByDescending(c => c.Id).ToList();
            var pagosNoPeriodo = pagos.Where(NoPeriodo).OrderByDescending(p => p.DataMovimentacao).ThenByDescending(p => p.Id).ToList();
            var parcelas = ProximasParcelas(dela);

            if (comprasNoPeriodo.Count == 0 && pagosNoPeriodo.Count == 0 && parcelas.Count == 0)
                continue;

            var devido = comprasNoPeriodo.Sum(c => c.Valor);
            var recebido = pagosNoPeriodo.Sum(p => p.Valor);
            var acerto = new AcertoPessoaDTO
            {
                PessoaId = pessoa.Id,
                Nome = pessoa.EhTitular ? "Eu" : pessoa.NomePessoa,
                EhTitular = pessoa.EhTitular,
                Devido = devido,
                Recebido = recebido,
                Falta = devido - recebido,
                Quitado = devido - recebido <= 0,
                AindaVaiCobrar = parcelas.Sum(p => p.Valor),
                ProximasParcelas = parcelas,
                Compras = comprasNoPeriodo.Select(c =>
                {
                    var pago = pagoPorCompra.GetValueOrDefault(c.Id);
                    return new AcertoCompraDTO
                    {
                        Id = c.Id, Data = c.DataMovimentacao, Descricao = c.Descricao, Valor = c.Valor, BancoId = c.BancoId,
                        BancoNome = c.Banco?.NomeBanco, ParcelaAtual = c.ParcelaAtual, QuantidadeParcelas = c.QuantidadeParcelas,
                        Pago = pago, Quitada = pago >= c.Valor
                    };
                }).ToList(),
                Pagamentos = pagosNoPeriodo.Select(p => new AcertoPagamentoDTO
                {
                    Id = p.Id, Data = p.DataMovimentacao, Descricao = p.Descricao, Valor = p.Valor, BancoId = p.BancoId,
                    BancoNome = p.Banco?.NomeBanco, AbateItemId = p.AbateItemId
                }).ToList()
            };

            if (pessoa.EhTitular)
            {
                resultado.Eu = acerto;
                continue;
            }

            resultado.Pessoas.Add(acerto);
        }

        resultado.Pessoas = resultado.Pessoas.OrderByDescending(p => p.Falta).ThenBy(p => p.Nome, StringComparer.Create(new CultureInfo("pt-BR"), true)).ToList();
        resultado.Totais = new TotaisAcertosDTO
        {
            Devido = resultado.Pessoas.Sum(p => p.Devido),
            Recebido = resultado.Pessoas.Sum(p => p.Recebido),
            FaltaReceber = resultado.Pessoas.Where(p => p.Falta > 0).Sum(p => p.Falta),
            AindaVaiCobrar = resultado.Pessoas.Sum(p => p.AindaVaiCobrar)
        };
        return resultado;
    }

    /// <summary>
    /// Parcelas que ainda vão vir. A mesma compra aparece uma vez por fatura importada; agrupa pela descrição sem
    /// números + valor + total de parcelas e parte da maior parcela já importada.
    /// </summary>
    private static List<AcertoParcelaFuturaDTO> ProximasParcelas(IEnumerable<ExtratoBancarioItem> compras) =>
        compras
            // A última parcela também entra no grupo: importada, ela zera o que falta daquela compra.
            .Where(c => c.QuantidadeParcelas is > 1 && c.ParcelaAtual is > 0 && c.ParcelaAtual <= c.QuantidadeParcelas)
            .GroupBy(c => (Chave: string.IsNullOrEmpty(c.ChaveDescricao) ? ExtratoBancarioService.GerarChaveDescricao(c.Descricao) : c.ChaveDescricao,
                c.Valor, Total: c.QuantidadeParcelas!.Value))
            .Select(g => g.OrderByDescending(c => c.ParcelaAtual).ThenByDescending(c => c.DataMovimentacao).First())
            .SelectMany(ultima =>
            {
                var base_ = MesDaFatura(ultima);
                return Enumerable.Range(ultima.ParcelaAtual!.Value + 1, ultima.QuantidadeParcelas!.Value - ultima.ParcelaAtual!.Value)
                    .Select(parcela => new AcertoParcelaFuturaDTO
                    {
                        Mes = base_.AddMonths(parcela - ultima.ParcelaAtual!.Value).ToString("yyyy-MM", CultureInfo.InvariantCulture),
                        Descricao = ultima.Descricao,
                        Valor = ultima.Valor,
                        Parcela = parcela,
                        QuantidadeParcelas = ultima.QuantidadeParcelas!.Value
                    });
            })
            .OrderBy(p => p.Mes).ThenBy(p => p.Descricao)
            .ToList();

    /// <summary>Mês da fatura ("AAAA-MM"), ou o mês da compra quando não há fatura.</summary>
    private static DateOnly MesDaFatura(ExtratoBancarioItem compra) =>
        DateOnly.TryParseExact((compra.NumeroFatura ?? "") + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fatura)
            ? fatura
            : new DateOnly(compra.DataMovimentacao.Year, compra.DataMovimentacao.Month, 1);
}
