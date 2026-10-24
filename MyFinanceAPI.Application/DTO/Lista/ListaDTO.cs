using System;
using System.ComponentModel.DataAnnotations;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class ListaDTO
{
    public int Id { get; set; }
    [Obrigatorio, TextoAte(120)] public string Nome { get; set; } = null!;
    [UmDe((int)TipoLista.Checklist, (int)TipoLista.Cronograma, (int)TipoLista.Orcamento)]
    public int TipoMovimentacao { get; set; }
    public bool Ativo { get; set; }
        
    public ListaDTO() { } 
}
