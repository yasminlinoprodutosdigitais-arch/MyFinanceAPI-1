using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;
public class TipoMovimentacaoDTO
{
    public int Id { get; set; }
    [Obrigatorio, TextoAte(60)] public string NomeTipoMovimentacao { get; set; } = null!;
    [Obrigatorio, TextoAte(200)] public string Descricao { get; set; } = null!;
    [Minimo(0)] public decimal ValorMeta { get; set; }
    public TipoMovimentacaoDTO() { } 
}
