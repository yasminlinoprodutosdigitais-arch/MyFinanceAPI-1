using System.Text.Json.Serialization;

namespace MyFinanceAPI.Domain.Entities;

/// <summary>Pessoa que usa um cartão compartilhado (card #77). O dono vem pelo banco.</summary>
public class BancoPessoa
{
    public int BancoId { get; set; }
    public int PessoaMovimentacaoId { get; set; }

    [JsonIgnore] public Banco Banco { get; set; } = null!;
    [JsonIgnore] public PessoaMovimentacao PessoaMovimentacao { get; set; } = null!;
}
