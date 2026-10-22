using System;
using System.ComponentModel.DataAnnotations;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class ListaDTO
{
    public int Id { get; set; }
    [TextoAte(120)] public string Nome { get; set; }
    public int TipoMovimentacao { get; set; }
    public bool Ativo { get; set; }
        
    public ListaDTO() { } 
}
