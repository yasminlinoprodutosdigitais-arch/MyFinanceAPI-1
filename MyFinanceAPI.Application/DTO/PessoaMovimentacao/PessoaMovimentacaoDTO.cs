using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class PessoaMovimentacaoDTO
{
    public int Id { get; set; }
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public string NomePessoa { get; set; }
    public string? MesAtualizacao{ get; set; }

    public Category? Categoria{ get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }

    public PessoaMovimentacaoDTO() { } 
}
