using System;
using System.ComponentModel;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class TransactionDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public int? IdAccount {get; set; }
    public int? CategoryId {get; set; }
    public string Status { get; set; }
    
    public bool EhParcelado { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public string? Observacao { get; set; }

    public Account? Account { get; set; }


    public TransactionDTO()
    {
        
    }

    public TransactionDTO(DateTime date, string name, double value,int? idAccount, int? categoryId, string status, bool ehParcelado, int? parcelaAtual, int? quantidadeParcelas, string? observacao)
    {
        Date = date;
        Name = name;
        Value = value;
        IdAccount = idAccount;
        CategoryId = categoryId;
        Status = status;
        EhParcelado = ehParcelado;
        ParcelaAtual = parcelaAtual;
        QuantidadeParcelas = quantidadeParcelas;
        Observacao = observacao;
    }

    public TransactionDTO(int id, DateTime date, string name, int idAccount, int? categoryId, double value, string status, bool ehParcelado, int? parcelaAtual, int? quantidadeParcelas, string? observacao)
    {
        Id = id;
        Date = date;
        Name = name;
        IdAccount = idAccount;
        CategoryId = categoryId;
        Value = value;
        Status = status;
        EhParcelado = ehParcelado;
        ParcelaAtual = parcelaAtual;
        QuantidadeParcelas = quantidadeParcelas;
        Observacao = observacao;
    } 
    
}
