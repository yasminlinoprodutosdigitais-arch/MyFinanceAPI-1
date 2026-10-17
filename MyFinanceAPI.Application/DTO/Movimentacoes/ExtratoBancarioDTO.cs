using System;
using System.Collections.Generic;

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
        public string Situacao { get; set; }

        public string? NomeArquivoOrigem { get; set; }

        public List<ExtratoBancarioItemDTO> Itens { get; set; }
    }
}
