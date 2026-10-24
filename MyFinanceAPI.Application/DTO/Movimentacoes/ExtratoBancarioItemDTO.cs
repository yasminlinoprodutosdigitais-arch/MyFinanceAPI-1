using System;
using System.Reflection.Metadata;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO.Extrato
{
    public class ExtratoBancarioItemDTO
    {
        public int Id { get; set; }
        public int? ExtratoBancarioId { get; set; }

        public DateOnly DataMovimentacao { get; set; }
        public decimal Valor { get; set; }
        [Obrigatorio, TextoAte(20), UmDe(TipoLancamentoExtrato.Entrada, TipoLancamentoExtrato.Saida)]
        public string TipoLancamento { get; set; } = null!;

        [TextoAte(200)] public string? Descricao { get; set; }
        [TextoAte(500)] public string? Observacao { get; set; }
        public int? PessoaMovimentacaoId { get; set; }
        [TextoAte(120)] public string? NomePessoaTransacao { get; set; }
        [TextoAte(100)] public string? Identificador { get; set; }

        public int? BancoId { get; set; }
        public string? BancoNome { get; set; }

        public int? CategoriaId { get; set; }
        [TextoAte(20)] public string? NumeroFatura { get; set; }
        public string? CategoriaNome { get; set; }

        public bool? EhParcelado { get; set; } 
        public int? ParcelaAtual { get; set; }
        public int? QuantidadeParcelas { get; set; }

        public int? TipoCartaoId { get; set; }
        public string? TipoCartaoNome { get; set; }

        public int? TipoMovimentacaoId { get; set; }
        public string? TipoMovimentacaoNome { get; set; }
        // Sem UsuarioId: o dono vem só do token (BE-P4-17).
        [TextoAte(200)] public string? ChaveDescricao { get; set; }
        [TextoAte(64)] public string? ChaveImportacao { get; set; }

        public bool AlteraVinculoPessoa { get; set; }


        public BancoDTO? Banco { get; set; }
        public TipoCartaoDTO? TipoCartao { get; set; }
        public TipoMovimentacaoDTO? TipoMovimentacao { get; set; }
        public CategoriaDTO? Categoria { get; set; }
        public PessoaMovimentacaoDTO? PessoaMovimentacao { get; set; }
        
    }
}
