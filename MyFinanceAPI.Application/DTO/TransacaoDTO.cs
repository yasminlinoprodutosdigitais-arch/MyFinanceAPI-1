using System;
using System.ComponentModel;
using AutoMapper.Configuration.Annotations;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Json;
using System.Text.Json.Serialization;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

public class TransacaoDTO
{
    public int Id { get; set; }
    /// <summary>"YYYY-MM-DD"; aceita também instante ISO de front antigo (ver DataCalendarioJsonConverter).</summary>
    [JsonConverter(typeof(DataCalendarioJsonConverter))]
    public DateOnly Data { get; set; }
    [Obrigatorio, TextoAte(120)] public string Nome { get; set; } = null!;
    public decimal Valor { get; set; }
    public int? ContaId {get; set; }
    public string? ContaNome {get; set; }
    public int? CategoriaId {get; set; }
    public string? CategoriaNome {get; set; }
    public string? SubcategoriaNome {get; set; }
    [Obrigatorio, TextoAte(20)]
    [UmDe(StatusTransacao.Pendente, StatusTransacao.Aguardando, StatusTransacao.PagoNoPrazo, StatusTransacao.PagoAtrasado)]
    public string Situacao { get; set; } = null!;
    [Ignore]
    public int? TipoContaId { get; set; }
    
    public bool EhParcelado { get; set; }
    public int? ParcelaAtual { get; set; }
    public int? QuantidadeParcelas { get; set; }
    [TextoAte(500)] public string? Observacao { get; set; }

    public Conta? Conta { get; set; }

    /// <summary>
    /// Na criação. Sem ContaId (conta avulsa, modal "Adicionar nova conta"): dias de vencimento da
    /// conta criada junto; vazio, o dia de <see cref="Data"/>. Com ContaId (lote, card #48): dias só deste
    /// lançamento, no lugar dos da conta, sem mudar o cadastro. Não vem do banco.
    /// </summary>
    [DiasDoMes] public List<int>? DiasVencimento { get; set; }

    /// <summary>
    /// Só na criação com ContaId (lote, card #48): valor deste mês no lugar do valor da conta, dividido
    /// entre os vencimentos como o dela; o cadastro não muda. Ausente: o valor da conta. Não vem do banco.
    /// </summary>
    public decimal? ValorDoMes { get; set; }

    /// <summary>
    /// Só na criação sem ContaId (card #47): false = lançamento avulso daquele mês, sem cadastrar a conta;
    /// ausente ou true = cria a conta junto, como antes. Não vem do banco.
    /// </summary>
    public bool? CadastrarConta { get; set; }

    public TransacaoDTO()
    {
    }
}
