using System.Globalization;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Card #77: números por conta no mês, para a listagem e o detalhe usarem os mesmos.</summary>
public class ResumoContasService(
    IBancoRepository bancos,
    IExtratoBancarioItemRepository itens,
    IExtratoBancarioRepository extratos,
    IAcertosService acertos) : IResumoContasService
{
    public async Task<ResumoContasDTO> ObterAsync(int userId, string? mes)
    {
        if (string.IsNullOrWhiteSpace(mes)
            || !DateOnly.TryParseExact(mes.Trim() + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            throw new RegraNegocioException("Mês inválido: use AAAA-MM.");

        var dosAcertos = await acertos.ObterAsync(userId, null, null, null);
        var compras = dosAcertos.Pessoas.SelectMany(p => p.Compras)
            .GroupBy(c => c.Id)
            .ToDictionary(g => g.Key, g => (Valor: g.Sum(c => c.Valor), Pago: g.Sum(c => c.Pago)));

        // Horário de Brasília para decidir se a fatura já fechou.
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(-3));
        return Montar(mes.Trim(), await bancos.ListarAsync(userId), await itens.ListarParaResumoAsync(userId),
            await extratos.ListarImportacoesAsync(userId, null), compras, hoje);
    }

    /// <summary>Puro, para testar. Os lançamentos vêm com tipo de cartão, categoria, pessoa e partes; os bancos com tipo e pessoas.</summary>
    public static ResumoContasDTO Montar(
        string mes,
        IEnumerable<Banco> listaBancos,
        IEnumerable<ExtratoBancarioItem> lancamentos,
        IEnumerable<ImportacaoResumo> importacoes,
        IReadOnlyDictionary<int, (decimal Valor, decimal Pago)> compras,
        DateOnly hoje)
    {
        var inicioMes = DateOnly.ParseExact(mes + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var todos = lancamentos.ToList();
        var doMes = todos.Where(i => ResumoCategoriasService.MesDoLancamento(i) == mes).ToList();
        var validas = importacoes.Where(i => i.DesfeitoEm == null).ToList();

        var contas = listaBancos
            .Select(b => MontarConta(b, doMes.Where(i => i.BancoId == b.Id).ToList(), validas.Where(i => i.BancoId == b.Id).ToList(), compras, inicioMes, hoje))
            .Where(c => c.Ativo || c.Quantidade > 0)
            .OrderBy(c => c.Nome, StringComparer.Create(new CultureInfo("pt-BR"), true))
            .ToList();

        var geral = ResumoCategoriasService.MontarEvolucao(todos, mes, 1).Meses[0];
        var compartilhados = contas.Where(c => c.EhCredito && c.Compartilhado).ToList();
        return new ResumoContasDTO
        {
            Mes = mes,
            Entradas = geral.Entradas,
            Saidas = geral.Saidas,
            ParaClassificar = contas.Sum(c => c.SemCategoria),
            FaltaReceber = compartilhados.Count > 0 ? compartilhados.Sum(c => c.FaltaReceber) : null,
            Contas = contas
        };
    }

    private static bool EhEntrada(ExtratoBancarioItem i) =>
        string.Equals(i.TipoLancamento, TipoLancamentoExtrato.Entrada, StringComparison.OrdinalIgnoreCase);

    private static bool SemCategoria(ExtratoBancarioItem i) =>
        i.Dividido ? i.Partes.Any(p => p.CategoriaId == null) : i.CategoriaId == null;

    private static bool DeOutraPessoa(PessoaMovimentacao? p) => p is { EhTitular: false };

    private static ResumoContaDTO MontarConta(
        Banco banco, List<ExtratoBancarioItem> daConta, List<ImportacaoResumo> importacoes,
        IReadOnlyDictionary<int, (decimal Valor, decimal Pago)> compras, DateOnly inicioMes, DateOnly hoje)
    {
        var saidas = daConta.Where(i => !EhEntrada(i)).ToList();
        var entradas = daConta.Where(EhEntrada).Sum(i => i.Valor);
        var totalSaidas = saidas.Sum(i => i.Valor);

        // Cada parte de um dividido conta pela pessoa dela.
        var pedacos = saidas.SelectMany(i => i.Dividido
            ? i.Partes.Select(p => (Pessoa: p.PessoaMovimentacao, p.Valor))
            : [(Pessoa: i.PessoaMovimentacao, i.Valor)]).ToList();
        var porPessoa = pedacos.Where(p => DeOutraPessoa(p.Pessoa))
            .GroupBy(p => p.Pessoa!.Id)
            .Select(g => new ResumoContaPessoaDTO { PessoaId = g.Key, Nome = g.First().Pessoa!.NomePessoa, Valor = g.Sum(p => p.Valor) })
            .OrderByDescending(p => p.Valor)
            .ToList();
        var deOutras = porPessoa.Sum(p => p.Valor);

        var idsDoMes = daConta.Where(i => i.ExtratoBancarioId != null).Select(i => i.ExtratoBancarioId!.Value).ToHashSet();
        var importouOMes = importacoes.Any(i => ImportacoesService.MesDe(i) == inicioMes);
        var ehCredito = banco.TipoCartao?.EhCredito == true;
        var dataFechamento = ehCredito && banco.DiaFechamento is int fechamento ? DataFechamento(inicioMes, fechamento, banco.DiaVencimento) : (DateOnly?)null;

        bool faltaImportar;
        if (dataFechamento is DateOnly fecha)
            faltaImportar = hoje >= fecha && !importouOMes;
        else
        {
            // Regra da Cobertura (card #75): só conta que já teve importação, e o mês corrente nunca é "falta".
            var primeiro = importacoes.Select(ImportacoesService.MesDe).Where(m => m != null).Min();
            faltaImportar = primeiro != null && inicioMes >= primeiro && inicioMes < new DateOnly(hoje.Year, hoje.Month, 1) && !importouOMes;
        }

        return new ResumoContaDTO
        {
            BancoId = banco.Id,
            Nome = banco.NomeBanco,
            TipoNome = banco.TipoCartao?.NomeTipoCartao ?? "",
            EhCredito = ehCredito,
            Ativo = banco.Ativo != false,
            Agencia = banco.Agencia,
            NumeroConta = banco.NumeroConta,
            Compartilhado = banco.Compartilhado,
            ConviteCompartilharDispensado = banco.ConviteCompartilharDispensado,
            Pessoas = banco.Pessoas.Where(p => p.PessoaMovimentacao != null).Select(p => p.PessoaMovimentacao.NomePessoa)
                .OrderBy(n => n, StringComparer.Create(new CultureInfo("pt-BR"), true)).ToList(),
            Quantidade = daConta.Count,
            Entradas = entradas,
            Saidas = totalSaidas,
            Resultado = entradas - totalSaidas,
            SemCategoria = daConta.Count(SemCategoria),
            Fatura = ehCredito ? totalSaidas : 0,
            PagamentosEstornos = ehCredito ? entradas : 0,
            ParcelasQueContinuam = saidas
                .Where(i => i.ParcelaAtual is int atual && i.QuantidadeParcelas is int total && total > atual)
                .Sum(i => i.Valor * (i.QuantidadeParcelas!.Value - i.ParcelaAtual!.Value)),
            MinhasCompras = totalSaidas - deOutras,
            DeOutrasPessoas = deOutras,
            PorPessoa = porPessoa,
            FaltaReceber = daConta.Sum(i => compras.TryGetValue(i.Id, out var c) ? Math.Max(0, c.Valor - c.Pago) : 0),
            ImportadoEm = importacoes.Where(i => idsDoMes.Contains(i.Id)).Select(i => (DateTime?)i.ImportadoEm).Max(),
            UltimaImportacao = importacoes.Select(i => (DateTime?)i.ImportadoEm).Max(),
            FaltaImportar = faltaImportar,
            DataFechamento = dataFechamento,
            DataVencimento = ehCredito && banco.DiaVencimento is int vence ? DiaNoMes(inicioMes, vence) : null
        };
    }

    /// <summary>
    /// Fechamento da fatura do mês (o do vencimento): no próprio mês quando o fechamento vem antes do vencimento,
    /// senão no mês anterior. Sem vencimento cadastrado, no próprio mês.
    /// </summary>
    public static DateOnly DataFechamento(DateOnly inicioMesFatura, int diaFechamento, int? diaVencimento) =>
        DiaNoMes(diaVencimento is int v && diaFechamento >= v ? inicioMesFatura.AddMonths(-1) : inicioMesFatura, diaFechamento);

    private static DateOnly DiaNoMes(DateOnly inicioMes, int dia) =>
        new(inicioMes.Year, inicioMes.Month, Math.Min(dia, DateTime.DaysInMonth(inicioMes.Year, inicioMes.Month)));
}
