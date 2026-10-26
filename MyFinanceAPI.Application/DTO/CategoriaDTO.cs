using System;
using System.Text.Json.Serialization;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

public class CategoriaDTO
{
    public int Id { get; set; }
    [Obrigatorio, TextoAte(120)] public string Nome { get; set; } = null!;
    [Obrigatorio, TextoAte(120)] public string Subcategoria { get; set; } = null!;
    [Faixa(0, 5)] public int NaturezaOperacao { get; set; }
    public bool Ativo { get; set; }
    [TextoAte(40)] public string? Icone { get; set; }
    [TextoAte(20)] public string? Cor { get; set; }
    public DateTime DataAlteracao { get; set;}

    /// <summary>Só leitura: quantas contas usam a categoria (a tela avisa antes de inativar ou excluir).</summary>
    public int QuantidadeContas { get; set; }
    
    [JsonIgnore]
    public ICollection<Conta>? Contas { get; set; }

    public CategoriaDTO() { }
}