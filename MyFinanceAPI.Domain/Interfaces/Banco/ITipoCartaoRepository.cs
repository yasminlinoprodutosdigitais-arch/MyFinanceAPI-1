using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>Lê os tipos do usuário e os padrão do sistema; altera e remove só os do usuário.</summary>
public interface ITipoCartaoRepository : IRepositorioDoUsuario<TipoCartao>
{
    /// <summary>True se o tipo de cartão é do usuário e está em uso por bancos ou lançamentos.</summary>
    Task<bool> EmUsoAsync(int id, int userId);
    /// <summary>True se o usuário já tem outro tipo de cartão com o mesmo nome (ignora maiúsculas e espaços).</summary>
    Task<bool> ExisteNomeAsync(int userId, string nome, int ignorarId);
}
