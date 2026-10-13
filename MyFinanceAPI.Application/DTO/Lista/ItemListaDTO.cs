using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class ItemListaDTO
{
    public int Id { get; set; }
    public int ListaId { get; set; }
    public string Descricao { get; set; }
    public int? Quantidade { get; set; }
    public string? Observacao { get; set; }
    public string? Status { get; set; }
    public decimal? Valor { get; set; }
    public DateOnly? DataTarefa { get; set; }
    public TimeOnly? HorarioTarefa { get; set; }

    public Lista? Lista { get; set; }  
        
    public ItemListaDTO() { } 
}
