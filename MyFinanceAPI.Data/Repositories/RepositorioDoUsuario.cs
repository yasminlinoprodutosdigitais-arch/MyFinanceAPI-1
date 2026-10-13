using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

/// <summary>
/// CRUD dos cadastros simples num lugar só (BE-P2-03). O repositório concreto declara o dono,
/// a ordem, os campos alteráveis e, se precisar, os includes e uma visibilidade mais ampla.
/// Ler usa <see cref="Visiveis"/>; alterar e remover usam só <see cref="Proprios"/> — assim um
/// registro do sistema (TipoCartao sem dono) nunca é alterado nem removido por aqui.
/// </summary>
public abstract class RepositorioDoUsuario<T>(DbContext context) : IRepositorioDoUsuario<T>
    where T : class, IEntidade
{
    protected DbSet<T> Tabela => context.Set<T>();

    protected abstract string MensagemNaoEncontrado { get; }

    /// <summary>Registros que o usuário pode alterar e remover.</summary>
    protected abstract Expression<Func<T, bool>> Proprios(int userId);

    /// <summary>Registros que o usuário enxerga; por padrão, os próprios.</summary>
    protected virtual Expression<Func<T, bool>> Visiveis(int userId) => Proprios(userId);

    /// <summary>Includes da leitura.</summary>
    protected virtual IQueryable<T> ComDetalhes(IQueryable<T> query) => query;

    protected abstract IOrderedQueryable<T> Ordenar(IQueryable<T> query);

    /// <summary>Campos alteráveis, um a um. Dono e data de criação nunca entram aqui.</summary>
    protected abstract void CopiarAlteracoes(T existente, T entrada);

    public virtual async Task<List<T>> ListarAsync(int userId) =>
        await Ordenar(ComDetalhes(Tabela.Where(Visiveis(userId)))).ToListAsync();

    public virtual async Task<T?> ObterPorIdAsync(int id, int userId) =>
        await ComDetalhes(Tabela.Where(Visiveis(userId))).FirstOrDefaultAsync(e => e.Id == id);

    public virtual async Task<T> CriarAsync(T entidade)
    {
        await Tabela.AddAsync(entidade);
        await context.SaveChangesAsync();
        return entidade;
    }

    public virtual async Task AtualizarAsync(T entrada, int userId)
    {
        var existente = await ObterProprioAsync(entrada.Id, userId)
            ?? throw new NaoEncontradoException(MensagemNaoEncontrado);

        CopiarAlteracoes(existente, entrada);
        await context.SaveChangesAsync();
    }

    public virtual async Task<T?> RemoverAsync(int id, int userId)
    {
        var existente = await ObterProprioAsync(id, userId);
        if (existente == null)
            return null;

        Tabela.Remove(existente);
        await context.SaveChangesAsync();
        return existente;
    }

    protected Task<T?> ObterProprioAsync(int id, int userId) =>
        Tabela.Where(Proprios(userId)).FirstOrDefaultAsync(e => e.Id == id);
}
