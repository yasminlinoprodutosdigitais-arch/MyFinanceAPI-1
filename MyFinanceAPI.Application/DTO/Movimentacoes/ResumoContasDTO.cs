namespace MyFinanceAPI.Application.DTO.Movimentacoes;

/// <summary>
/// Card #77: os números de cada conta no mês, iguais no card da listagem e no topo do detalhe, e os totais gerais.
/// Entradas e Saídas gerais tiram pagamento de fatura e transferências (a mesma regra do Relatório).
/// </summary>
public class ResumoContasDTO
{
    public string Mes { get; set; } = "";
    public decimal Entradas { get; set; }
    public decimal Saidas { get; set; }
    /// <summary>Lançamentos sem categoria no mês, em todas as contas.</summary>
    public int ParaClassificar { get; set; }
    /// <summary>Soma do que falta receber nos cartões compartilhados; null quando nenhum cartão é compartilhado.</summary>
    public decimal? FaltaReceber { get; set; }
    public List<ResumoContaDTO> Contas { get; set; } = [];
}

public class ResumoContaDTO
{
    public int BancoId { get; set; }
    public string Nome { get; set; } = "";
    public string TipoNome { get; set; } = "";
    public bool EhCredito { get; set; }
    public bool Ativo { get; set; }
    public string? Agencia { get; set; }
    public string? NumeroConta { get; set; }
    public bool Compartilhado { get; set; }
    public bool ConviteCompartilharDispensado { get; set; }
    /// <summary>Quem usa o cartão (cadastro), em ordem de nome.</summary>
    public List<string> Pessoas { get; set; } = [];

    public int Quantidade { get; set; }
    public decimal Entradas { get; set; }
    public decimal Saidas { get; set; }
    public decimal Resultado { get; set; }
    public int SemCategoria { get; set; }

    /// <summary>Cartão: total das compras da fatura (minhas + de outras pessoas).</summary>
    public decimal Fatura { get; set; }
    /// <summary>Cartão: entradas na fatura (pagamentos e estornos).</summary>
    public decimal PagamentosEstornos { get; set; }
    /// <summary>Cartão: o que as compras parceladas desta fatura ainda vão cobrar nas próximas.</summary>
    public decimal ParcelasQueContinuam { get; set; }
    public decimal MinhasCompras { get; set; }
    public decimal DeOutrasPessoas { get; set; }
    public List<ResumoContaPessoaDTO> PorPessoa { get; set; } = [];
    /// <summary>Das compras deste mês, o que ainda falta receber nos Acertos.</summary>
    public decimal FaltaReceber { get; set; }

    /// <summary>Importação mais recente que trouxe lançamentos deste mês.</summary>
    public DateTime? ImportadoEm { get; set; }
    public DateTime? UltimaImportacao { get; set; }
    /// <summary>Extrato ou fatura do mês que já deveria ter sido importado (fechamento ou regra da Cobertura).</summary>
    public bool FaltaImportar { get; set; }
    public DateOnly? DataFechamento { get; set; }
    public DateOnly? DataVencimento { get; set; }
}

public class ResumoContaPessoaDTO
{
    public int PessoaId { get; set; }
    public string Nome { get; set; } = "";
    public decimal Valor { get; set; }
}

/// <summary>Switch "Outras pessoas usam este cartão" do detalhe (card #77). Campo null = não muda.</summary>
public class CompartilhamentoDTO
{
    public bool? Compartilhado { get; set; }
    public List<int>? PessoasIds { get; set; }
    public bool? ConviteDispensado { get; set; }
}
