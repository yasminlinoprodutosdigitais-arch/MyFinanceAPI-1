using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyFinanceAPI.Domain.Entities;
public class Banco : BaseEntity
{
    public string NomeBanco { get; set; }
    public string? Agencia { get; set; }
    public string? NumeroConta { get; set; }
    public int TipoCartaoId { get; set; }
    public decimal? SaldoInicial { get; set; }
    public bool Ativo { get; set; }


    public Banco() { } 

    public Banco(string? nomeBanco, string? numeroConta, int tipoCartaoId, decimal? saldoInicial, bool ativo)
    {
        NomeBanco = nomeBanco;
        NumeroConta = numeroConta;
        TipoCartaoId = tipoCartaoId;
        SaldoInicial = saldoInicial;
        Ativo = ativo;
    }

    public Banco(int id, string nomeBanco, string numeroConta, int tipoCartaoId, decimal? saldoInicial, bool ativo)
    {
        Id = id;
        NomeBanco = nomeBanco;
        NumeroConta = numeroConta;
        TipoCartaoId = tipoCartaoId;
        SaldoInicial = saldoInicial;
        Ativo = ativo;
    }
}
