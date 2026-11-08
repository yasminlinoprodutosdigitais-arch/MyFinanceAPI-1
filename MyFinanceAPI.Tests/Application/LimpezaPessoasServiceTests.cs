using Moq;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #76, fase 2 (07/11/2026): limpar as pessoas que a importação antiga criou para estabelecimentos. Candidata =
/// fora dos acertos, sem categoria e tipo padrão, sem regra e não o "Eu". Os lançamentos ficam com o nome.
/// </summary>
public class LimpezaPessoasServiceTests
{
    private const int UserId = 7;

    private readonly Mock<IPessoaMovimentacaoRepository> _pessoas = new();
    private readonly Mock<IRegraClassificacaoRepository> _regras = new();
    private readonly Mock<IExtratoBancarioItemRepository> _itens = new();
    private readonly UnitOfWorkFalso _uow = new();
    private readonly List<PessoaMovimentacao> _cadastradas =
    [
        new() { Id = 1, NomePessoa = "SHOPEE" },
        new() { Id = 2, NomePessoa = "EU", EhTitular = true },
        new() { Id = 3, NomePessoa = "CARLOS", EntraNosAcertos = true },
        new() { Id = 4, NomePessoa = "HAVAN", CategoriaId = 9 },
        new() { Id = 5, NomePessoa = "POSTO", TipoMovimentacaoId = 2 },
        new() { Id = 6, NomePessoa = "GIOVANNE" },                   // usada por regra
        new() { Id = 7, NomePessoa = "AMAZON BR" }
    ];
    private IReadOnlyCollection<int>? _desligadas;
    private IReadOnlyCollection<int>? _removidas;

    public LimpezaPessoasServiceTests()
    {
        _pessoas.Setup(r => r.ListarAsync(UserId)).ReturnsAsync(() => _cadastradas);
        _regras.Setup(r => r.ListarAsync(UserId)).ReturnsAsync([new RegraClassificacao { Id = 1, Texto = "PIX GIO", PessoaMovimentacaoId = 6 }]);
        _itens.Setup(r => r.ContarPorPessoaAsync(UserId)).ReturnsAsync(new Dictionary<int, int> { [1] = 12, [3] = 4 });
        _itens.Setup(r => r.DesligarPessoasAsync(UserId, It.IsAny<IReadOnlyCollection<int>>()))
            .Callback<int, IReadOnlyCollection<int>>((_, ids) => { _uow.Registrar("Desligar"); _desligadas = ids; })
            .Returns(Task.CompletedTask);
        _pessoas.Setup(r => r.RemoverVariasAsync(UserId, It.IsAny<IReadOnlyCollection<int>>()))
            .Callback<int, IReadOnlyCollection<int>>((_, ids) => { _uow.Registrar("Remover"); _removidas = ids; })
            .ReturnsAsync((int _, IReadOnlyCollection<int> ids) => ids.Count);
    }

    private LimpezaPessoasService Servico() => new(_pessoas.Object, _regras.Object, _itens.Object, _uow);

    [Fact]
    public async Task Candidatas_sao_so_as_que_parecem_estabelecimento_em_ordem_de_nome_com_os_lancamentos()
    {
        var candidatas = await Servico().CandidatasAsync(UserId);

        Assert.Equal([(7, "AMAZON BR", 0), (1, "SHOPEE", 12)], candidatas.Select(c => (c.Id, c.NomePessoa, c.Lancamentos)));
    }

    [Fact]
    public async Task Remover_desliga_e_apaga_so_as_pedidas_que_ainda_sao_candidatas_na_mesma_operacao()
    {
        // 3 (acertos), 4 (com padrão) e 99 (não existe) são ignoradas.
        var quantidade = await Servico().RemoverAsync(UserId, [1, 3, 4, 7, 99]);

        Assert.Equal(2, quantidade);
        Assert.Equal([1, 7], _desligadas!.Order());
        Assert.Equal([1, 7], _removidas!.Order());
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Remover_nada_valido_nao_grava()
    {
        Assert.Equal(0, await Servico().RemoverAsync(UserId, [2, 3]));
        Assert.Equal(0, await Servico().RemoverAsync(UserId, []));

        _itens.Verify(r => r.DesligarPessoasAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
        _pessoas.Verify(r => r.RemoverVariasAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
    }
}
