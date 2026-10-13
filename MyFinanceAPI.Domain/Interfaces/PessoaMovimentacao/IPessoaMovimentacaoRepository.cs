using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IPessoaMovimentacaoRepository : IRepositorioDoUsuario<PessoaMovimentacao>
{
    /// <summary>Busca pelo nome normalizado (trim + maiúsculas); nome vazio devolve lista vazia.</summary>
    Task<IEnumerable<PessoaMovimentacao>> VerificaPossuiPessoa(string? nomePessoa, int userId);
    /// <summary>Busca-ou-cria pelo nome normalizado; se a pessoa já existe, devolve a existente.</summary>
    Task<PessoaMovimentacao> ObterOuCriarAsync(PessoaMovimentacao pessoa, int userId);
    /// <summary>True se a pessoa é do usuário e está vinculada a itens de extrato.</summary>
    Task<bool> EmUsoAsync(int id, int userId);
}
