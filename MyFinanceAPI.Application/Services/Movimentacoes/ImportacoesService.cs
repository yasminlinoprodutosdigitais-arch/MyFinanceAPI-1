using System.Globalization;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Card #75: o extrato é o registro da importação; desfazer tira os lançamentos e mantém a linha como Desfeita.</summary>
public class ImportacoesService(
    IExtratoBancarioRepository extratos, IExtratoBancarioItemRepository itens, IBancoService bancos, IUnitOfWork unitOfWork) : IImportacoesService
{
    private const int TamanhoMotivo = 200;

    public async Task<IReadOnlyList<ImportacaoDTO>> ListarAsync(int userId, int? meses)
    {
        var desde = meses is > 0 ? DateTime.UtcNow.AddMonths(-meses.Value) : (DateTime?)null;
        var nomes = await NomesDosBancosAsync(userId);
        return (await extratos.ListarImportacoesAsync(userId, desde)).Select(r => new ImportacaoDTO
        {
            Id = r.Id,
            BancoId = r.BancoId,
            BancoNome = nomes.GetValueOrDefault(r.BancoId) ?? "Banco não encontrado",
            NomeArquivo = r.NomeArquivo,
            ImportadoEm = r.ImportadoEm,
            MesReferencia = MesDe(r)?.ToString("yyyy-MM", CultureInfo.InvariantCulture),
            Lancamentos = r.Lancamentos,
            Duplicados = r.Duplicados,
            SemCategoria = r.SemCategoria,
            Entradas = r.Entradas,
            Saidas = r.Saidas,
            Status = r.DesfeitoEm != null ? "Desfeita" : r.SemCategoria > 0 ? "Pendencias" : "Concluida",
            DesfeitoEm = r.DesfeitoEm,
            MotivoDesfazer = r.MotivoDesfazer
        }).ToList();
    }

    public async Task<CoberturaImportacoesDTO> CoberturaAsync(int userId, int meses)
    {
        var nomes = await NomesDosBancosAsync(userId);
        return MontarCobertura(await extratos.ListarImportacoesAsync(userId, null), nomes, DateOnly.FromDateTime(DateTime.UtcNow), Math.Clamp(meses, 1, 24));
    }

    public async Task<VinculosImportacaoDTO> VinculosAsync(int userId, int id)
    {
        var v = await extratos.ObterVinculosAsync(id, userId) ?? throw new KeyNotFoundException("Importação não encontrada.");
        return new VinculosImportacaoDTO
        {
            Lancamentos = v.Lancamentos, Divididos = v.Divididos, ComNota = v.ComNota, PagamentosAbatendo = v.PagamentosAbatendo, Pessoas = v.Pessoas.ToList()
        };
    }

    public Task DesfazerAsync(int userId, int id, string? motivo) =>
        unitOfWork.ExecuteAsync(async () =>
        {
            var extrato = await extratos.GetByIdAsync(id, userId) ?? throw new KeyNotFoundException("Importação não encontrada.");
            if (extrato.DesfeitoEm != null)
                throw new RegraNegocioException("Esta importação já foi desfeita.");

            // O mês vem dos lançamentos em importação antiga: depois de removê-los, só a coluna guarda.
            extrato.MesReferencia ??= MesDaFatura(extrato.Itens?.Select(i => i.NumeroFatura).FirstOrDefault(n => n != null)) ?? PrimeiroDia(extrato.DataInicioPeriodo);
            await itens.RemoveByExtratoIdAsync(id);

            var texto = motivo?.Trim();
            extrato.MotivoDesfazer = string.IsNullOrEmpty(texto) ? null : texto[..Math.Min(texto.Length, TamanhoMotivo)];
            extrato.DesfeitoEm = DateTime.UtcNow;
            extrato.Situacao = "Desfeito";
            extrato.QuantidadeLancamentos = 0;
            extrato.ValorTotal = 0;
            await extratos.UpdateAsync(extrato);
        });

    /// <summary>
    /// Conta × mês (puro, para testar). "Falta" só depois da primeira importação da conta e em mês já fechado: o mês corrente
    /// ainda não tem extrato completo.
    /// </summary>
    public static CoberturaImportacoesDTO MontarCobertura(IEnumerable<ImportacaoResumo> importacoes, IReadOnlyDictionary<int, string> nomes, DateOnly hoje, int meses)
    {
        var validas = importacoes.Where(i => i.DesfeitoEm == null).Select(i => (Importacao: i, Mes: MesDe(i))).Where(x => x.Mes != null).Select(x => (x.Importacao, Mes: x.Mes!.Value)).ToList();
        var mesAtual = new DateOnly(hoje.Year, hoje.Month, 1);
        var ultimo = validas.Count > 0 && validas.Max(x => x.Mes) > mesAtual ? validas.Max(x => x.Mes) : mesAtual;
        var janela = Enumerable.Range(0, meses).Select(n => ultimo.AddMonths(n - meses + 1)).ToList();

        return new CoberturaImportacoesDTO
        {
            Meses = janela.Select(Texto).ToList(),
            Contas = validas.GroupBy(x => x.Importacao.BancoId)
                .Select(conta =>
                {
                    var primeiro = conta.Min(x => x.Mes);
                    return new CoberturaContaDTO
                    {
                        BancoId = conta.Key,
                        BancoNome = nomes.GetValueOrDefault(conta.Key) ?? "Banco não encontrado",
                        Celulas = janela.Select(mes =>
                        {
                            var doMes = conta.Where(x => x.Mes == mes).ToList();
                            var lancamentos = doMes.Sum(x => x.Importacao.Lancamentos);
                            var pendencias = doMes.Sum(x => x.Importacao.SemCategoria);
                            var estado = lancamentos > 0 ? (pendencias > 0 ? "pendencias" : "importado")
                                : mes >= primeiro && mes < mesAtual ? "falta" : "sem-dados";
                            return new CoberturaCelulaDTO { Mes = Texto(mes), Estado = estado, Lancamentos = lancamentos, Pendencias = pendencias };
                        }).ToList()
                    };
                })
                .OrderBy(c => c.BancoNome, StringComparer.CurrentCultureIgnoreCase)
                .ToList()
        };
    }

    /// <summary>Fatura vale pelo mês dela; extrato, pelo início do período.</summary>
    public static DateOnly? MesDe(ImportacaoResumo r) => r.MesReferencia ?? MesDaFatura(r.NumeroFatura) ?? PrimeiroDia(r.DataInicioPeriodo);

    public static DateOnly? MesDaFatura(string? numeroFatura) =>
        DateOnly.TryParseExact(numeroFatura, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var mes) ? mes : null;

    private static DateOnly? PrimeiroDia(DateOnly? data) => data is { } d ? new DateOnly(d.Year, d.Month, 1) : null;
    private static string Texto(DateOnly mes) => mes.ToString("yyyy-MM", CultureInfo.InvariantCulture);

    private async Task<Dictionary<int, string>> NomesDosBancosAsync(int userId) =>
        (await bancos.ListarAsync(userId)).ToDictionary(b => b.Id, b => b.NomeBanco);
}
