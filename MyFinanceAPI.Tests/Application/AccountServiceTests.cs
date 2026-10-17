using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Conta e vencimentos são gravados em passos separados; precisam valer juntos (BE-P1-05).
/// </summary>
public class AccountServiceTests
{
    private const int UserId = 7;
    private const int AccountId = 100;

    private readonly Mock<IAccountRepository> _repo = new();
    private readonly UnitOfWorkFalso _uow = new();
    private List<ContaVencimento>? _vencimentosCriados;

    public AccountServiceTests()
    {
        _repo.Setup(r => r.Create(It.IsAny<Account>()))
            .Callback(() => _uow.Registrar("Account.Create"))
            .ReturnsAsync((Account a) => { a.Id = AccountId; return a; });
        _repo.Setup(r => r.Update(It.IsAny<Account>(), UserId))
            .Callback(() => _uow.Registrar("Account.Update"))
            .ReturnsAsync((Account a, int _) => a);
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

    private AccountService CriarService() => new(_repo.Object, MapperFactory.Create(), _uow);

    private static AccountDTO Conta(int id = 0) => new()
    {
        Id = id,
        Name = "Internet",
        Value = 120m,
        CategoryId = 4,
        Status = 1,
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

        _repo.Verify(r => r.Update(It.IsAny<Account>(), UserId), Times.Once);
        _repo.Verify(r => r.RemoveContaVencimento(AccountId, UserId), Times.Once);
        Assert.Equal(new[] { 5, 20 }, _vencimentosCriados!.Select(v => v.Dia));
        Assert.Empty(_uow.Fora);
    }
}
