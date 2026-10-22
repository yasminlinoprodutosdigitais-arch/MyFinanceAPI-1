using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceAPI.Domain.Entities;
public class Lista : BaseEntity
{
    public string Nome { get; set; }
    /// <summary>Era a coluna "TipoMovimentacao" (DB-P1-06): é o tipo da lista, não FK para TipoMovimentacao.</summary>
    public TipoLista TipoLista { get; set; }
    public bool Ativo { get; set; }
        
    public Lista() { } 
}
