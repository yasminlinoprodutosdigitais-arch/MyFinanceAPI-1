using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>Notas fiscais dos lançamentos (card #76, fase 6).</summary>
public interface INotaFiscalRepository
{
    /// <summary>A nota do lançamento, com os itens em ordem e a categoria deles.</summary>
    Task<NotaFiscal?> ObterPorItemAsync(int itemId, int userId);
    /// <summary>Grava a nota, trocando a que o lançamento já tinha.</summary>
    Task SubstituirAsync(NotaFiscal nota);
    Task RemoverPorItemAsync(int itemId, int userId);
}
