using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data;

/// <summary>
/// Transação explícita sobre o DbContext da requisição (BE-P1-05). Os repositórios continuam
/// chamando SaveChanges; dentro de ExecuteAsync essas gravações só valem no commit.
/// </summary>
public class UnitOfWork(DbContext context) : IUnitOfWork
{
    private readonly DbContext _context = context;

    public async Task ExecuteAsync(Func<Task> operacao)
    {
        await ExecuteAsync(async () =>
        {
            await operacao();
            return true;
        });
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operacao)
    {
        // Já dentro de uma operação (ex.: TransactionService.Add criando a conta): participa dela.
        if (_context.Database.CurrentTransaction != null)
            return await operacao();

        await using var transacao = await _context.Database.BeginTransactionAsync();
        try
        {
            var resultado = await operacao();
            await transacao.CommitAsync();
            return resultado;
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }
}
