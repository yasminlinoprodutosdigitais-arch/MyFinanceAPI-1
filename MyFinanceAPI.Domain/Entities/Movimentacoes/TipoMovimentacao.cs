using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class TipoMovimentacao : BaseEntity
{
    public string NomeTipoMovimentacao { get; set; }
    public string Descricao { get; set; }
    public decimal ValorMeta { get; set; }
    public ICollection<ExtratoBancarioItem>? ExtratoBancarioItem { get; set; }
        
    public TipoMovimentacao() { } 
}
