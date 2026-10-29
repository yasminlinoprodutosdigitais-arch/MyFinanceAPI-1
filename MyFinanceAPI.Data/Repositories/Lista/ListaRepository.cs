using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ListaRepository(ContextDB context) : RepositorioDoUsuario<Lista>(context), IListaRepository
{
    protected override string MensagemNaoEncontrado => "Lista não encontrada.";

    protected override Expression<Func<Lista, bool>> Proprios(int userId) => l => l.UsuarioId == userId;

    protected override IOrderedQueryable<Lista> Ordenar(IQueryable<Lista> query) => query.OrderBy(l => l.Nome);

    /// <summary>Arquivar não passa por aqui (<see cref="DefinirArquivadaAsync"/>): o PUT não tira a lista da pasta.</summary>
    protected override void CopiarAlteracoes(Lista existente, Lista entrada)
    {
        existente.Nome = entrada.Nome;
        existente.Icone = entrada.Icone;
        existente.UsaQuantidade = entrada.UsaQuantidade;
        existente.UsaValor = entrada.UsaValor;
        existente.ModoValor = entrada.ModoValor;
        existente.UsaLimite = entrada.UsaLimite;
        existente.Limite = entrada.Limite;
        existente.UsaData = entrada.UsaData;
        existente.UsaObservacao = entrada.UsaObservacao;
        existente.Modelo = entrada.Modelo;
    }

    private IQueryable<ListaComResumo> ComResumo(IQueryable<Lista> listas, DateOnly hoje) =>
        ResumoListas.Projetar(listas, context.ItensLista.AsNoTracking(), hoje);

    public async Task<List<ListaComResumo>> ListarComResumoAsync(int userId, bool arquivadas, DateOnly hoje)
    {
        var listas = Tabela.AsNoTracking().Where(Proprios(userId))
            .Where(l => arquivadas ? l.ArquivadaEm != null : l.ArquivadaEm == null);
        return await ComResumo(Ordenar(listas), hoje).ToListAsync();
    }

    public async Task<ListaComResumo?> ObterComResumoAsync(int id, int userId, DateOnly hoje) =>
        await ComResumo(Tabela.AsNoTracking().Where(Proprios(userId)).Where(l => l.Id == id), hoje).FirstOrDefaultAsync();

    public async Task<List<ListaComResumo>> ListarDaSerieAsync(Guid serieId, int userId, DateOnly hoje) =>
        await ComResumo(Tabela.AsNoTracking().Where(Proprios(userId)).Where(l => l.SerieId == serieId).OrderBy(l => l.Id), hoje)
            .ToListAsync();

    public async Task DefinirSerieAsync(int id, int userId, Guid serieId)
    {
        var lista = await ObterProprioAsync(id, userId) ?? throw new NaoEncontradoException(MensagemNaoEncontrado);
        lista.SerieId = serieId;
        await context.SaveChangesAsync();
    }

    public async Task DefinirArquivadaAsync(int id, int userId, DateTime? arquivadaEm)
    {
        var lista = await ObterProprioAsync(id, userId) ?? throw new NaoEncontradoException(MensagemNaoEncontrado);
        lista.ArquivadaEm = arquivadaEm;
        await context.SaveChangesAsync();
    }
}
