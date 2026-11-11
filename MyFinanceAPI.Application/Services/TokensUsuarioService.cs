using System.Security.Cryptography;
using System.Text;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #50: token aleatório (32 bytes, base64url) no link; no banco, só o SHA-256 dele. Quem lê o banco
/// não consegue montar um link válido.
/// </summary>
public class TokensUsuarioService(ITokenUsuarioRepository repositorio, TimeProvider relogio) : ITokensUsuarioService
{
    private static TimeSpan Validade(TipoTokenUsuario tipo) => tipo switch
    {
        TipoTokenUsuario.ConfirmarEmail => TimeSpan.FromHours(24),
        TipoTokenUsuario.RedefinirSenha => TimeSpan.FromMinutes(30),
        _ => throw new ArgumentOutOfRangeException(nameof(tipo)),
    };

    public async Task<string> GerarAsync(int usuarioId, TipoTokenUsuario tipo)
    {
        var agora = relogio.GetUtcNow().UtcDateTime;
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        await repositorio.SubstituirAsync(new TokenUsuario
        {
            UsuarioId = usuarioId,
            Tipo = tipo,
            Hash = Hash(token),
            ExpiraEm = agora + Validade(tipo),
        }, agora);
        return token;
    }

    public async Task<int?> ConsumirAsync(string token, TipoTokenUsuario tipo)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var agora = relogio.GetUtcNow().UtcDateTime;
        var gravado = await repositorio.ObterPorHashAsync(Hash(token), tipo);
        if (gravado == null || gravado.UsadoEm != null || gravado.ExpiraEm <= agora)
            return null;

        await repositorio.MarcarUsadoAsync(gravado, agora);
        return gravado.UsuarioId;
    }

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();
}
