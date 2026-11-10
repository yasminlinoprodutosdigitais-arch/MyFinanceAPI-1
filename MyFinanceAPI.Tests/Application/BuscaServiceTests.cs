using Moq;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Busca rápida do menu (card #72).</summary>
public class BuscaServiceTests
{
    private readonly Mock<IBuscaRepository> _repo = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" a ")]
    public async Task Menos_de_duas_letras_nao_busca(string? texto)
    {
        Assert.Empty(await new BuscaService(_repo.Object).BuscarAsync(1, texto));
        _repo.Verify(r => r.BuscarAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Busca_em_maiusculas_sem_espacos_nas_pontas_e_ate_5_de_cada()
    {
        _repo.Setup(r => r.BuscarAsync(7, "MERC", 5)).ReturnsAsync([
            new ItemBusca(TiposBusca.Item, 3, "Arroz", "Mercado do mês", 9),
            new ItemBusca(TiposBusca.Categoria, 4, "Mercado", " ", null)
        ]);

        var r = await new BuscaService(_repo.Object).BuscarAsync(7, "  merc ");

        Assert.Equal([("item", 3, 9), ("categoria", 4, (int?)null)], r.Select(x => (x.Tipo, x.Id, x.ListaId)));
        Assert.Null(r[1].Detalhe);
    }

    [Fact]
    public void Filtros_so_trazem_o_que_e_do_usuario_e_contem_o_termo()
    {
        var listas = new[]
        {
            new Lista { Id = 1, UsuarioId = 7, Nome = "Mercado do mês" },
            new Lista { Id = 2, UsuarioId = 8, Nome = "Mercado do outro" },
            new Lista { Id = 3, UsuarioId = 7, Nome = "Viagem" }
        }.AsQueryable();
        var contas = new[]
        {
            new Conta { Id = 5, UsuarioId = 7, Nome = "Internet" },
            new Conta { Id = 6, UsuarioId = 8, Nome = "Internet" }
        }.AsQueryable();

        Assert.Equal([1], listas.Where(BuscaFiltros.Planejamento(7, "MERC")).Select(l => l.Id));
        Assert.Equal([5], contas.Where(BuscaFiltros.ContaFixa(7, "NET")).Select(c => c.Id));
    }
}
