using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

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

        [TextoAte(200)] public string? Descricao { get; set; }

        [Obrigatorio, TextoAte(20)] public string TipoLancamento { get; set; } = string.Empty;

        [TextoAte(120)] public string? NomePessoaTransacao { get; set; }

        // Navegação (opcional)
        public Banco? Banco { get; set; }
        public TipoCartao? TipoCartao { get; set; }
        public TipoMovimentacao? TipoMovimentacao { get; set; }

        [TextoAte(100)] public string? Identificador { get; set; }

        public MovimentacaoDiariaDTO() { }
    }
}
