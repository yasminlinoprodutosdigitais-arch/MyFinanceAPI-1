using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
public class AccountDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Value { get; set; }
    public int Categoryid { get; set; }
    [NotMapped]
    public string? CategoryName { get; set; }
    public int Status { get; set; }
    public bool EhParcelado { get; set; }
    public int? QuantidadeParcelas { get; set; }
    public int? ParcelaAtual { get; set; }
    public List<int> DataOperacao { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
    public ICollection<ContaVencimento>? ContaVencimentos { get; set; }

    public AccountDTO() { }
}
