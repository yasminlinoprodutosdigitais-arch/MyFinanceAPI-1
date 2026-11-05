using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
public class PessoaMovimentacao : BaseEntity
{
    public string NomePessoa { get; set; } = null!;
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    /// <summary>A pessoa "Eu" (dono do cartão): uma por usuário, criada sob demanda, sem renomear nem excluir (card #45).</summary>
    public bool EhTitular { get; set; }
    /// <summary>Só as pessoas marcadas entram nos Acertos; nomes criados pela importação (mercado, Pix) nascem fora.</summary>
    public bool EntraNosAcertos { get; set; }
    public Categoria? Categoria{ get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }
    public PessoaMovimentacao() { } 
}
