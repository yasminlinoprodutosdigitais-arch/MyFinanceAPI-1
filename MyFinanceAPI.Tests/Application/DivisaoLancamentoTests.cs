using Moq;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #76, fase 3 (08/11/2026): dividir um lançamento em partes com valor, categoria e pessoa.</summary>
public class DivisaoLancamentoTests
{
    private const int UserId = 7;
    private const int Carlos = 88;
    private const int Ana = 89;

    private readonly Mock<IExtratoBancarioItemRepository> _itens = new();
    private readonly Mock<IPessoaMovimentacaoRepository> _pessoas = new();
    private readonly Mock<IRegraClassificacaoRepository> _regras = new();
    private readonly ExtratoBancarioItem _compra = new()
    {
        Id = 10, UsuarioId = UserId, Valor = 300, TipoLancamento = TipoLancamentoExtrato.Saida, DataMovimentacao = new DateOnly(2026, 10, 5),
        PessoaMovimentacaoId = Carlos, Descricao = "Mercado"
    };
    private IReadOnlyList<ExtratoItemParte>? _gravadas;

    public DivisaoLancamentoTests()
    {
        _itens.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(_compra);
        _itens.Setup(r => r.SubstituirPartesAsync(10, It.IsAny<IReadOnlyList<ExtratoItemParte>>()))
            .Callback<int, IReadOnlyList<ExtratoItemParte>>((_, p) => _gravadas = p).Returns(Task.CompletedTask);
        _pessoas.Setup(r => r.ObterPorIdAsync(Carlos, UserId)).ReturnsAsync(new PessoaMovimentacao { Id = Carlos, NomePessoa = "CARLOS" });
    }

    private Task Dividir(params ExtratoItemParteDTO[] partes) =>
        new ExtratoBancarioItemService(_itens.Object, _pessoas.Object, _regras.Object, MapperFactory.Create()).DividirAsync(10, partes, UserId);

    private static ExtratoItemParteDTO Parte(decimal valor, int? categoria = 3, int? pessoa = null) =>
        new() { Valor = valor, CategoriaId = categoria, PessoaMovimentacaoId = pessoa };

    [Fact]
    public async Task Grava_as_partes_em_ordem_com_o_dono()
    {
        await Dividir(Parte(200), Parte(100, 4, Carlos));

        Assert.Equal([(200m, 3, (int?)null, 0), (100m, 4, Carlos, 1)],
            _gravadas!.Select(p => (p.Valor, p.CategoriaId!.Value, p.PessoaMovimentacaoId, p.Ordem)));
        Assert.All(_gravadas!, p => Assert.Equal(UserId, p.UsuarioId));
    }

    [Fact]
    public async Task Recusa_categoria_de_outro_usuario_sem_gravar()
    {
        _itens.Setup(r => r.TemCategoriaForaDoUsuarioAsync(It.Is<IReadOnlyCollection<int>>(c => c.Contains(4)), UserId)).ReturnsAsync(true);

        await Assert.ThrowsAsync<RegraNegocioException>(() => Dividir(Parte(200), Parte(100, 4)));

        Assert.Null(_gravadas);
    }

    [Fact]
    public async Task Lista_vazia_desfaz()
    {
        await Dividir();
        Assert.Empty(_gravadas!);
    }

    [Theory]
    [InlineData(300, 1, "pelo menos 2")]
    [InlineData(100, 2, "soma das partes")]
    public async Task Recusa_partes_invalidas(decimal valor, int quantas, string mensagem)
    {
        var partes = Enumerable.Range(0, quantas).Select(_ => Parte(valor)).ToArray();
        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() => Dividir(partes));
        Assert.Contains(mensagem, ex.Message);
        Assert.Null(_gravadas);
    }

    [Fact]
    public async Task Recusa_entrada_valor_zero_e_pessoa_de_outro_usuario()
    {
        await Assert.ThrowsAsync<RegraNegocioException>(() => Dividir(Parte(300), Parte(0)));
        await Assert.ThrowsAsync<RegraNegocioException>(() => Dividir(Parte(200), Parte(100, 4, 999)));
        _compra.TipoLancamento = TipoLancamentoExtrato.Entrada;
        await Assert.ThrowsAsync<RegraNegocioException>(() => Dividir(Parte(200), Parte(100)));
    }

    [Fact]
    public async Task Recusa_tirar_da_divisao_quem_ja_tem_pagamento_abatendo()
    {
        _itens.Setup(r => r.PessoasQueAbatemAsync(10)).ReturnsAsync(new int?[] { Carlos });

        await Assert.ThrowsAsync<RegraNegocioException>(() => Dividir(Parte(200), Parte(100)));
        await Dividir(Parte(200), Parte(100, 4, Carlos));
        Assert.NotNull(_gravadas);
    }

    [Fact]
    public async Task Mudar_o_valor_de_lancamento_dividido_e_recusado()
    {
        _compra.Partes.Add(new ExtratoItemParte { Valor = 300 });
        var service = new ExtratoBancarioItemService(_itens.Object, _pessoas.Object, _regras.Object, MapperFactory.Create());

        await Assert.ThrowsAsync<RegraNegocioException>(() => service.UpdateAsync(
            new ExtratoBancarioItemDTO { Id = 10, Valor = 250, TipoLancamento = _compra.TipoLancamento, DataMovimentacao = _compra.DataMovimentacao }, UserId));
    }

    // ---------- acertos ----------

    private static PessoaMovimentacao P(int id, string nome) => new() { Id = id, NomePessoa = nome };

    [Fact]
    public void Acertos_valem_pela_pessoa_de_cada_parte_e_somam_as_da_mesma_pessoa()
    {
        var carlos = P(Carlos, "CARLOS");
        var ana = P(Ana, "ANA");
        _compra.PessoaMovimentacao = carlos;
        _compra.Partes.AddRange([
            new ExtratoItemParte { Valor = 150 },                                                   // sem pessoa: é do dono
            new ExtratoItemParte { Valor = 60, PessoaMovimentacaoId = Carlos, PessoaMovimentacao = carlos },
            new ExtratoItemParte { Valor = 40, PessoaMovimentacaoId = Carlos, PessoaMovimentacao = carlos },
            new ExtratoItemParte { Valor = 50, PessoaMovimentacaoId = Ana, PessoaMovimentacao = ana }
        ]);
        var pagamento = new ExtratoBancarioItem
        {
            Id = 11, UsuarioId = UserId, Valor = 100, TipoLancamento = TipoLancamentoExtrato.Entrada, DataMovimentacao = new DateOnly(2026, 10, 9),
            PessoaMovimentacaoId = Carlos, PessoaMovimentacao = carlos, AbateItemId = 10
        };

        var acertos = AcertosService.Montar([_compra, pagamento], null, null, null);

        var doCarlos = acertos.Pessoas.Single(p => p.PessoaId == Carlos);
        var compra = Assert.Single(doCarlos.Compras);
        Assert.Equal((10, 100m, 100m, true), (compra.Id, compra.Valor, compra.Pago, compra.Quitada));
        Assert.Equal(0, doCarlos.Falta);
        var daAna = acertos.Pessoas.Single(p => p.PessoaId == Ana);
        Assert.Equal((50m, 50m), (daAna.Devido, daAna.Falta));   // o pagamento do Carlos não quita a parte da Ana
        Assert.Equal(50m, acertos.Totais.FaltaReceber);
    }

    [Fact]
    public async Task Pagamento_abate_compra_dividida_de_quem_tem_parte()
    {
        _compra.Partes.Add(new ExtratoItemParte { Valor = 300, PessoaMovimentacaoId = Ana });
        var service = new ExtratoBancarioItemService(_itens.Object, _pessoas.Object, _regras.Object, MapperFactory.Create());
        _pessoas.Setup(r => r.VerificaPossuiPessoa(It.IsAny<string?>(), UserId)).ReturnsAsync([]);
        _itens.Setup(r => r.CreateAsync(It.IsAny<ExtratoBancarioItem>())).ReturnsAsync((ExtratoBancarioItem i) => i);
        _regras.Setup(r => r.ListarAtivasAsync(UserId)).ReturnsAsync([]);
        _pessoas.Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), UserId)).ReturnsAsync((int id, int _) => P(id, "X"));

        ExtratoBancarioItemDTO Pagamento(int pessoa) => new()
        {
            DataMovimentacao = new DateOnly(2026, 10, 9), Valor = 50, TipoLancamento = TipoLancamentoExtrato.Entrada, BancoId = 5, AbateItemId = 10,
            PessoaMovimentacao = new MyFinanceAPI.Application.DTO.PessoaMovimentacaoDTO { Id = pessoa, NomePessoa = "X" }
        };

        await service.AddAsync(Pagamento(Ana), UserId);
        await Assert.ThrowsAsync<RegraNegocioException>(() => service.AddAsync(Pagamento(Carlos), UserId));   // a do lançamento não vale mais
    }
}
