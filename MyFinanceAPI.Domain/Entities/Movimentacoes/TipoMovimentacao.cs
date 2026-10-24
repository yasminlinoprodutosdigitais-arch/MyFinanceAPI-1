using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class TipoMovimentacao : BaseEntity
{
    public string NomeTipoMovimentacao { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal ValorMeta { get; set; }
    public ICollection<ExtratoBancarioItem>? ExtratoBancarioItem { get; set; }
        
    public TipoMovimentacao() { } 
}
