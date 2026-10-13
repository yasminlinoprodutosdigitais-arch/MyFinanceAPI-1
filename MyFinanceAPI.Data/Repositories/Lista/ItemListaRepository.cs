using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ItemListaRepository(ContextDB context) : RepositorioDoUsuario<ItemLista>(context), IItemListaRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Item da lista não encontrado.";

    protected override Expression<Func<ItemLista, bool>> Proprios(int userId) => i => i.UserId == userId;

    // Antes ordenava pela navegação (c.Lista), que o EF traduz pela chave: mesmo resultado.
    protected override IOrderedQueryable<ItemLista> Ordenar(IQueryable<ItemLista> query) => query.OrderBy(i => i.ListaId);

    protected override void CopiarAlteracoes(ItemLista existente, ItemLista entrada)
    {
        existente.ListaId = entrada.ListaId;
        existente.Descricao = entrada.Descricao;
        existente.Quantidade = entrada.Quantidade;
        existente.Status = entrada.Status;
        existente.Observacao = entrada.Observacao;
        existente.Valor = entrada.Valor;
    }

    public async Task<List<ItemLista>> GetItemListaByListaId(int id, int userId)
    {
        return await _context.ItemLista
            .Where(c => c.UserId == userId && c.ListaId == id)
            .OrderBy(c => c.Descricao)
            .ToListAsync();
    }
}
