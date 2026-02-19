using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class PessoaMovimentacao : BaseEntity
{
    public string NomePessoa { get; set; }
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public Category? Categoria{ get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }
    public PessoaMovimentacao() { } 

    public PessoaMovimentacao(string nomePessoa, int? categoriaId, int? tipoMovimentacaoId)
    {
        NomePessoa = nomePessoa;
        CategoriaId = categoriaId;
        TipoMovimentacaoId = tipoMovimentacaoId;
    }

    public PessoaMovimentacao(int id, string nomePessoa, int? categoriaId, int? tipoMovimentacaoId)
    {
        Id = id;
        NomePessoa = nomePessoa;
        CategoriaId = categoriaId;
        TipoMovimentacaoId = tipoMovimentacaoId;
    }

}
