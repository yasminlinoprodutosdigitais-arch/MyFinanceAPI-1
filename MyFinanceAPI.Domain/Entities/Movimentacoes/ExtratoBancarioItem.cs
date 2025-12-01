using System;

namespace MyFinanceAPI.Domain.Entities
{
    public class ExtratoBancarioItem : BaseEntity
    {
        public int ExtratoBancarioId { get; set; }
        public ExtratoBancario ExtratoBancario { get; set; }

        public DateOnly DataMovimentacao { get; set; }
        public decimal Valor { get; set; }
        public string TipoLancamento { get; set; }

        public string? Descricao { get; set; }
        public string? NomePessoaTransacao { get; set; }
        public string? Identificador { get; set; }

        public int? BancoId { get; set; }
        public Banco? Banco { get; set; }

        public int? TipoCartaoId { get; set; }
        public TipoCartao? TipoCartao { get; set; }

        public int? TipoMovimentacaoId { get; set; }
        public TipoMovimentacao? TipoMovimentacao { get; set; }
    }
}
