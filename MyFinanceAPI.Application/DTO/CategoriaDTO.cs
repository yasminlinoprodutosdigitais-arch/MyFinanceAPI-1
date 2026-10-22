using System;
using System.Text.Json.Serialization;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

public class CategoriaDTO
{
    public int Id { get; set; }
    [TextoAte(120)] public string Nome { get; set; }
    [TextoAte(120)] public string Subcategoria { get; set; }
    public int NaturezaOperacao { get; set; }
    public int Ativo { get; set; }
    public DateTime DataAlteracao { get; set;}
    
    [JsonIgnore]
    public ICollection<Conta>? Contas { get; set; }

    public CategoriaDTO() { }
}