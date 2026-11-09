namespace MyFinanceAPI.Application.DTO.Extrato
{
    public class ExtratoImportacaoResultadoDTO
    {
        public int ExtratoId { get; set; }
        public int QuantidadeCriados { get; set; }
        public int QuantidadeIgnorados { get; set; }
        public decimal ValorTotal { get; set; }
        public string Mensagem { get; set; } = null!;
        /// <summary>Mês (AAAA-MM) em que os lançamentos aparecem: o da fatura ou o último do extrato. A tela vai para ele.</summary>
        public string? MesDestino { get; set; }
        /// <summary>Linhas que já existiam (card #74).</summary>
        public int QuantidadeJaImportados { get; set; }
        /// <summary>Linhas desmarcadas na prévia (card #74).</summary>
        public int QuantidadeDesmarcados { get; set; }

        public ExtratoImportacaoResultadoDTO() {}

        public ExtratoImportacaoResultadoDTO(
            int extratoId,
            int quantidadeCriados,
            int quantidadeIgnorados,
            decimal valorTotal,
            string mensagem
        )
        {
            ExtratoId = extratoId;
            QuantidadeCriados = quantidadeCriados;
            QuantidadeIgnorados = quantidadeIgnorados;
            ValorTotal = valorTotal;
            Mensagem = mensagem;
        }
    }
}
