using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #45, parte 2 (04/11/2026): a pessoa "Eu" (titular, uma por usuário, criada sob demanda, sem renomear
/// nem excluir) e o "Entra nos acertos" (só pessoas marcadas entram nos Acertos; desligado por padrão).
/// </summary>
public class PessoaMovimentacaoServiceTests
{
    private const int UserId = 7;

    private readonly Mock<IPessoaMovimentacaoRepository> _repo = new();
    private readonly Mock<IExtratoBancarioItemService> _itens = new();
    private readonly PessoaMovimentacao _eu = new() { Id = 1, NomePessoa = "EU", EhTitular = true, UsuarioId = UserId };

    public PessoaMovimentacaoServiceTests()
    {
        _repo.Setup(r => r.ObterTitularAsync(UserId)).ReturnsAsync(_eu);
        _repo.Setup(r => r.ObterPorIdAsync(_eu.Id, UserId)).ReturnsAsync(_eu);
        _repo.Setup(r => r.ListarAsync(UserId)).ReturnsAsync(() => [_eu, new() { Id = 2, NomePessoa = "CARLOS", UsuarioId = UserId }]);
        _repo.Setup(r => r.CriarAsync(It.IsAny<PessoaMovimentacao>())).ReturnsAsync((PessoaMovimentacao p) => p);
    }

    private PessoaMovimentacaoService Servico() => new(_repo.Object, _itens.Object, MapperFactory.Create());

    [Fact]
    public async Task Listar_garante_o_Eu_antes_de_listar()
    {
        var pessoas = (await Servico().ListarAsync(UserId)).ToList();

        _repo.Verify(r => r.ObterTitularAsync(UserId), Times.Once);
        Assert.True(pessoas.Single(p => p.Id == _eu.Id).EhTitular);
        Assert.False(pessoas.Single(p => p.Id == 2).EhTitular);
    }

    [Fact]
    public async Task Nao_renomeia_o_Eu()
    {
        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            Servico().AtualizarAsync(new PessoaMovimentacaoDTO { Id = _eu.Id, NomePessoa = "Yasmin" }, UserId));

        Assert.Contains("Eu", ex.Message);
        _repo.Verify(r => r.AtualizarAsync(It.IsAny<PessoaMovimentacao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Eu_aceita_mudar_categoria_e_tipo_com_o_mesmo_nome()
    {
        await Servico().AtualizarAsync(new PessoaMovimentacaoDTO { Id = _eu.Id, NomePessoa = " eu ", CategoriaId = 4 }, UserId);

        _repo.Verify(r => r.AtualizarAsync(It.Is<PessoaMovimentacao>(p => p.CategoriaId == 4), UserId), Times.Once);
    }

    [Fact]
    public async Task Nao_exclui_o_Eu()
    {
        await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().RemoverAsync(_eu.Id, UserId));

        _repo.Verify(r => r.RemoverAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Pessoa_nova_nunca_nasce_titular_e_guarda_o_entra_nos_acertos()
    {
        var criada = await Servico().AdicionarAsync(
            new PessoaMovimentacaoDTO { NomePessoa = "Ágatha", EhTitular = true, EntraNosAcertos = true }, UserId);

        Assert.False(criada.EhTitular);
        Assert.True(criada.EntraNosAcertos);
    }

    // ---------- modelo ----------

    private static IEntityType Pessoas()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        return contexto.GetService<IDesignTimeModel>().Model.GetEntityTypes().Single(e => e.GetTableName() == "pessoas_movimentacao");
    }

    [Theory]
    [InlineData("eh_titular")]
    [InlineData("entra_nos_acertos")]
    public void Tabela_tem_as_colunas_novas_com_padrao_falso(string coluna)
    {
        var propriedade = Pessoas().GetProperties().Single(p => p.GetColumnName() == coluna);

        Assert.False(propriedade.IsNullable);
        Assert.Equal(false, propriedade.GetDefaultValue());
    }

    [Fact]
    public void Uma_titular_por_usuario()
    {
        var indice = Pessoas().GetIndexes().Single(i => i.GetDatabaseName() == "ux_pessoas_movimentacao_titular");

        Assert.True(indice.IsUnique);
        Assert.Equal(["UsuarioId"], indice.Properties.Select(p => p.Name));
        Assert.Equal("eh_titular", indice.GetFilter());
    }
}
