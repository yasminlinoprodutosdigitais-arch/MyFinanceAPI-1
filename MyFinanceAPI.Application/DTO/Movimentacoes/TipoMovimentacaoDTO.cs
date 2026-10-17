using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
public class TipoMovimentacaoDTO
{
    public int Id { get; set; }
    public string NomeTipoMovimentacao { get; set; }
    public string Descricao { get; set; }
    public decimal ValorMeta { get; set; }
    public TipoMovimentacaoDTO() { } 
}
