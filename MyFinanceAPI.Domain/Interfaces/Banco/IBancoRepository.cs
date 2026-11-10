using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IBancoRepository : IRepositorioDoUsuario<Banco>
{
    Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId);
    /// <summary>True se o banco é do usuário e tem lançamentos ou extratos (FKs RESTRICT bloqueiam o DELETE).</summary>
    Task<bool> EmUsoAsync(int id, int userId);
    /// <summary>True se o usuário já tem outro banco com o mesmo nome (ignora maiúsculas e espaços).</summary>
    Task<bool> ExisteNomeAsync(int userId, string nome, int ignorarId);
    /// <summary>True se todas as pessoas são do usuário e nenhuma é o titular (card #77).</summary>
    Task<bool> PessoasValidasAsync(int userId, IReadOnlyCollection<int> pessoasIds);
    /// <summary>Switch do detalhe (card #77); null não muda o campo.</summary>
    Task DefinirCompartilhamentoAsync(int id, int userId, bool? compartilhado, IReadOnlyCollection<int>? pessoasIds, bool? conviteDispensado);
}
