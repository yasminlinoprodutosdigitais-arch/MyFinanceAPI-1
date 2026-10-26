using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #23: a natureza acompanha o tipo (Receita só "Rendimento / Entrada", Despesa nunca), a cor é uma
/// chave da paleta do front e o ícone é um PrimeIcons. A lista leva quantas contas usam cada categoria.
/// </summary>
public class CategoriaServiceTests
{
    private const int UserId = 7;

    private readonly Mock<ICategoriaRepository> _repo = new();
    private readonly Mock<IContaRepository> _contas = new();

    private CategoriaService CriarService() => new(_repo.Object, _contas.Object, MapperFactory.Create());

    private static CategoriaDTO Categoria(string tipo, int natureza, string? cor = null, string? icone = null) =>
        new() { Nome = "Casa", Subcategoria = tipo, NaturezaOperacao = natureza, Ativo = true, Cor = cor, Icone = icone };

    [Theory]
    [InlineData("Receita", 1)]
    [InlineData("Receita", 4)]
    [InlineData("Despesa", 0)]
    [InlineData(" despesa ", 0)]
    public async Task Tipo_e_natureza_incompativeis_sao_recusados(string tipo, int natureza)
    {
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AdicionarAsync(Categoria(tipo, natureza), UserId));

        Assert.Contains("natureza", erro.Message);
        _repo.Verify(r => r.CriarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task Tipo_e_natureza_incompativeis_tambem_sao_recusados_na_edicao()
    {
        var dto = Categoria("Despesa", 0);
        dto.Id = 16;

        await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().AtualizarAsync(dto, UserId));

        _repo.Verify(r => r.AtualizarAsync(It.IsAny<Categoria>(), It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData("Receita", 0)]
    [InlineData("Despesa", 1)]
    [InlineData("Despesa", 3)]
    [InlineData("Despesa", 5)]
    public async Task Tipo_e_natureza_compativeis_gravam(string tipo, int natureza)
    {
        await CriarService().AdicionarAsync(Categoria(tipo, natureza), UserId);

        _repo.Verify(r => r.CriarAsync(It.IsAny<Categoria>()), Times.Once);
    }

    [Fact]
    public async Task Cor_e_icone_validos_sao_gravados()
    {
        await CriarService().AdicionarAsync(Categoria("Despesa", 1, cor: "verde-agua", icone: "pi pi-shopping-cart"), UserId);

        _repo.Verify(r => r.CriarAsync(It.Is<Categoria>(c => c.Cor == "verde-agua" && c.Icone == "pi pi-shopping-cart")), Times.Once);
    }

    [Fact]
    public async Task Sem_cor_e_sem_icone_grava_nulo()
    {
        await CriarService().AdicionarAsync(Categoria("Despesa", 1, cor: "", icone: " "), UserId);

        _repo.Verify(r => r.CriarAsync(It.Is<Categoria>(c => c.Cor == null && c.Icone == null)), Times.Once);
    }

    [Theory]
    [InlineData("#ff0000", null)]
    [InlineData("magenta", null)]
    [InlineData(null, "fa fa-car")]
    [InlineData(null, "pi pi-car\" onclick=\"x")]
    public async Task Cor_fora_da_paleta_ou_icone_fora_do_padrao_sao_recusados(string? cor, string? icone)
    {
        await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AdicionarAsync(Categoria("Despesa", 1, cor, icone), UserId));

        _repo.Verify(r => r.CriarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task Listar_traz_quantas_contas_usam_cada_categoria()
    {
        _repo.Setup(r => r.ListarAsync(UserId)).ReturnsAsync(
        [
            new Categoria { Id = 6, Nome = "Casa", Subcategoria = "Despesa", NaturezaOperacao = 1 },
            new Categoria { Id = 26, Nome = "Tansferência entre contas", Subcategoria = "Despesa", NaturezaOperacao = 1 }
        ]);
        _repo.Setup(r => r.ContarContasPorCategoriaAsync(UserId)).ReturnsAsync(new Dictionary<int, int> { [6] = 5 });

        var lista = (await CriarService().ListarAsync(UserId)).ToList();

        Assert.Equal(5, lista.Single(c => c.Id == 6).QuantidadeContas);
        Assert.Equal(0, lista.Single(c => c.Id == 26).QuantidadeContas);
    }
}
