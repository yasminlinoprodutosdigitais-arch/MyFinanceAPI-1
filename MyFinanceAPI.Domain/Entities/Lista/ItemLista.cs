using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace MyFinanceAPI.Domain.Entities;
public class ItemLista : BaseEntity
{
    public int ListaId { get; set; }
    public string Descricao { get; set; } = null!;
    public int? Quantidade { get; set; }
    public string? Observacao { get; set; }
    public string? Situacao { get; set; }
    public decimal? Valor { get; set; }
    public DateOnly? DataTarefa { get; set; }
    public TimeOnly? HorarioTarefa { get; set; }

    public Lista? Lista { get; set; }  
        
    public ItemLista() { } 
}
