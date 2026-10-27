using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Conta e vencimentos são gravados em passos separados; precisam valer juntos (BE-P1-05).
/// </summary>
public class ContaServiceTests
{
    private const int UserId = 7;
    private const int AccountId = 100;

    private readonly Mock<IContaRepository> _repo = new();
    private readonly UnitOfWorkFalso _uow = new();
    private List<ContaVencimento>? _vencimentosCriados;

    public ContaServiceTests()
    {
        _repo.Setup(r => r.Create(It.IsAny<Conta>()))
            .Callback(() => _uow.Registrar("Account.Create"))
            .ReturnsAsync((Conta a) => { a.Id = AccountId; return a; });
        _repo.Setup(r => r.Update(It.IsAny<Conta>(), UserId))
            .Callback(() => _uow.Registrar("Account.Update"))
            .ReturnsAsync((Conta a, int _) => a);
        _repo.Setup(r => r.RemoveContaVencimento(AccountId, UserId))
            .Callback(() => _uow.Registrar("Vencimentos.Remove"))
            .Returns(Task.CompletedTask);
        _repo.Setup(r => r.CreateContaVencimento(It.IsAny<List<ContaVencimento>>()))
            .Callback<List<ContaVencimento>>(v =>
            {
                _uow.Registrar("Vencimentos.Create");
                _vencimentosCriados = v;
            })
            .Returns(Task.CompletedTask);
    }

    private ContaService CriarService() => new(_repo.Object, MapperFactory.Create(), _uow);

    private static ContaDTO Conta(int id = 0) => new()
    {
        Id = id,
        Nome = "Internet",
        Valor = 120m,
        CategoriaId = 4,
        Ativo = true,
        DataOperacao = new List<int> { 5, 20 },
    };

    [Fact]
    public async Task Add_cria_conta_e_vencimentos_na_mesma_operacao()
    {
        var criada = await CriarService().Add(Conta(), UserId);

        Assert.Equal(AccountId, criada.Id);
        Assert.Equal(new[] { 5, 20 }, _vencimentosCriados!.Select(v => v.Dia));
        Assert.All(_vencimentosCriados!, v => Assert.Equal(AccountId, v.ContaId));
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Update_atualiza_conta_e_troca_vencimentos_na_mesma_operacao()
    {
        await CriarService().Update(Conta(AccountId), UserId);

        _repo.Verify(r => r.Update(It.IsAny<Conta>(), UserId), Times.Once);
        _repo.Verify(r => r.RemoveContaVencimento(AccountId, UserId), Times.Once);
        Assert.Equal(new[] { 5, 20 }, _vencimentosCriados!.Select(v => v.Dia));
        Assert.Empty(_uow.Fora);
    }

    // Card #22: parcelas fora do CHECK do banco (total ≥ 1, atual de 0 até o total) voltam com mensagem clara,
    // em vez do erro genérico de constraint.
    [Theory]
    [InlineData(null, 1)]
    [InlineData(0, 0)]
    [InlineData(3, 4)]
    [InlineData(3, -1)]
    [InlineData(3, null)]
    public async Task Parcelada_com_parcelas_invalidas_e_recusada(int? total, int? atual)
    {
        var dto = Conta();
        dto.EhParcelado = true;
        dto.QuantidadeParcelas = total;
        dto.ParcelaAtual = atual;

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().Add(dto, UserId));

        Assert.Contains("parcela", erro.Message, StringComparison.OrdinalIgnoreCase);
        _repo.Verify(r => r.Create(It.IsAny<Conta>()), Times.Never);
    }

    [Fact]
    public async Task Parcelas_invalidas_tambem_sao_recusadas_na_edicao()
    {
        var dto = Conta(AccountId);
        dto.EhParcelado = true;
        dto.QuantidadeParcelas = 10;
        dto.ParcelaAtual = 11;

        await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().Update(dto, UserId));

        _repo.Verify(r => r.Update(It.IsAny<Conta>(), It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(10, 1)]
    [InlineData(10, 10)]
    public async Task Parcelada_dentro_do_limite_grava(int total, int atual)
    {
        var dto = Conta();
        dto.EhParcelado = true;
        dto.QuantidadeParcelas = total;
        dto.ParcelaAtual = atual;

        await CriarService().Add(dto, UserId);

        _repo.Verify(r => r.Create(It.IsAny<Conta>()), Times.Once);
    }

    [Fact]
    public async Task Nao_parcelada_nao_olha_as_parcelas()
    {
        var dto = Conta();
        dto.QuantidadeParcelas = 0;
        dto.ParcelaAtual = 0;

        await CriarService().Add(dto, UserId);

        _repo.Verify(r => r.Create(It.IsAny<Conta>()), Times.Once);
    }

    [Fact]
    public async Task Excluir_conta_com_lancamentos_e_recusado_sugerindo_inativar()
    {
        _repo.Setup(r => r.EmUsoAsync(AccountId, UserId)).ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().Remove(AccountId, UserId));

        Assert.Contains("lançamentos", erro.Message);
        Assert.Contains("Inative", erro.Message);
        _repo.Verify(r => r.Remove(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Valor_fixo_e_gravado()
    {
        var dto = Conta();
        dto.ValorFixo = true;

        await CriarService().Add(dto, UserId);

        _repo.Verify(r => r.Create(It.Is<Conta>(c => c.ValorFixo)), Times.Once);
    }

    [Fact]
    public async Task Observacao_e_gravada()
    {
        var dto = Conta();
        dto.Observacao = "Débito no cartão da Ágatha";

        await CriarService().Add(dto, UserId);

        _repo.Verify(r => r.Create(It.Is<Conta>(c => c.Observacao == "Débito no cartão da Ágatha")), Times.Once);
    }
}
