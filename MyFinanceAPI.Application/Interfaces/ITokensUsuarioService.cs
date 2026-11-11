using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Tokens dos links de e-mail (card #50): confirmar o e-mail e redefinir a senha.</summary>
public interface ITokensUsuarioService
{
    /// <summary>Token para o link; os anteriores do mesmo usuário e tipo deixam de valer.</summary>
    Task<string> GerarAsync(int usuarioId, TipoTokenUsuario tipo);

    /// <summary>Usuário do token, que passa a não valer mais; null se não existe, venceu ou já foi usado.</summary>
    Task<int?> ConsumirAsync(string token, TipoTokenUsuario tipo);
}
