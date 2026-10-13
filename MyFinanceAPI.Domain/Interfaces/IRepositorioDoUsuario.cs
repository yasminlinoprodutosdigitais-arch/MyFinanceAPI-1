using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>
/// CRUD comum dos cadastros simples, sempre restrito ao usuário (BE-P2-03).
/// Cada interface de cadastro herda esta e acrescenta só o que é dela.
/// </summary>
public interface IRepositorioDoUsuario<T> where T : class, IEntidade
{
    Task<List<T>> ListarAsync(int userId);
    /// <summary>Null se não existe ou se o usuário não o enxerga.</summary>
    Task<T?> ObterPorIdAsync(int id, int userId);
    Task<T> CriarAsync(T entidade);
    /// <summary>Só altera registro do próprio usuário; senão, NaoEncontradoException.</summary>
    Task AtualizarAsync(T entrada, int userId);
    /// <summary>Só remove registro do próprio usuário; devolve o removido, ou null.</summary>
    Task<T?> RemoverAsync(int id, int userId);
}
