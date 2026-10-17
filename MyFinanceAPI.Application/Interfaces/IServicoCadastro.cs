namespace MyFinanceAPI.Application.Interfaces;

/// <summary>
/// Operações comuns dos cadastros simples, sempre do usuário logado (BE-P2-02).
/// Cada interface de cadastro herda esta e acrescenta só o que é dela.
/// </summary>
public interface IServicoCadastro<TDto> where TDto : class
{
    Task<IEnumerable<TDto>> ListarAsync(int userId);
    /// <summary>Null se não existe ou se o usuário não o enxerga.</summary>
    Task<TDto?> ObterPorIdAsync(int id, int userId);
    /// <summary>Devolve o registro criado, já com o id gerado.</summary>
    Task<TDto> AdicionarAsync(TDto dto, int userId);
    /// <summary>Registro alheio ou inexistente: NaoEncontradoException (404).</summary>
    Task AtualizarAsync(TDto dto, int userId);
    Task RemoverAsync(int id, int userId);
}
