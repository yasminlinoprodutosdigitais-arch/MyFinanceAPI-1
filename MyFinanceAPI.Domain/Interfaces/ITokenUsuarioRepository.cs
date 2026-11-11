using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>Tokens dos links de e-mail (card #50).</summary>
public interface ITokenUsuarioRepository
{
    /// <summary>Grava <paramref name="novo"/> e marca como usados os ainda válidos do mesmo usuário e tipo.</summary>
    Task SubstituirAsync(TokenUsuario novo, DateTime agora);
    Task<TokenUsuario?> ObterPorHashAsync(string hash, TipoTokenUsuario tipo);
    Task MarcarUsadoAsync(TokenUsuario token, DateTime agora);
}
