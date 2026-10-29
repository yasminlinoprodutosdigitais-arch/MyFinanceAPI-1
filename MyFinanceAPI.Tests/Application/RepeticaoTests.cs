using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #67, parte 3: as ocorrências de um item que repete, com as exceções de "só esta".</summary>
public class RepeticaoTests
{
    private static DateOnly D(int ano, int mes, int dia) => new(ano, mes, dia);

    private static ItemLista Item(int id, DateOnly data, string repeticao = RepeticaoItem.Nenhuma, int[]? dias = null,
        DateOnly? ate = null, TimeOnly? hora = null, bool feito = false) =>
        new() { Id = id, Descricao = $"Item {id}", Data = data, Repeticao = repeticao, DiasSemana = dias, RepetirAte = ate, Hora = hora, Feito = feito };

    private static DateOnly[] Datas(ItemLista item, DateOnly de, DateOnly ate, params OcorrenciaItemLista[] excecoes) =>
        Repeticao.NoPeriodo([item], excecoes, de, ate).Select(o => o.Data).ToArray();

    [Fact]
    public void Diaria_vai_da_data_ate_o_fim_do_periodo_e_respeita_o_ate()
    {
        var item = Item(1, D(2026, 10, 28), RepeticaoItem.Diaria, ate: D(2026, 10, 31));

        Assert.Equal([D(2026, 10, 28), D(2026, 10, 29), D(2026, 10, 30), D(2026, 10, 31)], Datas(item, D(2026, 10, 26), D(2026, 11, 1)));
    }

    [Fact]
    public void Semanal_nos_dias_marcados_a_partir_da_data()
    {
        // 26/10/2026 é segunda. Seg, qua e sex, começando na quarta 28.
        var item = Item(1, D(2026, 10, 28), RepeticaoItem.Semanal, dias: [1, 3, 5]);

        Assert.Equal([D(2026, 10, 28), D(2026, 10, 30), D(2026, 11, 2), D(2026, 11, 4), D(2026, 11, 6)],
            Datas(item, D(2026, 10, 26), D(2026, 11, 8)));
    }

    [Fact]
    public void Semanal_no_domingo_usa_7()
    {
        var item = Item(1, D(2026, 10, 25), RepeticaoItem.Semanal, dias: [7]);

        Assert.Equal([D(2026, 10, 25), D(2026, 11, 1)], Datas(item, D(2026, 10, 25), D(2026, 11, 7)));
    }

    [Fact]
    public void Mensal_no_dia_31_cai_no_ultimo_dia_dos_meses_curtos_e_no_29_do_bissexto()
    {
        var item = Item(1, D(2027, 12, 31), RepeticaoItem.Mensal);

        Assert.Equal([D(2027, 12, 31), D(2028, 1, 31), D(2028, 2, 29), D(2028, 3, 31), D(2028, 4, 30)],
            Datas(item, D(2027, 12, 1), D(2028, 4, 30)));
    }

    [Fact]
    public void Item_sem_repeticao_aparece_uma_vez_com_o_feito_dele()
    {
        var item = Item(1, D(2026, 10, 29), hora: new TimeOnly(14, 30), feito: true);

        var o = Assert.Single(Repeticao.NoPeriodo([item], [], D(2026, 10, 26), D(2026, 11, 1)));
        Assert.True(o.Feito);
        Assert.False(o.Repete);
        Assert.Equal(new TimeOnly(14, 30), o.Hora);
        Assert.Empty(Repeticao.NoPeriodo([item], [], D(2026, 11, 2), D(2026, 11, 8)));
    }

    [Fact]
    public void Feito_vale_so_para_a_ocorrencia_marcada()
    {
        var item = Item(1, D(2026, 10, 26), RepeticaoItem.Diaria);
        var feita = new OcorrenciaItemLista { ItemId = 1, Data = D(2026, 10, 27), Feito = true };

        var ocorrencias = Repeticao.NoPeriodo([item], [feita], D(2026, 10, 26), D(2026, 10, 28));

        Assert.Equal([false, true, false], ocorrencias.Select(o => o.Feito));
        Assert.All(ocorrencias, o => Assert.True(o.Repete));
    }

    [Fact]
    public void Ocorrencia_pulada_some_e_movida_vai_para_o_outro_dia_e_hora()
    {
        var item = Item(1, D(2026, 10, 26), RepeticaoItem.Diaria, hora: new TimeOnly(7, 0));
        var pulada = new OcorrenciaItemLista { ItemId = 1, Data = D(2026, 10, 27), Pulada = true };
        var movida = new OcorrenciaItemLista { ItemId = 1, Data = D(2026, 10, 28), NovaData = D(2026, 11, 3), NovaHora = new TimeOnly(9, 0) };

        var semana = Repeticao.NoPeriodo([item], [pulada, movida], D(2026, 10, 26), D(2026, 10, 29));
        Assert.Equal([D(2026, 10, 26), D(2026, 10, 29)], semana.Select(o => o.Data));

        var mais = Repeticao.NoPeriodo([item], [pulada, movida], D(2026, 11, 3), D(2026, 11, 3));
        Assert.Equal(2, mais.Count);
        var vinda = mais.Single(o => o.DataOriginal == D(2026, 10, 28));
        Assert.Equal(D(2026, 11, 3), vinda.Data);
        Assert.Equal(new TimeOnly(9, 0), vinda.Hora);
    }

    [Fact]
    public void Ordena_por_data_dia_todo_primeiro_e_depois_pela_hora()
    {
        var itens = new[]
        {
            Item(1, D(2026, 10, 29), hora: new TimeOnly(18, 0)),
            Item(2, D(2026, 10, 29)),
            Item(3, D(2026, 10, 29), hora: new TimeOnly(8, 0)),
            Item(4, D(2026, 10, 28), hora: new TimeOnly(23, 0)),
        };

        Assert.Equal([4, 2, 3, 1], Repeticao.NoPeriodo(itens, [], D(2026, 10, 26), D(2026, 11, 1)).Select(o => o.Item.Id));
    }

    [Fact]
    public void Item_sem_data_nao_entra_na_agenda()
    {
        var semData = new ItemLista { Id = 9, Descricao = "Sem data" };

        Assert.Empty(Repeticao.NoPeriodo([semData], [], D(2026, 1, 1), D(2026, 12, 31)));
    }

    [Theory]
    [InlineData(2026, 10, 30, true)]   // sexta
    [InlineData(2026, 10, 31, false)]  // sábado
    [InlineData(2026, 10, 26, false)]  // segunda antes do início
    [InlineData(2026, 11, 30, false)]  // depois do até
    public void Sabe_se_uma_data_e_ocorrencia_da_regra(int ano, int mes, int dia, bool esperado)
    {
        var item = Item(1, D(2026, 10, 28), RepeticaoItem.Semanal, dias: [1, 3, 5], ate: D(2026, 11, 27));

        Assert.Equal(esperado, Repeticao.EhOcorrencia(item, D(ano, mes, dia)));
    }

    [Fact]
    public void Proxima_pendente_pula_as_feitas_e_as_puladas()
    {
        var item = Item(1, D(2026, 10, 26), RepeticaoItem.Diaria);
        var excecoes = new[]
        {
            new OcorrenciaItemLista { ItemId = 1, Data = D(2026, 10, 29), Feito = true },
            new OcorrenciaItemLista { ItemId = 1, Data = D(2026, 10, 30), Pulada = true },
        };

        Assert.Equal(D(2026, 10, 31), Repeticao.ProximaPendente(item, excecoes, D(2026, 10, 29)));
        Assert.Null(Repeticao.ProximaPendente(Item(2, D(2026, 10, 1), RepeticaoItem.Diaria, ate: D(2026, 10, 10)), [], D(2026, 10, 29)));
    }
}
