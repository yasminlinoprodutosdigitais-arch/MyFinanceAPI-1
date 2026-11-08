using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO.Extrato;

/// <summary>Parte de um lançamento dividido (card #76, fase 3). Sem pessoa, a parte é do dono.</summary>
public class ExtratoItemParteDTO
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    /// <summary>Natureza da categoria, para o relatório contar a parte (achatado de Categoria.NaturezaOperacao).</summary>
    public int? CategoriaNaturezaOperacao { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public int? PessoaMovimentacaoId { get; set; }
    public string? PessoaNome { get; set; }
    [TextoAte(200)] public string? Descricao { get; set; }
}
