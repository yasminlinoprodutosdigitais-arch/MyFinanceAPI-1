using System;
using System.ComponentModel;
using AutoMapper.Configuration.Annotations;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Json;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Application.DTO;

public class TransactionDTO
{
    public int Id { get; set; }
    /// <summary>"YYYY-MM-DD"; aceita também instante ISO de front antigo (ver DataCalendarioJsonConverter).</summary>
    [JsonConverter(typeof(DataCalendarioJsonConverter))]
    public DateOnly Date { get; set; }
    public string Name { get; set; }
    public decimal Value { get; set; }
    public int? IdAccount {get; set; }
    public string? AccountName {get; set; }
    public int? CategoryId {get; set; }
    public string? CategoryName {get; set; }
    public string? SubCategoryName {get; set; }
    public string Status { get; set; }
    [Ignore]
    public int? TipoContaId { get; set; }
    
    public bool EhParcelado { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public string? Observacao { get; set; }

    public Account? Account { get; set; }

    public TransactionDTO()
    {
    }
}
