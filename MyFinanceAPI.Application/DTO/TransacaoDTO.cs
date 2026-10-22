using System;
using System.ComponentModel;
using AutoMapper.Configuration.Annotations;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Json;
using System.Text.Json.Serialization;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

public class TransacaoDTO
{
    public int Id { get; set; }
    /// <summary>"YYYY-MM-DD"; aceita também instante ISO de front antigo (ver DataCalendarioJsonConverter).</summary>
    [JsonConverter(typeof(DataCalendarioJsonConverter))]
    public DateOnly Data { get; set; }
    [TextoAte(120)] public string Nome { get; set; }
    public decimal Valor { get; set; }
    public int? ContaId {get; set; }
    public string? ContaNome {get; set; }
    public int? CategoriaId {get; set; }
    public string? CategoriaNome {get; set; }
    public string? SubcategoriaNome {get; set; }
    [TextoAte(20)] public string Situacao { get; set; }
    [Ignore]
    public int? TipoContaId { get; set; }
    
    public bool EhParcelado { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    [TextoAte(500)] public string? Observacao { get; set; }

    public Conta? Conta { get; set; }

    /// <summary>
    /// Só na criação sem ContaId (conta avulsa, modal "Adicionar nova conta"): dias de vencimento da
    /// conta criada junto. Vazio: o dia de <see cref="Data"/>. Não vem do banco.
    /// </summary>
    public List<int>? DiasVencimento { get; set; }

    public TransacaoDTO()
    {
    }
}
