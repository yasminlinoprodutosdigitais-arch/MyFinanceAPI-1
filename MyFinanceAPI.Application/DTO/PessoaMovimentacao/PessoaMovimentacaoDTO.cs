using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
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
