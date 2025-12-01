using System;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO.Extrato
{
    public class ExtratoBancarioItemDTO
    {
        public int Id { get; set; }
        public int ExtratoBancarioId { get; set; }

        public DateOnly DataMovimentacao { get; set; }
        public decimal Valor { get; set; }
        public string TipoLancamento { get; set; }

        public string? Descricao { get; set; }
        public string? NomePessoaTransacao { get; set; }
        public string? Identificador { get; set; }

        public int? BancoId { get; set; }
        public string? BancoNome { get; set; }

        public int? TipoCartaoId { get; set; }
        public string? TipoCartaoNome { get; set; }

        public int? TipoMovimentacaoId { get; set; }
        public string? TipoMovimentacaoNome { get; set; }
        public int UserId { get; set; }

        public Banco? Banco { get; set; }
        public TipoCartao? TipoCartao { get; set; }
        public TipoMovimentacao? TipoMovimentacao { get; set; }
    }
}
