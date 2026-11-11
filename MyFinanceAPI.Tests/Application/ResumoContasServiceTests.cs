using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #77: números por conta, iguais na listagem e no detalhe.</summary>
public class ResumoContasServiceTests
{
    private static readonly TipoCartao Corrente = new() { Id = 1, NomeTipoCartao = "Conta corrente", EhCredito = false };
    private static readonly TipoCartao Credito = new() { Id = 2, NomeTipoCartao = "Cartão de crédito", EhCredito = true };
    private static readonly Categoria Comida = new() { Id = 1, Nome = "Alimentação" };
    private static readonly Categoria Fatura = new() { Id = 3, Nome = "Cartão de crédito" };
    private static readonly PessoaMovimentacao Carlos = new() { Id = 20, NomePessoa = "CARLOS" };
    private static readonly PessoaMovimentacao Eu = new() { Id = 21, NomePessoa = "EU", EhTitular = true };
    private static readonly DateOnly Hoje = new(2026, 11, 20);
    private static readonly Dictionary<int, (decimal, decimal)> SemAcertos = new();

    private static Banco Conta(int id, string nome, TipoCartao tipo, bool ativo = true) =>
        new() { Id = id, NomeBanco = nome, TipoCartao = tipo, TipoCartaoId = tipo.Id, Ativo = ativo };

    private static ExtratoBancarioItem Item(int banco, string data, decimal valor, string tipo = TipoLancamentoExtrato.Saida,
        Categoria? c = null, PessoaMovimentacao? p = null, string? fatura = null) => new()
    {
        Id = Random.Shared.Next(1000, 999999), BancoId = banco, DataMovimentacao = DateOnly.Parse(data), Valor = valor, TipoLancamento = tipo,
        Categoria = c, CategoriaId = c?.Id, PessoaMovimentacao = p, PessoaMovimentacaoId = p?.Id,
        NumeroFatura = fatura, TipoCartao = fatura != null ? Credito : Corrente
    };

    private static ImportacaoResumo Importacao(int id, int banco, string mes, bool desfeita = false) =>
        new(id, banco, new DateTime(2026, 11, 5), "x.csv", DateOnly.Parse(mes + "-01"), 0, desfeita ? DateTime.UtcNow : null, null, null, 1, 0, 0, 0, null);

    [Fact]
    public void Conta_corrente_tem_entradas_saidas_e_resultado_e_os_totais_tiram_pagamento_de_fatura()
    {
        var r = ResumoContasService.Montar("2026-11", [Conta(1, "Caixa", Corrente)], [
            Item(1, "2026-11-05", 1000, TipoLancamentoExtrato.Entrada, Comida),
            Item(1, "2026-11-06", 300, c: Comida),
            Item(1, "2026-11-07", 200, c: Fatura),
            Item(1, "2026-10-07", 999, c: Comida)
        ], [], SemAcertos, Hoje);

        var caixa = r.Contas.Single();
        Assert.Equal((1000m, 500m, 500m, 3, 0), (caixa.Entradas, caixa.Saidas, caixa.Resultado, caixa.Quantidade, caixa.SemCategoria));
        Assert.Equal((1000m, 300m), (r.Entradas, r.Saidas));
        Assert.Null(r.FaltaReceber);
    }

    [Fact]
    public void Cartao_tem_fatura_pagamentos_sem_categoria_e_parcelas_que_continuam()
    {
        var parcelada = Item(2, "2026-10-28", 100, c: Comida, fatura: "2026-11");
        parcelada.ParcelaAtual = 2;
        parcelada.QuantidadeParcelas = 5;

        var r = ResumoContasService.Montar("2026-11", [Conta(2, "Nubank", Credito)], [
            parcelada,
            Item(2, "2026-10-29", 50, fatura: "2026-11"),
            Item(2, "2026-11-02", 30, TipoLancamentoExtrato.Entrada, Comida, fatura: "2026-11")
        ], [], SemAcertos, Hoje);

        var nu = r.Contas.Single();
        Assert.Equal((150m, 30m, 1, 300m), (nu.Fatura, nu.PagamentosEstornos, nu.SemCategoria, nu.ParcelasQueContinuam));
        Assert.Equal(1, r.ParaClassificar);
    }

    [Fact]
    public void Cartao_compartilhado_separa_minhas_de_outras_pessoas_e_soma_o_que_falta_receber()
    {
        var banco = Conta(2, "Nubank", Credito);
        banco.Compartilhado = true;
        banco.Pessoas.Add(new BancoPessoa { PessoaMovimentacao = Carlos });
        var doCarlos = Item(2, "2026-10-28", 80, c: Comida, p: Carlos, fatura: "2026-11");
        var compras = new Dictionary<int, (decimal, decimal)> { [doCarlos.Id] = (80m, 30m) };

        var r = ResumoContasService.Montar("2026-11", [banco], [
            doCarlos,
            Item(2, "2026-10-29", 20, c: Comida, p: Eu, fatura: "2026-11"),
            Item(2, "2026-10-30", 10, c: Comida, fatura: "2026-11")
        ], [], compras, Hoje);

        var nu = r.Contas.Single();
        Assert.Equal((110m, 30m, 80m, 50m), (nu.Fatura, nu.MinhasCompras, nu.DeOutrasPessoas, nu.FaltaReceber));
        Assert.Equal(["CARLOS"], nu.Pessoas);
        Assert.Equal(("CARLOS", 80m), (nu.PorPessoa.Single().Nome, nu.PorPessoa.Single().Valor));
        Assert.Equal(50m, r.FaltaReceber);
    }

    [Fact]
    public void Dividido_conta_cada_parte_pela_pessoa_dela()
    {
        var d = Item(2, "2026-10-28", 100, fatura: "2026-11");
        d.Partes.AddRange([
            new ExtratoItemParte { Valor = 60, CategoriaId = 1, PessoaMovimentacao = Carlos },
            new ExtratoItemParte { Valor = 40, CategoriaId = 1 }
        ]);

        var nu = ResumoContasService.Montar("2026-11", [Conta(2, "Nubank", Credito)], [d], [], SemAcertos, Hoje).Contas.Single();

        Assert.Equal((40m, 60m, 0), (nu.MinhasCompras, nu.DeOutrasPessoas, nu.SemCategoria));
    }

    [Theory]
    [InlineData(3, 10, "2026-11-02", false)] // fecha 03/11: ainda não fechou
    [InlineData(3, 10, "2026-11-03", true)]  // fechou e não importou
    [InlineData(28, 5, "2026-10-28", true)]  // fechamento depois do vencimento: fecha no mês anterior
    public void Fatura_nao_importada_pelo_fechamento(int fechamento, int vencimento, string hoje, bool falta)
    {
        var banco = Conta(2, "Nubank", Credito);
        banco.DiaFechamento = fechamento;
        banco.DiaVencimento = vencimento;

        var nu = ResumoContasService.Montar("2026-11", [banco], [], [], SemAcertos, DateOnly.Parse(hoje)).Contas.Single();

        Assert.Equal(falta, nu.FaltaImportar);
    }

    [Fact]
    public void Fatura_importada_ou_desfeita()
    {
        var banco = Conta(2, "Nubank", Credito);
        banco.DiaFechamento = 3;
        banco.DiaVencimento = 10;

        Assert.False(ResumoContasService.Montar("2026-11", [banco], [], [Importacao(1, 2, "2026-11")], SemAcertos, Hoje).Contas.Single().FaltaImportar);
        Assert.True(ResumoContasService.Montar("2026-11", [banco], [], [Importacao(1, 2, "2026-11", desfeita: true)], SemAcertos, Hoje).Contas.Single().FaltaImportar);
    }

    [Fact]
    public void Sem_fechamento_segue_a_regra_da_cobertura()
    {
        var banco = Conta(1, "Caixa", Corrente);
        var importacoes = new[] { Importacao(1, 1, "2026-09") };

        Assert.True(ResumoContasService.Montar("2026-10", [banco], [], importacoes, SemAcertos, Hoje).Contas.Single().FaltaImportar);
        Assert.False(ResumoContasService.Montar("2026-11", [banco], [], importacoes, SemAcertos, Hoje).Contas.Single().FaltaImportar); // mês corrente
        Assert.False(ResumoContasService.Montar("2026-08", [banco], [], importacoes, SemAcertos, Hoje).Contas.Single().FaltaImportar); // antes da primeira
    }

    [Fact]
    public void Conta_inativa_sem_lancamento_no_mes_nao_aparece()
    {
        var r = ResumoContasService.Montar("2026-11", [Conta(1, "Caixa", Corrente), Conta(3, "Antigo", Corrente, ativo: false)], [], [], SemAcertos, Hoje);
        Assert.Equal(["Caixa"], r.Contas.Select(c => c.Nome));
    }
}
