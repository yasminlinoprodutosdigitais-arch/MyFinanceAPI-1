using System;

namespace MyFinanceAPI.Domain.Entities
{
    public class ExtratoBancarioItem : BaseEntity
    {
        public int? ExtratoBancarioId { get; set; }
        public ExtratoBancario? ExtratoBancario { get; set; }

        public DateOnly DataMovimentacao { get; set; }
        public decimal Valor { get; set; }
        public string TipoLancamento { get; set; } = null!;

        public string? Descricao { get; set; }
        public string? Observacao { get; set; }
        public int? PessoaMovimentacaoId { get; set; }
        /// <summary>
        /// Snapshot intencional (DB-P1-08): o nome como veio no extrato importado ou foi digitado
        /// no lançamento manual. A pessoa normalizada é o vínculo PessoaMovimentacaoId; renomear a
        /// pessoa não reescreve este texto.
        /// </summary>
        public string? NomePessoaTransacao { get; set; }
        public string? Identificador { get; set; }

        public bool? EhParcelado { get; set; }
        public int? ParcelaAtual { get; set; }
        public int? QuantidadeParcelas { get; set; }

        public int? BancoId { get; set; }
        public Banco? Banco { get; set; }

        public int? TipoCartaoId { get; set; }
        public TipoCartao? TipoCartao { get; set; }

        public int? CategoriaId { get; set; }
        public string? NumeroFatura { get; set; }
        public Categoria? Categoria { get; set; }

        public int? TipoMovimentacaoId { get; set; }
        public TipoMovimentacao? TipoMovimentacao { get; set; }
        public string? ChaveDescricao { get; set; }
        /// <summary>
        /// Card #79: a categoria veio de uma regra com nome parecido (não igual). Some quando a categoria é trocada.
        /// </summary>
        public bool ClassificadoPorSemelhanca { get; set; }
        /// <summary>
        /// Hash da linha do arquivo importado (idempotência da importação). Gravado só na importação,
        /// nunca editado; nulo em itens lançados à mão.
        /// </summary>
        public string? ChaveImportacao { get; set; }
        /// <summary>
        /// Pagamento de uma pessoa (Entrada) que abate uma compra dela (Saída) — card #45, parte 3. Nulo = abate o saldo
        /// geral da pessoa. Se a compra for apagada, volta a nulo.
        /// </summary>
        public int? AbateItemId { get; set; }
        public PessoaMovimentacao? PessoaMovimentacao { get; set; }
        /// <summary>Partes do lançamento dividido (card #76, fase 3); vazio = não dividido.</summary>
        public List<ExtratoItemParte> Partes { get; set; } = new();
        public bool Dividido => Partes.Count > 0;
        /// <summary>Nota fiscal lançada neste lançamento (card #76, fase 6).</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public NotaFiscal? NotaFiscal { get; set; }
    }

}
