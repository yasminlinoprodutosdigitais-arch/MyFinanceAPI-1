using System;
using System.ComponentModel.DataAnnotations;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO;

public class VinculoCreateDTO
{
    public string ChaveDescricao { get; set; }
    public string DescricaoOriginalExemplo { get; set; }
}
public class VinculoUpdateDTO
{
    public int TipoMovimentacaoId { get; set; }
    public int? CategoriaId { get; set; }


    public bool AplicarRetroativamente { get; set; } // atualizar itens antigos
}

public class VinculoTipoMovimentacaoDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string ChaveDescricao { get; set; }
    public string? DescricaoOriginalExemplo { get; set; }

    public int? TipoMovimentacaoId { get; set; }

    public TipoMovimentacaoDTO? TipoMovimentacao { get; set; }
    
    public int? CategoriaId { get; set; }
    
    public CategoryDTO? Categoria { get; set; }

    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimoUso { get; set; }
}
