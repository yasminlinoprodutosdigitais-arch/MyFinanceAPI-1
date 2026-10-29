using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #67, parte 2 (back do #68): comparar duas listas da mesma série. A cascata tem que fechar no centavo:
/// preço + quantidade + novos − saíram = total atual − total anterior.
/// </summary>
public class ComparacaoListasTests
{
    private static ItemLista I(int id, string descricao, int? qtd, decimal? valor, int? origem = null, bool feito = false) =>
        new() { Id = id, Descricao = descricao, Quantidade = qtd, Valor = valor, ItemOrigemId = origem, Feito = feito };

    private static Lista L(int id, string nome, string modo = ModoValorLista.Unitario) => new() { Id = id, Nome = nome, ModoValor = modo };

    private static ComparacaoListasDTO Comparar(Lista anterior, ItemLista[] deAnterior, Lista atual, ItemLista[] deAtual,
        bool soFeitos = false, ItemLista[]? outrosDaSerie = null)
    {
        var origens = deAnterior.Concat(deAtual).Concat(outrosDaSerie ?? []).ToDictionary(i => i.Id, i => i.ItemOrigemId);
        return ComparacaoListas.Comparar(anterior, deAnterior, atual, deAtual, origens, soFeitos);
    }

    private static void CascataFecha(ComparacaoListasDTO c)
    {
        var e = c.Efeitos;
        Assert.Equal(c.Atual.Total - c.Anterior.Total, e.Preco + e.Quantidade + e.Novos - e.Sairam);
        Assert.Equal(c.Diferenca, c.Atual.Total - c.Anterior.Total);
    }

    [Fact]
    public void Decompoe_preco_e_quantidade_e_a_cascata_fecha()
    {
        var c = Comparar(
            L(1, "Compras out"), [I(10, "Arroz", 2, 6.50m), I(11, "Café", 1, 18.90m)],
            L(2, "Compras nov"), [I(20, "Arroz", 3, 7.10m, origem: 10), I(21, "Café", 1, 18.90m, origem: 11)]);

        Assert.Equal(31.90m, c.Anterior.Total);
        Assert.Equal(40.20m, c.Atual.Total);
        Assert.Equal(8.30m, c.Diferenca);
        Assert.Equal(1.20m, c.Efeitos.Preco);        // (7,10 − 6,50) × 2
        Assert.Equal(7.10m, c.Efeitos.Quantidade);   // (3 − 2) × 7,10
        Assert.Equal(26.02m, c.Percentual);
        var arroz = c.Itens.First();
        Assert.Equal("Arroz", arroz.Descricao);
        Assert.Equal(new[] { "PRECO_SUBIU", "QUANTIDADE" }, arroz.Mudancas);
        Assert.Equal(13.00m, arroz.Anterior!.Total);
        Assert.Equal(21.30m, arroz.Atual!.Total);
        Assert.Equal(63.85m, arroz.Percentual);
        Assert.Empty(c.Itens.Single(i => i.Descricao == "Café").Mudancas);
        CascataFecha(c);
    }

    [Fact]
    public void Novos_e_que_sairam_entram_pelo_total_do_item()
    {
        var c = Comparar(
            L(1, "out"), [I(10, "Arroz", 1, 5m), I(11, "Sabão", 2, 3.33m)],
            L(2, "nov"), [I(20, "Arroz", 1, 5m, origem: 10), I(21, "Leite", 6, 4.79m)]);

        Assert.Equal(28.74m, c.Efeitos.Novos);
        Assert.Equal(6.66m, c.Efeitos.Sairam);
        Assert.Equal(new[] { "NOVO" }, c.Itens.Single(i => i.Descricao == "Leite").Mudancas);
        var sabao = c.Itens.Single(i => i.Descricao == "Sabão");
        Assert.Equal(new[] { "SAIU" }, sabao.Mudancas);
        Assert.Null(sabao.Atual);
        Assert.Equal(-6.66m, sabao.Diferenca);
        CascataFecha(c);
    }

    [Fact]
    public void Sem_origem_pareia_pelo_nome_sem_acento_caixa_ou_espacos()
    {
        var c = Comparar(
            L(1, "out"), [I(10, "  Feijão   Carioca ", 1, 8m)],
            L(2, "nov"), [I(20, "feijao carioca", 1, 9m)]);

        var item = Assert.Single(c.Itens);
        Assert.Equal(new[] { "PRECO_SUBIU" }, item.Mudancas);
        Assert.Equal(0m, c.Efeitos.Novos);
        CascataFecha(c);
    }

    [Fact]
    public void Origem_vale_mesmo_com_nome_trocado_e_atravessa_meses()
    {
        // out (10) → nov (20, renomeado) → dez (30): comparar out × dez pela raiz da corrente.
        var c = Comparar(
            L(1, "out"), [I(10, "Detergente", 1, 2.50m)],
            L(3, "dez"), [I(30, "Detergente Ypê", 1, 2.90m, origem: 20)],
            outrosDaSerie: [I(20, "Detergente Ypê", 1, 2.70m, origem: 10)]);

        var item = Assert.Single(c.Itens);
        Assert.Equal(0.40m, item.Diferenca);
        Assert.Equal("PRECO_SUBIU", item.Mudancas.Single());
        CascataFecha(c);
    }

    [Fact]
    public void Modo_total_leva_toda_a_diferenca_para_o_preco()
    {
        var c = Comparar(
            L(1, "Gastos out", ModoValorLista.Total), [I(10, "Luz", 1, 180m), I(11, "Água", 1, 90m)],
            L(2, "Gastos nov", ModoValorLista.Total), [I(20, "Luz", 1, 210.45m, origem: 10), I(21, "Água", 1, 85.10m, origem: 11)]);

        Assert.Equal(25.55m, c.Efeitos.Preco);
        Assert.Equal(0m, c.Efeitos.Quantidade);
        Assert.Equal(new[] { "PRECO_CAIU" }, c.Itens.Single(i => i.Descricao == "Água").Mudancas);
        CascataFecha(c);
    }

    [Fact]
    public void Base_ok_considera_so_os_itens_feitos_de_cada_lista()
    {
        var c = Comparar(
            L(1, "out"), [I(10, "Arroz", 1, 5m, feito: true), I(11, "Café", 1, 20m)],
            L(2, "nov"), [I(20, "Arroz", 1, 6m, origem: 10, feito: true), I(21, "Café", 1, 22m, origem: 11)],
            soFeitos: true);

        Assert.Equal(5m, c.Anterior.Total);
        Assert.Equal(6m, c.Atual.Total);
        Assert.Equal("Arroz", Assert.Single(c.Itens).Descricao);
        CascataFecha(c);
    }

    [Fact]
    public void Item_sem_valor_fica_fora_da_conta_e_vai_para_o_aviso_e_a_cascata_continua_fechando()
    {
        var c = Comparar(
            L(1, "out"), [I(10, "Arroz", 1, 5m), I(11, "Pão", 1, 7m), I(12, "Sal", 1, null)],
            L(2, "nov"), [I(20, "Arroz", 1, 5m, origem: 10), I(21, "Pão", 1, null, origem: 11), I(22, "Sal", 2, null, origem: 12), I(23, "Ovos", 1, null)]);

        Assert.Equal(new[] { "Ovos", "Pão", "Sal" }, c.SemValor);
        Assert.Equal(7m, c.Efeitos.Sairam);   // o Pão perdeu o valor: o que tinha sai da conta
        Assert.Equal(new[] { "QUANTIDADE" }, c.Itens.Single(i => i.Descricao == "Sal").Mudancas);
        CascataFecha(c);
    }

    [Fact]
    public void Sem_quantidade_conta_um_e_percentual_nulo_quando_o_anterior_e_zero()
    {
        var c = Comparar(L(1, "out"), [], L(2, "nov"), [I(20, "Arroz", null, 5m)]);

        Assert.Equal(5m, c.Atual.Total);
        Assert.Null(c.Percentual);
        Assert.Null(c.Itens.Single().Percentual);
        CascataFecha(c);
    }

    [Fact]
    public void Destaques_e_ordem_por_maior_impacto()
    {
        var c = Comparar(
            L(1, "out"), [I(10, "Arroz", 1, 5m), I(11, "Carne", 1, 60m), I(12, "Café", 1, 20m)],
            L(2, "nov"), [I(20, "Arroz", 1, 7.50m, origem: 10), I(21, "Carne", 1, 72m, origem: 11), I(22, "Café", 1, 15m, origem: 12)]);

        Assert.Equal(new[] { "Carne", "Café", "Arroz" }, c.Itens.Select(i => i.Descricao));
        Assert.Equal("Arroz", c.Destaques.MaiorAltaPreco!.Descricao);
        Assert.Equal(50m, c.Destaques.MaiorAltaPreco.Percentual);
        Assert.Equal("Carne", c.Destaques.MaiorImpacto!.Descricao);
        Assert.Equal(12m, c.Destaques.MaiorImpacto.Diferenca);
        Assert.Equal("Café", c.Destaques.MaiorQueda!.Descricao);
        Assert.Equal(-5m, c.Destaques.MaiorQueda.Diferenca);
    }

    [Fact]
    public void Cascata_fecha_no_centavo_em_listas_aleatorias()
    {
        var sorte = new Random(67);
        decimal? Valor() => sorte.Next(6) == 0 ? null : Math.Round((decimal)sorte.NextDouble() * 300, 2);
        int? Qtd() => sorte.Next(5) == 0 ? null : sorte.Next(1, 9);

        for (var caso = 0; caso < 300; caso++)
        {
            var modos = new[] { ModoValorLista.Unitario, ModoValorLista.Total };
            var anterior = Enumerable.Range(1, sorte.Next(0, 15))
                .Select(n => I(n, $"Item {n}", Qtd(), Valor(), feito: sorte.Next(2) == 0)).ToArray();
            var atual = anterior.Where(_ => sorte.Next(4) > 0)
                .Select(p => I(p.Id + 100, sorte.Next(3) == 0 ? $"Outro {p.Id}" : p.Descricao, Qtd(), Valor(),
                    origem: sorte.Next(2) == 0 ? p.Id : null, feito: sorte.Next(2) == 0))
                .Concat(Enumerable.Range(200, sorte.Next(0, 5)).Select(n => I(n, $"Novo {n}", Qtd(), Valor())))
                .ToArray();

            var c = Comparar(L(1, "a", modos[sorte.Next(2)]), anterior, L(2, "b", modos[sorte.Next(2)]), atual, soFeitos: sorte.Next(2) == 0);

            CascataFecha(c);
        }
    }

    [Fact]
    public void Listas_iguais_nao_tem_destaques()
    {
        var c = Comparar(L(1, "out"), [I(10, "Arroz", 1, 5m)], L(2, "nov"), [I(20, "Arroz", 1, 5m, origem: 10)]);

        Assert.Null(c.Destaques.MaiorAltaPreco);
        Assert.Null(c.Destaques.MaiorImpacto);
        Assert.Null(c.Destaques.MaiorQueda);
        Assert.Equal(0m, c.Diferenca);
    }
}
