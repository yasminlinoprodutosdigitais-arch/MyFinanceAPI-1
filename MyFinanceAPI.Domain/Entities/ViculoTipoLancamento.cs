using System;

namespace MyFinanceAPI.Domain.Entities;
public class VinculoTipoMovimentacao : BaseEntity
{
    public int UserId { get; set; }

    public string ChaveDescricao { get; set; } = string.Empty;
    public string? DescricaoOriginalExemplo { get; set; }

    public int? TipoMovimentacaoId { get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }

    public int? CategoriaId { get; set; }
    public Category? Categoria { get; set; }

    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataUltimoUso { get; set; }
}
