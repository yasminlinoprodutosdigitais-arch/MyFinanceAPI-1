using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
public class ListaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int TipoMovimentacao { get; set; }
    public bool Status { get; set; }
        
    public ListaDTO() { } 
}
