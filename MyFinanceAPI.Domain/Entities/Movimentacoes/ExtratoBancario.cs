using System;
using System.Collections.Generic;

namespace MyFinanceAPI.Domain.Entities
{
    public class ExtratoBancario : BaseEntity
    {
        public DateTime DataImportacao { get; set; }

        public DateOnly? DataInicioPeriodo { get; set; }
        public DateOnly? DataFimPeriodo { get; set; }

        public int BancoId { get; set; }
        public int? TipoCartaoId { get; set; }

        public int QuantidadeLancamentos { get; set; }
        public decimal ValorTotal { get; set; }

        public Guid LoteImportacaoId { get; set; }

        public string? NomeArquivoOrigem { get; set; }
        public string Situacao { get; set; } = "Concluido";

        // Relação reversa
        public ICollection<ExtratoBancarioItem> Itens { get; set; }
    }
}
