using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

/// <summary>Busca rápida (card #72). Os filtros são expressões públicas para os testes rodarem em memória.</summary>
public class BuscaRepository(ContextDB context) : IBuscaRepository
{
    public async Task<IReadOnlyList<ItemBusca>> BuscarAsync(int userId, string termoMaiusculo, int limite)
    {
        var planejamentos = await context.Listas.AsNoTracking().Where(BuscaFiltros.Planejamento(userId, termoMaiusculo))
            .OrderBy(l => l.ArquivadaEm != null).ThenBy(l => l.Nome).Take(limite)
            .Select(l => new ItemBusca(TiposBusca.Planejamento, l.Id, l.Nome, l.ArquivadaEm != null ? "Arquivado" : null, null))
            .ToListAsync();
        var itens = await context.ItensLista.AsNoTracking().Where(BuscaFiltros.Item(userId, termoMaiusculo))
            .OrderBy(i => i.Descricao).Take(limite)
            .Select(i => new ItemBusca(TiposBusca.Item, i.Id, i.Descricao, i.Lista != null ? i.Lista.Nome : null, i.ListaId))
            .ToListAsync();
        var categorias = await context.Categorias.AsNoTracking().Where(BuscaFiltros.Categoria(userId, termoMaiusculo))
            .OrderBy(c => c.Nome).Take(limite)
            .Select(c => new ItemBusca(TiposBusca.Categoria, c.Id, c.Nome, c.Subcategoria, null))
            .ToListAsync();
        var contas = await context.Contas.AsNoTracking().Where(BuscaFiltros.ContaFixa(userId, termoMaiusculo))
            .OrderBy(c => c.Nome).Take(limite)
            .Select(c => new ItemBusca(TiposBusca.ContaFixa, c.Id, c.Nome, c.Categoria != null ? c.Categoria.Nome : null, null))
            .ToListAsync();
        return [.. planejamentos, .. itens, .. categorias, .. contas];
    }
}

public static class BuscaFiltros
{
    public static Expression<Func<Lista, bool>> Planejamento(int userId, string termo) =>
        l => l.UsuarioId == userId && l.Nome.ToUpper().Contains(termo);

    public static Expression<Func<ItemLista, bool>> Item(int userId, string termo) =>
        i => i.UsuarioId == userId && i.Descricao.ToUpper().Contains(termo);

    public static Expression<Func<Categoria, bool>> Categoria(int userId, string termo) =>
        c => c.UsuarioId == userId && c.Nome.ToUpper().Contains(termo);

    public static Expression<Func<Conta, bool>> ContaFixa(int userId, string termo) =>
        c => c.UsuarioId == userId && c.Nome.ToUpper().Contains(termo);
}
