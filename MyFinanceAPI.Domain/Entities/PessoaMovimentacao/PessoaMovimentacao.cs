using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class PessoaMovimentacao : BaseEntity
{
    public string NomePessoa { get; set; }
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public Categoria? Categoria{ get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }
    public PessoaMovimentacao() { } 
}
