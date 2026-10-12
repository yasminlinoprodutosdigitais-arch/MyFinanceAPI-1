using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Executa a operação direto, mas sabe se está dentro dela. Os mocks chamam Registrar em cada
/// gravação: o que for gravado fora de uma operação aparece em Fora.
/// </summary>
internal sealed class UnitOfWorkFalso : IUnitOfWork
{
    private bool _dentro;

    public List<string> Fora { get; } = new();

    public void Registrar(string gravacao)
    {
        if (!_dentro)
            Fora.Add(gravacao);
    }

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
        var antes = _dentro;
        _dentro = true;
        try
        {
            return await operacao();
        }
        finally
        {
            _dentro = antes;
        }
    }
}
