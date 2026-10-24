using System;
using System.Collections.Generic;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO.Extrato
{
    public class ExtratoBancarioDTO
    {
        public int Id { get; set; }

        public DateTime DataImportacao { get; set; }

        public DateOnly? DataInicioPeriodo { get; set; }
        public DateOnly? DataFimPeriodo { get; set; }

        public int BancoId { get; set; }
        public string? BancoNome { get; set; }

        public int? TipoCartaoId { get; set; }

        public int QuantidadeLancamentos { get; set; }
        public decimal ValorTotal { get; set; }

        public Guid LoteImportacaoId { get; set; }
        [TextoAte(20)] public string Situacao { get; set; } = null!;

        [TextoAte(260)] public string? NomeArquivoOrigem { get; set; }

        public List<ExtratoBancarioItemDTO> Itens { get; set; } = null!;
    }
}
