using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;
/// <summary>
/// Quem usou o cartão ("Carlos"), cadastrado à mão; toda pessoa que não é o "Eu" entra nos Acertos. O nome que vem no
/// extrato ("CIRCUITO") não é pessoa: ele fica no lançamento e é classificado pelas regras (card #79).
/// </summary>
public class PessoaMovimentacao : BaseEntity
{
    public string NomePessoa { get; set; } = null!;
    public int? CategoriaId { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    /// <summary>A pessoa "Eu" (dono do cartão): uma por usuário, criada sob demanda, sem renomear nem excluir (card #45).</summary>
    public bool EhTitular { get; set; }
    public Categoria? Categoria{ get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }
    public PessoaMovimentacao() { } 
}
