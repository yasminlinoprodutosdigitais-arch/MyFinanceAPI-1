namespace MyFinanceAPI.Application.DTO.Movimentacoes;

/// <summary>Linha do histórico de importações (card #75).</summary>
public class ImportacaoDTO
{
    public int Id { get; set; }
    public int BancoId { get; set; }
    public string BancoNome { get; set; } = "";
    public string? NomeArquivo { get; set; }
    public DateTime ImportadoEm { get; set; }
    /// <summary>AAAA-MM; null quando não dá para saber.</summary>
    public string? MesReferencia { get; set; }
    public int Lancamentos { get; set; }
    /// <summary>Null em importação anterior ao card #75.</summary>
    public int? Duplicados { get; set; }
    public int SemCategoria { get; set; }
    public decimal Entradas { get; set; }
    public decimal Saidas { get; set; }
    /// <summary>Concluida, Pendencias ou Desfeita.</summary>
    public string Status { get; set; } = "";
    public DateTime? DesfeitoEm { get; set; }
    public string? MotivoDesfazer { get; set; }
}

public class CoberturaImportacoesDTO
{
    /// <summary>AAAA-MM, do mais antigo ao mais recente.</summary>
    public List<string> Meses { get; set; } = [];
    public List<CoberturaContaDTO> Contas { get; set; } = [];
}

public class CoberturaContaDTO
{
    public int BancoId { get; set; }
    public string BancoNome { get; set; } = "";
    public List<CoberturaCelulaDTO> Celulas { get; set; } = [];
}

public class CoberturaCelulaDTO
{
    public string Mes { get; set; } = "";
    /// <summary>importado, pendencias, falta ou sem-dados.</summary>
    public string Estado { get; set; } = "";
    public int Lancamentos { get; set; }
    public int Pendencias { get; set; }
}

public class VinculosImportacaoDTO
{
    public int Lancamentos { get; set; }
    public int Divididos { get; set; }
    public int ComNota { get; set; }
    public int PagamentosAbatendo { get; set; }
    public List<string> Pessoas { get; set; } = [];
}

public class DesfazerImportacaoDTO
{
    public string? Motivo { get; set; }
}
