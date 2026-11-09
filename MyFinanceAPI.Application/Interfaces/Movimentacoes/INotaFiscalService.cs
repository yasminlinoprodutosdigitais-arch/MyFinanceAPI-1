using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Leitura da NFC-e pelo link do QR Code (card #76, fase 6).</summary>
public interface INotaFiscalService
{
    Task<NotaFiscalDTO> LerAsync(int userId, string? link);
    /// <summary>Guarda a nota no lançamento e o classifica pelos itens, se o total bater.</summary>
    Task<NotaSalvaDTO> SalvarAsync(int userId, int itemId, NotaFiscalDTO nota);
    Task<NotaFiscalDTO?> ObterAsync(int userId, int itemId);
    Task RemoverAsync(int userId, int itemId);
}
