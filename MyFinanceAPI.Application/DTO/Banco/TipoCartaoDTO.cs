using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;
public class TipoCartaoDTO 
{
    public int Id { get; set; }
    public string NomeTipoCartao { get; set; }
    public string? Descricao { get; set; }
    /// <summary>Agrupa por fatura em vez de data (DB-P1-07). JSON: ehCredito.</summary>
    public bool EhCredito { get; set; }
    /// <summary>Padrão do sistema (sem usuário): somente leitura na tela. JSON: ehSistema.</summary>
    public bool EhSistema { get; set; }

    public TipoCartaoDTO() { } 
}
