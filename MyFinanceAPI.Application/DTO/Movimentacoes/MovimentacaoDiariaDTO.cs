using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.DTO.Movimentacoes
{
    public class MovimentacaoDiariaDTO
    {
        public int Id { get; set; }

        public DateOnly DataMovimentacao { get; set; }

        public int BancoId { get; set; }

        public int? TipoCartaoId { get; set; }

        // 🔹 agora nullable
        public int? TipoMovimentacaoId { get; set; }

        public decimal Valor { get; set; }

        public string? Descricao { get; set; }

        public string TipoLancamento { get; set; } = string.Empty;

        public string? NomePessoaTransacao { get; set; }

        // Navegação (opcional)
        public Banco? Banco { get; set; }
        public TipoCartao? TipoCartao { get; set; }
        public TipoMovimentacao? TipoMovimentacao { get; set; }

        public string? Identificador { get; set; }

        public MovimentacaoDiariaDTO() { }
    }
}
