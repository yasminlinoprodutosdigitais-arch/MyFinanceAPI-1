namespace MyFinanceAPI.Domain.Entities;

/// <summary>Uma importação com os totais dos lançamentos (card #75).</summary>
public record ImportacaoResumo(
    int Id, int BancoId, DateTime ImportadoEm, string? NomeArquivo, DateOnly? MesReferencia, int? Duplicados,
    DateTime? DesfeitoEm, string? MotivoDesfazer, DateOnly? DataInicioPeriodo, int Lancamentos,
    decimal Entradas, decimal Saidas, int SemCategoria, string? NumeroFatura);

/// <summary>O que uma importação leva junto ao ser desfeita (card #75).</summary>
public record VinculosImportacao(int Lancamentos, int Divididos, int ComNota, int PagamentosAbatendo, IReadOnlyList<string> Pessoas);
