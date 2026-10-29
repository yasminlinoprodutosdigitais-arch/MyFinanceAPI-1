using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ItemListaRepository(ContextDB context) : RepositorioDoUsuario<ItemLista>(context), IItemListaRepository
{
    private const string DataAlteracao = nameof(DataAlteracao);

    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Item da lista não encontrado.";

    protected override Expression<Func<ItemLista, bool>> Proprios(int userId) => i => i.UsuarioId == userId;

    protected override IOrderedQueryable<ItemLista> Ordenar(IQueryable<ItemLista> query) =>
        query.OrderBy(i => i.ListaId).ThenBy(i => i.Feito).ThenBy(i => i.Ordem ?? int.MaxValue).ThenBy(i => i.Descricao);

    /// <summary>A lista do item não muda pelo PUT (o item vem pela rota da lista).</summary>
    protected override void CopiarAlteracoes(ItemLista existente, ItemLista entrada)
    {
        existente.Descricao = entrada.Descricao;
        existente.Feito = entrada.Feito;
        existente.FeitoEm = entrada.FeitoEm;
        existente.Quantidade = entrada.Quantidade;
        existente.Valor = entrada.Valor;
        existente.Data = entrada.Data;
        existente.Hora = entrada.Hora;
        existente.Repeticao = entrada.Repeticao;
        existente.DiasSemana = entrada.DiasSemana;
        existente.RepetirAte = entrada.RepetirAte;
        existente.Observacao = entrada.Observacao;
        existente.Ordem = entrada.Ordem;
    }

    private IQueryable<ItemLista> DaLista(int listaId, int userId) =>
        _context.ItensLista.Where(i => i.UsuarioId == userId && i.ListaId == listaId);

    public async Task<List<ItemLista>> ListarDaListaAsync(int listaId, int userId) =>
        await Ordenar(DaLista(listaId, userId).AsNoTracking()).ToListAsync();

    public async Task<List<ItemLista>> ListarDasListasAsync(IReadOnlyCollection<int> listaIds, int userId) =>
        await _context.ItensLista.AsNoTracking()
            .Where(i => i.UsuarioId == userId && listaIds.Contains(i.ListaId))
            .ToListAsync();

    public async Task CriarVariosAsync(IEnumerable<ItemLista> itens)
    {
        await _context.ItensLista.AddRangeAsync(itens);
        await _context.SaveChangesAsync();
    }

    public Task<int> MarcarFeitoAsync(int listaId, IReadOnlyCollection<int> ids, bool feito, DateTime agora, int userId) =>
        DaLista(listaId, userId)
            .Where(i => ids.Contains(i.Id) && i.Feito != feito)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Feito, feito)
                .SetProperty(i => i.FeitoEm, feito ? agora : (DateTime?)null)
                .SetProperty(i => EF.Property<DateTime>(i, DataAlteracao), agora));

    public Task<int> DesmarcarTodosAsync(int listaId, DateTime agora, int userId) =>
        DaLista(listaId, userId)
            .Where(i => i.Feito)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Feito, false)
                .SetProperty(i => i.FeitoEm, (DateTime?)null)
                .SetProperty(i => EF.Property<DateTime>(i, DataAlteracao), agora));

    public Task<int> ExcluirConcluidosAsync(int listaId, int userId) =>
        DaLista(listaId, userId).Where(i => i.Feito && i.Repeticao == RepeticaoItem.Nenhuma).ExecuteDeleteAsync();

    public async Task<List<ItemLista>> ListarQueRepetemAsync(IReadOnlyCollection<int> listaIds, int userId) =>
        await _context.ItensLista.AsNoTracking()
            .Where(i => i.UsuarioId == userId && listaIds.Contains(i.ListaId) && i.Repeticao != RepeticaoItem.Nenhuma)
            .ToListAsync();

    public async Task<List<OcorrenciaItemLista>> ListarOcorrenciasAsync(IReadOnlyCollection<int> itemIds, int userId) =>
        await _context.ItensListaOcorrencias.AsNoTracking()
            .Where(o => o.UsuarioId == userId && itemIds.Contains(o.ItemId))
            .ToListAsync();

    public async Task GravarOcorrenciaAsync(int itemId, DateOnly dataOriginal, int userId, Action<OcorrenciaItemLista> alterar)
    {
        var ocorrencia = await _context.ItensListaOcorrencias
            .FirstOrDefaultAsync(o => o.UsuarioId == userId && o.ItemId == itemId && o.Data == dataOriginal);
        if (ocorrencia is null)
        {
            ocorrencia = new OcorrenciaItemLista { ItemId = itemId, Data = dataOriginal, UsuarioId = userId };
            await _context.ItensListaOcorrencias.AddAsync(ocorrencia);
        }

        alterar(ocorrencia);
        await _context.SaveChangesAsync();
    }

    public Task<int> ApagarOcorrenciasAsync(int itemId, DateOnly aPartirDe, int userId) =>
        _context.ItensListaOcorrencias
            .Where(o => o.UsuarioId == userId && o.ItemId == itemId && o.Data >= aPartirDe)
            .ExecuteDeleteAsync();

    public Task<int> DesmarcarOcorrenciasAsync(int listaId, int userId) =>
        _context.ItensListaOcorrencias
            .Where(o => o.UsuarioId == userId && o.Feito && DaLista(listaId, userId).Any(i => i.Id == o.ItemId))
            .ExecuteUpdateAsync(s => s
                .SetProperty(o => o.Feito, false)
                .SetProperty(o => o.FeitoEm, (DateTime?)null));
}
