namespace MyFinanceAPI.Application.DTO.Movimentacoes;

/// <summary>O que a importação faria, sem gravar nada (card #74).</summary>
public class PreviaImportacaoDTO
{
    /// <summary>Formato lido do conteúdo (ex.: "Nubank", "Fatura Caixa").</summary>
    public string Formato { get; set; } = "";
    public bool EhFatura { get; set; }
    /// <summary>Banco usado na prévia; null quando não foi informado nem reconhecido pelo formato (a tela pede).</summary>
    public int? BancoId { get; set; }
    /// <summary>true quando o banco veio do formato do arquivo, não da escolha da pessoa.</summary>
    public bool BancoDetectado { get; set; }
    /// <summary>AAAA-MM: o da fatura ou o do começo do extrato.</summary>
    public string? MesReferencia { get; set; }
    public DateOnly? DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public int Novos { get; set; }
    public int Duplicados { get; set; }
    public int Invalidas { get; set; }
    /// <summary>Soma dos novos.</summary>
    public decimal Entradas { get; set; }
    public decimal Saidas { get; set; }
    /// <summary>O banco já tem importação (não desfeita) neste mês: só os novos entram.</summary>
    public bool MesJaImportado { get; set; }
    public string Aviso { get; set; } = "";
    public List<LinhaPreviaImportacaoDTO> Linhas { get; set; } = [];
}

public class LinhaPreviaImportacaoDTO
{
    /// <summary>Chave de idempotência: identifica a linha ao confirmar.</summary>
    public string Chave { get; set; } = "";
    public DateOnly Data { get; set; }
    public string Descricao { get; set; } = "";
    public decimal Valor { get; set; }
    public string TipoLancamento { get; set; } = "";
    /// <summary>Sugerida por encargo, regra ou padrão da pessoa; null = sem categoria.</summary>
    public int? CategoriaId { get; set; }
    public string? NomePessoa { get; set; }
    /// <summary>Pessoa cadastrada que entra nos acertos (não o titular).</summary>
    public bool EntraNosAcertos { get; set; }
    /// <summary>Já importado antes (ou repetido no arquivo): não entra.</summary>
    public bool Duplicado { get; set; }
}

/// <summary>Escolhas feitas na prévia, enviadas ao confirmar (card #74).</summary>
public class OpcoesImportacao
{
    /// <summary>Chaves das linhas desmarcadas.</summary>
    public HashSet<string> Pular { get; set; } = [];
    /// <summary>Categoria escolhida por chave; null = sem categoria.</summary>
    public Dictionary<string, int?> Categorias { get; set; } = [];
}
