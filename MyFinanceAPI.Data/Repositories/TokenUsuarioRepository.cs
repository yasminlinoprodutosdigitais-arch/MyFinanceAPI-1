using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TokenUsuarioRepository(ContextDB context) : ITokenUsuarioRepository
{
    public async Task SubstituirAsync(TokenUsuario novo, DateTime agora)
    {
        var validos = await context.TokensUsuario
            .Where(t => t.UsuarioId == novo.UsuarioId && t.Tipo == novo.Tipo && t.UsadoEm == null)
            .ToListAsync();
        foreach (var anterior in validos)
            anterior.UsadoEm = agora;

        await context.TokensUsuario.AddAsync(novo);
        await context.SaveChangesAsync();
    }

    public Task<TokenUsuario?> ObterPorHashAsync(string hash, TipoTokenUsuario tipo) =>
        context.TokensUsuario.FirstOrDefaultAsync(t => t.Hash == hash && t.Tipo == tipo);

    public async Task MarcarUsadoAsync(TokenUsuario token, DateTime agora)
    {
        token.UsadoEm = agora;
        context.TokensUsuario.Update(token);
        await context.SaveChangesAsync();
    }
}
