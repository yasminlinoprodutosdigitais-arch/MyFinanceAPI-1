using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class ContaDTO
{
    public int Id { get; set; }
    [TextoAte(120)] public string? Nome { get; set; }
    public decimal Valor { get; set; }
    public int CategoriaId { get; set; }
    [NotMapped]
    public string? CategoriaNome { get; set; }
    public bool Ativo { get; set; }
    public bool EhParcelado { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public int? ParcelaAtual { get; set; }
    [Obrigatorio, DiasDoMes] public List<int> DataOperacao { get; set; } = null!;
    public ICollection<Transacao>? Transacoes { get; set; }
    public ICollection<ContaVencimento>? Vencimentos { get; set; }

    public ContaDTO() { }
}
