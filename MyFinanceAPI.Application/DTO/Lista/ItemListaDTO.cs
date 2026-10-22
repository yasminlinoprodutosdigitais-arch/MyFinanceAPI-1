using System;
using System.ComponentModel.DataAnnotations;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class ItemListaDTO
{
    public int Id { get; set; }
    public int ListaId { get; set; }
    [TextoAte(200)] public string Descricao { get; set; }
    public int? Quantidade { get; set; }
    [TextoAte(500)] public string? Observacao { get; set; }
    [TextoAte(20)] public string? Situacao { get; set; }
    public decimal? Valor { get; set; }
    public DateOnly? DataTarefa { get; set; }
    public TimeOnly? HorarioTarefa { get; set; }

    public Lista? Lista { get; set; }  
        
    public ItemListaDTO() { } 
}
