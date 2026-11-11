using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class PreferenciaNotificacaoRepository(ContextDB context) : IPreferenciaNotificacaoRepository
{
    public Task<PreferenciaNotificacao?> ObterAsync(int userId) =>
        context.PreferenciasNotificacao.AsNoTracking().FirstOrDefaultAsync(p => p.UsuarioId == userId);

    public async Task SalvarConfigAsync(int userId, string config)
    {
        var linha = await LinhaAsync(userId);
        linha.Config = config;
        await GravarAsync(linha);
    }

    public async Task MarcarLidasAsync(int userId, DateTime lidasAte)
    {
        var linha = await LinhaAsync(userId);
        linha.LidasAte = lidasAte;
        await GravarAsync(linha);
    }

    private async Task<PreferenciaNotificacao> LinhaAsync(int userId)
    {
        var linha = await context.PreferenciasNotificacao.FirstOrDefaultAsync(p => p.UsuarioId == userId);
        if (linha != null)
            return linha;
        linha = new PreferenciaNotificacao { UsuarioId = userId };
        await context.PreferenciasNotificacao.AddAsync(linha);
        return linha;
    }

    private async Task GravarAsync(PreferenciaNotificacao linha)
    {
        context.Entry(linha).Property("DataAlteracao").CurrentValue = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }
}
