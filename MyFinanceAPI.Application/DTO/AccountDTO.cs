using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
public class AccountDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Value { get; set; }
    public int Categoryid { get; set; }
    public int Status { get; set; }
    public bool EhParcelado { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public int? ParcelaAtual { get; set; }
    public List<int> DataOperacao { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
    public ICollection<ContaVencimento>? ContaVencimentos { get; set; }

    public AccountDTO() { }

    public AccountDTO(string name, decimal value, int categoryId, int status, bool ehParcelada, int quantidadeParcelas, int parcelaAtual,
                      ICollection<Transaction>? updates, List<int>? dataOperacao )
    {
        Name = name;
        Value = value;
        Categoryid = categoryId;
        Status = status;
        EhParcelado = ehParcelada;
        QuantidadeParcelas = quantidadeParcelas;
        ParcelaAtual = parcelaAtual;
        Transactions = updates;
        DataOperacao = dataOperacao;   // <<< conversão segura
    }

    public AccountDTO(int id, string name, decimal value, int categoryId, int status, bool ehParcelada, int quantidadeParcelas, int parcelaAtual,
                      ICollection<Transaction>? updates, List<int>? dataOperacao)
    {
        Id = id;
        Name = name;
        Value = value;
        Categoryid = categoryId;
        Status = status;
        EhParcelado = ehParcelada;
        QuantidadeParcelas = quantidadeParcelas;
        ParcelaAtual = parcelaAtual;
        Transactions = updates;
        DataOperacao = dataOperacao;   // <<< conversão segura
    }
}
