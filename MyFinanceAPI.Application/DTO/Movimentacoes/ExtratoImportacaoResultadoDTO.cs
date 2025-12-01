namespace MyFinanceAPI.Application.DTO.Extrato
{
    public class ExtratoImportacaoResultadoDTO
    {
        public int ExtratoId { get; set; }
        public int QuantidadeCriados { get; set; }
        public int QuantidadeIgnorados { get; set; }
        public decimal ValorTotal { get; set; }
        public string Mensagem { get; set; }

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
