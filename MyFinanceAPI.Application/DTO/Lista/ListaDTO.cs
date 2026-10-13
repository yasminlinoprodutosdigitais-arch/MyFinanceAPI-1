using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class ListaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int TipoMovimentacao { get; set; }
    public bool Status { get; set; }
        
    public ListaDTO() { } 
}
