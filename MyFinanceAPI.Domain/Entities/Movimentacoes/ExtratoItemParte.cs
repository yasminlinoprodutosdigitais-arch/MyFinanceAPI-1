namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Parte de um lançamento dividido (card #76, fase 3): R$ 300 no mercado viram R$ 200 em Alimentação e R$ 100 em Beleza do
/// Carlos. No extrato continua um lançamento só; relatórios e acertos olham as partes. Sem pessoa, a parte é do dono.
/// </summary>
public class ExtratoItemParte : BaseEntity
{
    public int ExtratoBancarioItemId { get; set; }
    public ExtratoBancarioItem? ExtratoBancarioItem { get; set; }
    public decimal Valor { get; set; }
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }
    public int? PessoaMovimentacaoId { get; set; }
    public PessoaMovimentacao? PessoaMovimentacao { get; set; }
    public string? Descricao { get; set; }
    public int Ordem { get; set; }
}
