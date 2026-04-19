using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

    public PessoaMovimentacaoDTO(string nomePessoa, int? categoriaId, int? tipoMovimentacaoId)
    {
        NomePessoa = nomePessoa;
        CategoriaId = categoriaId;
        TipoMovimentacaoId = tipoMovimentacaoId;
    }

    public PessoaMovimentacaoDTO(int id, string nomePessoa, int? categoriaId, int? tipoMovimentacaoId)
    {
        Id = id;
        NomePessoa = nomePessoa;
        CategoriaId = categoriaId;
        TipoMovimentacaoId = tipoMovimentacaoId;
    }
    public PessoaMovimentacaoDTO(int id, string nomePessoa, int? categoriaId, int? tipoMovimentacaoId, string mes)
    {
        Id = id;
        NomePessoa = nomePessoa;
        CategoriaId = categoriaId;
        TipoMovimentacaoId = tipoMovimentacaoId;
        MesAtualizacao = mes;
    }

}
