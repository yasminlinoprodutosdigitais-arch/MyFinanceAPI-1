using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class Lista : BaseEntity
{
    public string Nome { get; set; }
    /// <summary>Era a coluna "TipoMovimentacao" (DB-P1-06): é o tipo da lista, não FK para TipoMovimentacao.</summary>
    public TipoLista TipoLista { get; set; }
    public bool Status { get; set; }
        
    public Lista() { } 

    public Lista(string nomeLista, TipoLista tipoLista, bool status)
    {
        Nome = nomeLista;
        TipoLista = tipoLista;
        Status = status;
    }

    public Lista(int id, string nomeLista, TipoLista tipoLista, bool status)
    {
        Id = id;
        Nome = nomeLista;
        TipoLista = tipoLista;
        Status = status;
    }

}
