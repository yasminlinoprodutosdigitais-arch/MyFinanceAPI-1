using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Repositories;

/// <summary>
/// Resumo de cada lista (card #67): subconsultas correlacionadas que o EF junta num SELECT só, sem
/// trazer os itens para a memória. Os testes rodam a mesma projeção em LINQ to Objects. A próxima data aqui é só
/// dos itens que não repetem; a dos que repetem depende das ocorrências e sai do ListaService (parte 3).
/// </summary>
public static class ResumoListas
{
    public static IQueryable<ListaComResumo> Projetar(IQueryable<Lista> listas, IQueryable<ItemLista> itens, DateOnly hoje) =>
        listas.Select(l => new ListaComResumo
        {
            Lista = l,
            Itens = itens.Count(i => i.ListaId == l.Id),
            Feitos = itens.Count(i => i.ListaId == l.Id && i.Feito),
            Total = itens
                .Where(i => i.ListaId == l.Id && i.Valor != null)
                .Sum(i => l.ModoValor == ModoValorLista.Unitario ? (i.Quantidade ?? 1) * i.Valor!.Value : i.Valor!.Value),
            TotalFeito = itens
                .Where(i => i.ListaId == l.Id && i.Valor != null && i.Feito)
                .Sum(i => l.ModoValor == ModoValorLista.Unitario ? (i.Quantidade ?? 1) * i.Valor!.Value : i.Valor!.Value),
            ProximaData = itens
                .Where(i => i.ListaId == l.Id && !i.Feito && i.Repeticao == RepeticaoItem.Nenhuma && i.Data != null && i.Data >= hoje)
                .Min(i => i.Data),
        });
}
