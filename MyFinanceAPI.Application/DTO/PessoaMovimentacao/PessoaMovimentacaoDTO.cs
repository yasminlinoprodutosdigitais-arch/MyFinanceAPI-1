using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class PessoaMovimentacaoDTO
{
    public int Id { get; set; }
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    [Obrigatorio, TextoAte(120)] public string NomePessoa { get; set; } = null!;
    public string? MesAtualizacao{ get; set; }

    public Categoria? Categoria{ get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }

    public PessoaMovimentacaoDTO() { } 
}
