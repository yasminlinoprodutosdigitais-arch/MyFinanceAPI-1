using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
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

    public TipoCartaoDTO(string? nomeTipoCartao, string? descricao)
    {
        NomeTipoCartao = nomeTipoCartao;
        Descricao = descricao;
    }

    public TipoCartaoDTO(int id, string nomeTipoCartao, string descricao)
    {
        Id = id;
        NomeTipoCartao = nomeTipoCartao;
        Descricao = descricao;
    }
}
