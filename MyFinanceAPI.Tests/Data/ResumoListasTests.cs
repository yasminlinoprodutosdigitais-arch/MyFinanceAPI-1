using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Card #67: o resumo de cada lista no Cadastro sai de uma consulta só. A projeção é a mesma que o EF
/// traduz; aqui roda em memória.
/// </summary>
public class ResumoListasTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 28);

    private static Lista L(int id, string modo = ModoValorLista.Total) => new() { Id = id, Nome = $"L{id}", ModoValor = modo };

    private static ItemLista I(int listaId, bool feito = false, int? qtd = null, decimal? valor = null, DateOnly? data = null) =>
        new() { ListaId = listaId, Descricao = "x", Feito = feito, Quantidade = qtd, Valor = valor, Data = data };

    private static ListaComResumo Resumo(Lista lista, params ItemLista[] itens) =>
        ResumoListas.Projetar(new[] { lista }.AsQueryable(), itens.AsQueryable(), Hoje).Single();

    [Fact]
    public void Modo_total_soma_o_valor_e_ignora_a_quantidade()
    {
        var r = Resumo(L(1), I(1, qtd: 3, valor: 10m), I(1, feito: true, qtd: 2, valor: 5m));

        Assert.Equal(15m, r.Total);
        Assert.Equal(5m, r.TotalFeito);
    }

    [Fact]
    public void Modo_unitario_soma_quantidade_vezes_valor_e_sem_quantidade_conta_um()
    {
        var r = Resumo(L(1, ModoValorLista.Unitario), I(1, qtd: 2, valor: 6.5m), I(1, feito: true, valor: 4m));

        Assert.Equal(17m, r.Total);
        Assert.Equal(4m, r.TotalFeito);
    }

    [Fact]
    public void Item_sem_valor_fica_fora_da_soma_mas_conta_nos_itens()
    {
        var r = Resumo(L(1), I(1, valor: 10m), I(1), I(1, feito: true));

        Assert.Equal(3, r.Itens);
        Assert.Equal(1, r.Feitos);
        Assert.Equal(10m, r.Total);
        Assert.Equal(0m, r.TotalFeito);
    }

    [Fact]
    public void Proxima_data_e_a_menor_de_hoje_em_diante_entre_os_pendentes()
    {
        var r = Resumo(L(1),
            I(1, data: Hoje.AddDays(-1)),             // passou
            I(1, feito: true, data: Hoje),            // feito não conta
            I(1, data: Hoje.AddDays(5)),
            I(1, data: Hoje.AddDays(2)));

        Assert.Equal(Hoje.AddDays(2), r.ProximaData);
    }

    [Fact]
    public void Proxima_data_da_consulta_ignora_itens_que_repetem_o_servico_calcula_a_ocorrencia()
    {
        var repete = I(1, data: Hoje.AddDays(1));
        repete.Repeticao = RepeticaoItem.Diaria;

        Assert.Null(Resumo(L(1), repete).ProximaData);
    }

    [Fact]
    public void Lista_vazia_tem_tudo_zerado_e_sem_proxima_data()
    {
        var r = Resumo(L(1));

        Assert.Equal(0, r.Itens);
        Assert.Equal(0m, r.Total);
        Assert.Null(r.ProximaData);
    }

    [Fact]
    public void O_EF_traduz_o_resumo_numa_consulta_so()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);

        var sql = ResumoListas.Projetar(contexto.Listas, contexto.ItensLista, Hoje).ToQueryString();

        Assert.StartsWith("-- @", sql);
        Assert.Contains("FROM listas", sql);
        Assert.Contains("min(", sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cada_lista_so_soma_os_proprios_itens()
    {
        var resumos = ResumoListas.Projetar(new[] { L(1), L(2) }.AsQueryable(),
            new[] { I(1, valor: 10m), I(2, valor: 1m), I(2, valor: 2m) }.AsQueryable(), Hoje).ToList();

        Assert.Equal(10m, resumos.Single(r => r.Lista.Id == 1).Total);
        Assert.Equal(3m, resumos.Single(r => r.Lista.Id == 2).Total);
    }
}
