using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #76, fase 5: resumo do mês por categoria.</summary>
public class ResumoCategoriasServiceTests
{
    private static readonly Categoria Comida = new() { Id = 1, Nome = "Alimentação" };
    private static readonly Categoria Beleza = new() { Id = 2, Nome = "Beleza" };
    private static readonly Categoria Fatura = new() { Id = 3, Nome = "Cartão de crédito" };
    private static readonly TipoCartao Credito = new() { Id = 2, EhCredito = true };

    private static ExtratoBancarioItem Item(string data, decimal valor, Categoria? c = null, string tipo = TipoLancamentoExtrato.Saida) => new()
    {
        DataMovimentacao = DateOnly.Parse(data), Valor = valor, TipoLancamento = tipo, Categoria = c, CategoriaId = c?.Id
    };

    [Fact]
    public void Soma_as_saidas_por_categoria_em_ordem_de_valor_e_sem_entradas()
    {
        var r = ResumoCategoriasService.Montar([
            Item("2026-10-05", 100, Comida), Item("2026-10-06", 300, Beleza), Item("2026-10-07", 100),
            Item("2026-10-08", 999, Comida, TipoLancamentoExtrato.Entrada)
        ], "2026-10");

        Assert.Equal(500, r.Total);
        Assert.Equal([("Beleza", 300m, 60m), ("Alimentação", 100m, 20m), ("Sem categoria", 100m, 20m)],
            r.Categorias.Select(c => (c.Nome, c.Valor, c.Percentual)).Take(3).OrderByDescending(x => x.Valor));
        Assert.Null(r.MediaAnterior);
    }

    [Fact]
    public void Cartao_conta_pela_fatura()
    {
        var compra = Item("2026-09-28", 50, Comida);
        compra.TipoCartao = Credito;
        compra.NumeroFatura = "2026-10";

        Assert.Equal(50, ResumoCategoriasService.Montar([compra], "2026-10").Total);
        Assert.Equal(0, ResumoCategoriasService.Montar([compra], "2026-09").Total);
    }

    [Fact]
    public void De_outra_pessoa_e_pagamento_de_fatura_ficam_a_parte()
    {
        var doCarlos = Item("2026-10-05", 40, Comida);
        doCarlos.PessoaMovimentacao = new PessoaMovimentacao { EntraNosAcertos = true };
        var meu = Item("2026-10-05", 10, Comida);
        meu.PessoaMovimentacao = new PessoaMovimentacao { EhTitular = true, EntraNosAcertos = true };

        var r = ResumoCategoriasService.Montar([doCarlos, meu, Item("2026-10-05", 500, Fatura)], "2026-10");

        Assert.Equal((10m, 40m, 500m), (r.Total, r.DeOutrasPessoas, r.ForaDoResumo));
    }

    [Fact]
    public void Evolucao_tira_pagamento_de_fatura_e_devolve_os_meses_em_ordem_mesmo_vazios()
    {
        var compra = Item("2026-09-28", 50, Comida);
        compra.TipoCartao = Credito;
        compra.NumeroFatura = "2026-10";

        var r = ResumoCategoriasService.MontarEvolucao([
            compra, Item("2026-10-05", 500, Fatura), Item("2026-10-06", 1000, null, TipoLancamentoExtrato.Entrada),
            Item("2026-08-06", 10, Comida), Item("2026-11-01", 77, Comida)
        ], "2026-10", 3);

        Assert.Equal([("2026-08", 0m, 10m, -10m), ("2026-09", 0m, 0m, 0m), ("2026-10", 1000m, 50m, 950m)],
            r.Meses.Select(m => (m.Mes, m.Entradas, m.Saidas, m.Resultado)));
    }

    [Fact]
    public void Evolucao_de_dividido_tira_so_a_parte_de_fatura()
    {
        var d = Item("2026-10-05", 300);
        d.Partes.AddRange([
            new ExtratoItemParte { Valor = 200, CategoriaId = 1, Categoria = Comida },
            new ExtratoItemParte { Valor = 100, CategoriaId = 3, Categoria = Fatura }
        ]);

        Assert.Equal(200, ResumoCategoriasService.MontarEvolucao([d], "2026-10", 1).Meses.Single().Saidas);
    }

    [Theory]
    [InlineData("2026-13", 6)]
    [InlineData("2026-10", 0)]
    [InlineData("2026-10", 25)]
    public async Task Evolucao_recusa_mes_ou_janela_invalidos(string mes, int meses)
    {
        var servico = new ResumoCategoriasService(null!);
        await Assert.ThrowsAsync<RegraNegocioException>(() => servico.ObterEvolucaoAsync(1, mes, meses));
    }

    [Fact]
    public void Dividido_conta_pelas_partes()
    {
        var d = Item("2026-10-05", 300);
        d.Partes.AddRange([
            new ExtratoItemParte { Valor = 200, CategoriaId = 1, Categoria = Comida },
            new ExtratoItemParte { Valor = 100, CategoriaId = 2, Categoria = Beleza, PessoaMovimentacao = new PessoaMovimentacao { EntraNosAcertos = true } }
        ]);

        var r = ResumoCategoriasService.Montar([d], "2026-10");

        Assert.Equal((200m, 100m), (r.Total, r.DeOutrasPessoas));
        Assert.Equal("Alimentação", Assert.Single(r.Categorias).Nome);
    }

    private static IEnumerable<ExtratoBancarioItem> Mes(string mes, Categoria c, int quantos = 5) =>
        Enumerable.Range(1, quantos).Select(d => Item($"{mes}-{d:00}", 20, c));

    [Fact]
    public void Compara_com_a_media_dos_3_meses_anteriores_com_movimento()
    {
        var r = ResumoCategoriasService.Montar([
            .. Mes("2026-05", Beleza),                 // há 3 meses completos mais recentes: fica de fora
            .. Mes("2026-06", Comida),
            Item("2026-07-05", 9000, Comida),          // mês com 1 lançamento: histórico incompleto, não entra
            .. Mes("2026-08", Comida),
            .. Mes("2026-09", Beleza),
            Item("2026-10-05", 130, Comida)
        ], "2026-10");

        Assert.Equal(3, r.MesesComparados);
        Assert.Equal(100, r.MediaAnterior);
        Assert.Equal(30, r.VariacaoTotal);
        var comida = Assert.Single(r.Categorias);
        Assert.Equal(66.67m, comida.Media);
        Assert.Equal(95, comida.Variacao);
    }

    [Fact]
    public void Sem_3_meses_com_movimento_nao_compara_e_volta_sozinho_quando_tiver()
    {
        List<ExtratoBancarioItem> itens = [.. Mes("2026-07", Comida), .. Mes("2026-08", Comida), Item("2026-09-05", 25, Comida), Item("2026-10-05", 1000, Comida)];

        var sem = ResumoCategoriasService.Montar(itens, "2026-10");
        Assert.Equal((0, (decimal?)null, (decimal?)null), (sem.MesesComparados, sem.MediaAnterior, sem.VariacaoTotal));
        Assert.Null(sem.Categorias[0].Variacao);

        itens.AddRange(Mes("2026-09", Comida));
        Assert.Equal(3, ResumoCategoriasService.Montar(itens, "2026-10").MesesComparados);
    }

    [Fact]
    public async Task Mes_invalido_e_recusado()
    {
        var service = new ResumoCategoriasService(new Moq.Mock<MyFinanceAPI.Domain.Interfaces.IExtratoBancarioItemRepository>().Object);
        await Assert.ThrowsAsync<RegraNegocioException>(() => service.ObterAsync(1, "outubro"));
    }
}
