using Moq;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #45, parte 3 (05/11/2026): Acertos com pessoas. Devido = saídas da pessoa (cartão e conta), Recebido = entradas,
/// Falta = Devido − Recebido; pagamento pode abater uma compra; "Ainda vai cobrar" fica à parte e não conta duas vezes
/// a mesma compra parcelada; só pessoas com "Entra nos acertos"; o "Eu" vem à parte.
/// </summary>
public class AcertosServiceTests
{
    private const int UserId = 7;
    private const int Cartao = 3;
    private const int Conta = 5;

    private static readonly PessoaMovimentacao Carlos = new() { Id = 10, NomePessoa = "CARLOS" };
    private static readonly PessoaMovimentacao Agatha = new() { Id = 11, NomePessoa = "ÁGATHA" };
    private static readonly PessoaMovimentacao Eu = new() { Id = 1, NomePessoa = "EU", EhTitular = true };

    private int _id = 100;

    private ExtratoBancarioItem Item(PessoaMovimentacao? pessoa, string tipo, decimal valor, string data, int banco = Cartao,
        string? descricao = null, int? parcela = null, int? total = null, int? abate = null, string? fatura = null) => new()
    {
        Id = ++_id, UsuarioId = UserId, PessoaMovimentacao = pessoa, PessoaMovimentacaoId = pessoa?.Id, TipoLancamento = tipo,
        Valor = valor, DataMovimentacao = DateOnly.Parse(data), BancoId = banco, Banco = new Banco { Id = banco, NomeBanco = banco == Cartao ? "Nubank Cred" : "NuBank" },
        Descricao = descricao ?? $"Compra {_id}", ChaveDescricao = ExtratoBancarioService.GerarChaveDescricao(descricao ?? $"Compra {_id}"),
        EhParcelado = total != null, ParcelaAtual = parcela, QuantidadeParcelas = total, AbateItemId = abate, NumeroFatura = fatura
    };

    private const string S = TipoLancamentoExtrato.Saida;
    private const string E = TipoLancamentoExtrato.Entrada;

    [Fact]
    public void Devido_menos_recebido_e_o_que_falta_por_pessoa()
    {
        var itens = new[]
        {
            Item(Carlos, S, 100, "2026-10-03"), Item(Carlos, S, 50, "2026-10-10", banco: Conta),   // conta corrente também conta
            Item(Carlos, E, 30, "2026-10-20", banco: Conta),
            Item(Agatha, S, 80, "2026-10-05"), Item(Agatha, E, 80, "2026-10-25", banco: Conta),
        };

        var acertos = AcertosService.Montar(itens, null, null, null);

        var carlos = acertos.Pessoas.Single(p => p.PessoaId == Carlos.Id);
        Assert.Equal((150m, 30m, 120m, false), (carlos.Devido, carlos.Recebido, carlos.Falta, carlos.Quitado));
        var agatha = acertos.Pessoas.Single(p => p.PessoaId == Agatha.Id);
        Assert.True(agatha.Quitado);
        Assert.Equal((230m, 110m, 120m), (acertos.Totais.Devido, acertos.Totais.Recebido, acertos.Totais.FaltaReceber));
    }

    [Fact]
    public void Toda_pessoa_cadastrada_entra_e_o_Eu_a_parte()
    {
        // Card #79: pessoa é só quem usou o cartão, cadastrada à mão; sem pessoa (o mercado) fica de fora.
        var itens = new[]
        {
            Item(Carlos, S, 100, "2026-10-03"), Item(Agatha, S, 300, "2026-10-04"), Item(null, S, 20, "2026-10-04"),
            Item(Eu, S, 12, "2026-10-04", descricao: "IOF")
        };

        var acertos = AcertosService.Montar(itens, null, null, null);

        Assert.Equal([Carlos.Id, Agatha.Id], acertos.Pessoas.Select(p => p.PessoaId).Order());
        Assert.Equal(400m, acertos.Totais.Devido);
        Assert.NotNull(acertos.Eu);
        Assert.Equal(12m, acertos.Eu!.Devido);
        Assert.Equal("Eu", acertos.Eu.Nome);
    }

    [Fact]
    public void Pagamento_que_abate_uma_compra_mostra_o_pago_dela()
    {
        var compra = Item(Carlos, S, 120, "2026-10-03", descricao: "Tênis");
        var outra = Item(Carlos, S, 40, "2026-10-04");
        var itens = new[]
        {
            compra, outra,
            Item(Carlos, E, 50, "2026-10-10", banco: Conta, abate: compra.Id),
            Item(Carlos, E, 70, "2026-10-12", banco: Conta, abate: compra.Id),
            Item(Carlos, E, 10, "2026-10-15", banco: Conta)                       // saldo geral
        };

        var carlos = AcertosService.Montar(itens, null, null, null).Pessoas.Single();

        var tenis = carlos.Compras.Single(c => c.Id == compra.Id);
        Assert.Equal((120m, true), (tenis.Pago, tenis.Quitada));
        Assert.Equal((0m, false), (carlos.Compras.Single(c => c.Id == outra.Id).Pago, carlos.Compras.Single(c => c.Id == outra.Id).Quitada));
        Assert.Equal(30m, carlos.Falta);
        Assert.Equal(3, carlos.Pagamentos.Count);
        Assert.Equal(compra.Id, carlos.Pagamentos.First(p => p.Valor == 50).AbateItemId);
    }

    [Fact]
    public void Ainda_vai_cobrar_usa_a_maior_parcela_importada_da_mesma_compra()
    {
        var itens = new[]
        {
            Item(Carlos, S, 100, "2026-09-03", descricao: "Geladeira - Parcela 1/4", parcela: 1, total: 4, fatura: "2026-09"),
            Item(Carlos, S, 100, "2026-10-03", descricao: "Geladeira - Parcela 2/4", parcela: 2, total: 4, fatura: "2026-10"),
            Item(Carlos, S, 30, "2026-10-08", descricao: "Fone - Parcela 1/3", parcela: 1, total: 3, fatura: "2026-10"),
        };

        var carlos = AcertosService.Montar(itens, null, null, null).Pessoas.Single();

        // Geladeira: faltam 2 de 100 (nov e dez); fone: faltam 2 de 30.
        Assert.Equal(260m, carlos.AindaVaiCobrar);
        Assert.Equal(260m, AcertosService.Montar(itens, null, null, null).Totais.AindaVaiCobrar);
        Assert.Equal(230m, carlos.Devido); // o já cobrado não muda
        Assert.Equal(["2026-11", "2026-11", "2026-12", "2026-12"], carlos.ProximasParcelas.Select(p => p.Mes));
        var geladeira = carlos.ProximasParcelas.First(p => p.Descricao!.StartsWith("Geladeira"));
        Assert.Equal(("2026-11", 3, 4), (geladeira.Mes, geladeira.Parcela, geladeira.QuantidadeParcelas));
    }

    [Fact]
    public void Periodo_filtra_devido_e_recebido_mas_o_pago_da_compra_conta_qualquer_data()
    {
        var compra = Item(Carlos, S, 100, "2026-10-03");
        var itens = new[] { compra, Item(Carlos, S, 60, "2026-11-03"), Item(Carlos, E, 100, "2026-11-20", banco: Conta, abate: compra.Id) };

        var outubro = AcertosService.Montar(itens, new DateOnly(2026, 10, 1), new DateOnly(2026, 11, 1), null).Pessoas.Single();

        Assert.Equal((100m, 0m), (outubro.Devido, outubro.Recebido));
        Assert.True(outubro.Compras.Single().Quitada);
    }

    [Fact]
    public void Filtro_de_conta_leva_as_compras_dela_e_os_pagamentos_que_as_abatem()
    {
        var noCartao = Item(Carlos, S, 100, "2026-10-03", banco: Cartao);
        var itens = new[]
        {
            noCartao, Item(Carlos, S, 40, "2026-10-04", banco: Conta),
            Item(Carlos, E, 60, "2026-10-10", banco: Conta, abate: noCartao.Id),
            Item(Carlos, E, 15, "2026-10-11", banco: Conta)   // saldo geral: só sem filtro de conta
        };

        var cartao = AcertosService.Montar(itens, null, null, Cartao).Pessoas.Single();

        Assert.Equal((100m, 60m, 40m), (cartao.Devido, cartao.Recebido, cartao.Falta));
    }

    [Fact]
    public async Task Servico_le_os_itens_do_usuario_e_converte_o_periodo()
    {
        var repo = new Mock<IExtratoBancarioItemRepository>();
        repo.Setup(r => r.ListarParaAcertosAsync(UserId)).ReturnsAsync([Item(Carlos, S, 100, "2026-10-03"), Item(Carlos, S, 50, "2026-12-03")]);

        var acertos = await new AcertosService(repo.Object).ObterAsync(UserId, "2026-10", "2026-11", null);

        Assert.Equal(100m, acertos.Totais.Devido);
    }

    [Theory]
    [InlineData("2026-13", null)]
    [InlineData("out/2026", null)]
    [InlineData("2026-11", "2026-10")]
    public async Task Periodo_invalido_e_regra_de_negocio(string? inicio, string? fim)
    {
        var repo = new Mock<IExtratoBancarioItemRepository>();

        await Assert.ThrowsAsync<RegraNegocioException>(() => new AcertosService(repo.Object).ObterAsync(UserId, inicio, fim, null));
    }
}
