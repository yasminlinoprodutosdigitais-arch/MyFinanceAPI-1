namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>
/// Agrupa várias gravações numa transação: ou todas valem, ou nenhuma (BE-P1-05).
/// Dentro de uma operação já em andamento, a chamada participa da transação existente.
/// </summary>
public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> operacao);
    Task<T> ExecuteAsync<T>(Func<Task<T>> operacao);
}
