using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Services;

/// <summary>Uma ocorrência na agenda: <see cref="Data"/> e <see cref="Hora"/> já com a exceção de "só esta" aplicada.</summary>
public sealed record OcorrenciaAgenda(ItemLista Item, DateOnly DataOriginal, DateOnly Data, TimeOnly? Hora, bool Feito, DateTime? FeitoEm)
{
    public bool Repete => Item.Repeticao != RepeticaoItem.Nenhuma;
}

/// <summary>
/// Ocorrências dos itens com data (card #67, parte 3). Pura: a regra fica no item (a data é a primeira ocorrência)
/// e as exceções (feita, movida, pulada) chegam de itens_lista_ocorrencias pela data original.
/// <list type="bullet">
/// <item>DIARIA: todo dia; SEMANAL: nos dias_semana (1 = seg … 7 = dom); MENSAL: no dia da data, ou no último dia
/// do mês quando ele for mais curto (31 → 30/11, 29/02 no bissexto).</item>
/// <item>Item sem repetição aparece uma vez, com o feito dele.</item>
/// </list>
/// </summary>
public static class Repeticao
{
    /// <summary>Até onde a próxima pendente procura.</summary>
    private const int DiasDeBuscaDaProxima = 400;

    public static List<OcorrenciaAgenda> NoPeriodo(IEnumerable<ItemLista> itens, IEnumerable<OcorrenciaItemLista> excecoes, DateOnly de, DateOnly ate)
    {
        var porItem = excecoes.GroupBy(e => e.ItemId).ToDictionary(g => g.Key, g => g.ToDictionary(e => e.Data));
        var ocorrencias = new List<OcorrenciaAgenda>();

        foreach (var item in itens.Where(i => i.Data is not null))
        {
            if (item.Repeticao == RepeticaoItem.Nenhuma)
            {
                if (item.Data >= de && item.Data <= ate)
                    ocorrencias.Add(new OcorrenciaAgenda(item, item.Data!.Value, item.Data.Value, item.Hora, item.Feito, item.FeitoEm));
                continue;
            }

            var doItem = porItem.GetValueOrDefault(item.Id) ?? [];
            foreach (var original in DatasDaRegra(item, de, ate))
                if (Aplicar(item, original, doItem.GetValueOrDefault(original)) is { } o && o.Data >= de && o.Data <= ate)
                    ocorrencias.Add(o);

            // Movidas de fora do período para dentro dele.
            foreach (var e in doItem.Values.Where(e => !e.Pulada && e.NovaData >= de && e.NovaData <= ate && (e.Data < de || e.Data > ate)))
                if (EhOcorrencia(item, e.Data))
                    ocorrencias.Add(Aplicar(item, e.Data, e)!);
        }

        return ocorrencias
            .OrderBy(o => o.Data)
            .ThenBy(o => o.Hora.HasValue)
            .ThenBy(o => o.Hora)
            .ThenBy(o => o.Item.Descricao, StringComparer.InvariantCulture)
            .ToList();
    }

    /// <summary>A data é uma ocorrência da regra (antes de qualquer exceção)?</summary>
    public static bool EhOcorrencia(ItemLista item, DateOnly data) =>
        item.Data is not null && DatasDaRegra(item, data, data).Any();

    /// <summary>Primeira ocorrência pendente de <paramref name="aPartirDe"/> em diante (para o resumo da lista).</summary>
    public static DateOnly? ProximaPendente(ItemLista item, IEnumerable<OcorrenciaItemLista> excecoes, DateOnly aPartirDe) =>
        NoPeriodo([item], excecoes, aPartirDe, aPartirDe.AddDays(DiasDeBuscaDaProxima))
            .Where(o => !o.Feito)
            .Select(o => (DateOnly?)o.Data)
            .FirstOrDefault();

    private static OcorrenciaAgenda? Aplicar(ItemLista item, DateOnly original, OcorrenciaItemLista? excecao)
    {
        if (excecao?.Pulada == true)
            return null;
        return new OcorrenciaAgenda(item, original,
            excecao?.NovaData ?? original,
            excecao?.NovaHora ?? item.Hora,
            excecao?.Feito ?? false,
            excecao?.FeitoEm);
    }

    /// <summary>Datas originais da regra entre <paramref name="de"/> e <paramref name="ate"/>, inclusive.</summary>
    private static IEnumerable<DateOnly> DatasDaRegra(ItemLista item, DateOnly de, DateOnly ate)
    {
        var inicio = item.Data!.Value;
        if (item.RepetirAte is { } fim && fim < ate)
            ate = fim;
        if (de < inicio)
            de = inicio;
        if (de > ate)
            yield break;

        if (item.Repeticao == RepeticaoItem.Nenhuma)
        {
            if (inicio >= de && inicio <= ate)
                yield return inicio;
            yield break;
        }

        if (item.Repeticao == RepeticaoItem.Mensal)
        {
            for (var mes = new DateOnly(de.Year, de.Month, 1); mes <= ate; mes = mes.AddMonths(1))
            {
                var dia = new DateOnly(mes.Year, mes.Month, Math.Min(inicio.Day, DateTime.DaysInMonth(mes.Year, mes.Month)));
                if (dia >= de && dia <= ate)
                    yield return dia;
            }
            yield break;
        }

        var dias = item.Repeticao == RepeticaoItem.Semanal ? new HashSet<int>(item.DiasSemana ?? []) : null;
        for (var dia = de; dia <= ate; dia = dia.AddDays(1))
            if (dias is null || dias.Contains(DiaIso(dia)))
                yield return dia;
    }

    /// <summary>1 = segunda … 7 = domingo.</summary>
    private static int DiaIso(DateOnly dia) => dia.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dia.DayOfWeek;
}
