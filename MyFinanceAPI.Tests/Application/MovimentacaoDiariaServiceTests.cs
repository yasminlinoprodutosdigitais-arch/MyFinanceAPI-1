using Moq;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Lançamento com banco e tipo de cartão ajusta o saldo do banco: criar aplica, excluir desfaz,
/// editar desfaz o lançamento antigo e aplica o novo — inclusive trocando o tipo (Desconto/Crédito)
/// ou o banco.
/// </summary>
public class MovimentacaoDiariaServiceTests
{
    private const int UserId = 7;
    private const int BancoA = 1;
    private const int BancoB = 2;

    private readonly Mock<IMovimentacaoDiariaRepository> _movRepo = new();
    private readonly Mock<IBancoRepository> _bancoRepo = new();
    private readonly Dictionary<int, Banco> _bancos = new()
    {
        [BancoA] = new Banco { Id = BancoA, UserId = UserId, SaldoInicial = 90 },
        [BancoB] = new Banco { Id = BancoB, UserId = UserId, SaldoInicial = 500 },
    };

    /// <summary>Como no EF: o repositório devolve a instância rastreada, que o Atualizar altera.</summary>
    private readonly MovimentacaoDiaria _gravada = new()
    {
        Id = 10, UserId = UserId, BancoId = BancoA, TipoCartaoId = 2, Valor = 10, TipoLancamento = "Desconto"
    };

    public MovimentacaoDiariaServiceTests()
    {
        _bancoRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), UserId))
            .ReturnsAsync((int id, int _) => _bancos.GetValueOrDefault(id));
        _bancoRepo.Setup(r => r.UpdateSaldo(It.IsAny<int>(), It.IsAny<decimal>(), UserId))
            .Callback((int id, decimal saldo, int _) => _bancos[id].SaldoInicial = saldo)
            .ReturnsAsync(true);

        _movRepo.Setup(r => r.ObterPorIdAsync(_gravada.Id, UserId)).ReturnsAsync(_gravada);
        _movRepo.Setup(r => r.AtualizarAsync(It.IsAny<MovimentacaoDiaria>(), UserId))
            .Callback((MovimentacaoDiaria e, int _) =>
            {
                _gravada.BancoId = e.BancoId;
                _gravada.TipoCartaoId = e.TipoCartaoId;
                _gravada.Valor = e.Valor;
                _gravada.TipoLancamento = e.TipoLancamento;
            })
            .Returns(Task.CompletedTask);
    }

    private MovimentacaoDiariaService CriarService() => new(_movRepo.Object, _bancoRepo.Object, MapperFactory.Create());

    private static MovimentacaoDiariaDTO Lancamento(int bancoId, string tipo, decimal valor, int? tipoCartaoId = 2) =>
        new() { Id = 10, BancoId = bancoId, TipoCartaoId = tipoCartaoId, Valor = valor, TipoLancamento = tipo, DataMovimentacao = new DateOnly(2026, 10, 13) };

    [Fact]
    public async Task Criar_desconto_tira_do_saldo()
    {
        await CriarService().AdicionarAsync(Lancamento(BancoA, "Desconto", 10), UserId);

        Assert.Equal(80, _bancos[BancoA].SaldoInicial);
    }

    [Fact]
    public async Task Editar_valor_ajusta_pela_diferenca()
    {
        await CriarService().AtualizarAsync(Lancamento(BancoA, "Desconto", 15), UserId);

        Assert.Equal(85, _bancos[BancoA].SaldoInicial);
    }

    [Fact]
    public async Task Editar_trocando_desconto_por_credito_grava_o_tipo_e_corrige_o_saldo()
    {
        await CriarService().AtualizarAsync(Lancamento(BancoA, "Crédito", 10), UserId);

        // Desfaz o desconto de 10 (90 → 100) e aplica o crédito de 10 (→ 110).
        Assert.Equal(110, _bancos[BancoA].SaldoInicial);
        _movRepo.Verify(r => r.AtualizarAsync(It.Is<MovimentacaoDiaria>(m => m.TipoLancamento == "Crédito"), UserId), Times.Once);
    }

    [Fact]
    public async Task Editar_trocando_de_banco_devolve_ao_antigo_e_tira_do_novo()
    {
        await CriarService().AtualizarAsync(Lancamento(BancoB, "Desconto", 10), UserId);

        Assert.Equal(100, _bancos[BancoA].SaldoInicial);
        Assert.Equal(490, _bancos[BancoB].SaldoInicial);
    }

    [Fact]
    public async Task Excluir_desfaz_o_lancamento()
    {
        await CriarService().RemoverAsync(_gravada.Id, UserId);

        Assert.Equal(100, _bancos[BancoA].SaldoInicial);
        _movRepo.Verify(r => r.RemoverAsync(_gravada.Id, UserId), Times.Once);
    }

    [Fact]
    public async Task Sem_tipo_de_cartao_nao_mexe_no_saldo_nem_ao_criar_nem_ao_excluir()
    {
        await CriarService().AdicionarAsync(Lancamento(BancoA, "Desconto", 10, tipoCartaoId: null), UserId);

        _gravada.TipoCartaoId = null;
        await CriarService().RemoverAsync(_gravada.Id, UserId);

        Assert.Equal(90, _bancos[BancoA].SaldoInicial);
        _bancoRepo.Verify(r => r.UpdateSaldo(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<int>()), Times.Never);
    }
}
