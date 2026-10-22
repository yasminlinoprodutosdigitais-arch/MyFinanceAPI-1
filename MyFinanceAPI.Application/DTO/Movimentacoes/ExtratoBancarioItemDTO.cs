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
        [TextoAte(20)] public string TipoLancamento { get; set; }

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
        public int UsuarioId { get; set; }
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
