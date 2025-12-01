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

        public MovimentacaoDiariaDTO(
            DateOnly dataMovimentacao,
            int bancoId,
            int? tipoCartaoId,
            int? tipoMovimentacaoId,
            decimal valor,
            string? descricao,
            string tipoLancamento,
            string? nomePessoaTransacao = null,
            string? identificador = null)
        {
            DataMovimentacao = dataMovimentacao;
            BancoId = bancoId;
            TipoCartaoId = tipoCartaoId;
            TipoMovimentacaoId = tipoMovimentacaoId;
            Valor = valor;
            Descricao = descricao;
            TipoLancamento = tipoLancamento;
            NomePessoaTransacao = nomePessoaTransacao;
            Identificador = identificador;
        }

        public MovimentacaoDiariaDTO(
            int id,
            DateOnly dataMovimentacao,
            int bancoId,
            int? tipoCartaoId,
            int? tipoMovimentacaoId,
            decimal valor,
            string? descricao,
            string tipoLancamento,
            string? nomePessoaTransacao = null,
            string? identificador = null)
        {
            Id = id;
            DataMovimentacao = dataMovimentacao;
            BancoId = bancoId;
            TipoCartaoId = tipoCartaoId;
            TipoMovimentacaoId = tipoMovimentacaoId;
            Valor = valor;
            Descricao = descricao;
            TipoLancamento = tipoLancamento;
            NomePessoaTransacao = nomePessoaTransacao;
            Identificador = identificador;
        }
    }
}
