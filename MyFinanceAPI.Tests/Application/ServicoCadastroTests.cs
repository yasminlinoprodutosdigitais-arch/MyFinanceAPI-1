using AutoMapper;
using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Base dos serviços de cadastro (BE-P2-02): mapeia, grava o dono e roda as validações
/// do cadastro antes de gravar ou excluir.
/// </summary>
public class ServicoCadastroTests
{
    private const int UserId = 7;

    private readonly Mock<IListaRepository> _repo = new();

    /// <summary>Serviço mínimo: registra as validações e recusa o que for marcado.</summary>
    private sealed class ServicoTeste(IListaRepository repo, IMapper mapper) : ServicoCadastro<Lista, ListaDTO>(repo, mapper)
    {
        public List<string> Validacoes { get; } = [];
        public bool RecusarGravacao { get; set; }
        public bool RecusarExclusao { get; set; }

        protected override Task ValidarGravacaoAsync(ListaDTO dto, int userId, int ignorarId)
        {
            Validacoes.Add($"gravar {dto.Nome} ignorando {ignorarId}");
            return RecusarGravacao ? throw new RegraNegocioException("recusado") : Task.CompletedTask;
        }

        protected override Task ValidarExclusaoAsync(int id, int userId)
        {
            Validacoes.Add($"excluir {id}");
            return RecusarExclusao ? throw new RegraNegocioException("em uso") : Task.CompletedTask;
        }
    }

    private ServicoTeste CriarService() => new(_repo.Object, MapperFactory.Create());

    [Fact]
    public async Task Adicionar_grava_o_dono_e_valida_sem_ignorar_ninguem()
    {
        var service = CriarService();

        await service.AdicionarAsync(new ListaDTO { Nome = "Mercado" }, UserId);

        Assert.Equal(["gravar Mercado ignorando 0"], service.Validacoes);
        _repo.Verify(r => r.CriarAsync(It.Is<Lista>(l => l.Nome == "Mercado" && l.UsuarioId == UserId)), Times.Once);
    }

    [Fact]
    public async Task Adicionar_devolve_o_criado_com_o_id_gerado()
    {
        // O banco gera o id na gravação; o 201 do controller leva esse id no Location (BE-P3-05).
        _repo.Setup(r => r.CriarAsync(It.IsAny<Lista>())).ReturnsAsync((Lista l) => { l.Id = 42; return l; });

        var criada = await CriarService().AdicionarAsync(new ListaDTO { Nome = "Mercado" }, UserId);

        Assert.Equal(42, criada.Id);
        Assert.Equal("Mercado", criada.Nome);
    }

    [Fact]
    public async Task Adicionar_recusado_nao_grava()
    {
        var service = CriarService();
        service.RecusarGravacao = true;

        await Assert.ThrowsAsync<RegraNegocioException>(() => service.AdicionarAsync(new ListaDTO { Nome = "X" }, UserId));

        _repo.Verify(r => r.CriarAsync(It.IsAny<Lista>()), Times.Never);
    }

    [Fact]
    public async Task Atualizar_valida_ignorando_o_proprio_registro()
    {
        var service = CriarService();

        await service.AtualizarAsync(new ListaDTO { Id = 5, Nome = "Feira" }, UserId);

        Assert.Equal(["gravar Feira ignorando 5"], service.Validacoes);
        _repo.Verify(r => r.AtualizarAsync(It.Is<Lista>(l => l.Id == 5 && l.Nome == "Feira"), UserId), Times.Once);
    }

    [Fact]
    public async Task Atualizar_recusado_nao_altera()
    {
        var service = CriarService();
        service.RecusarGravacao = true;

        await Assert.ThrowsAsync<RegraNegocioException>(() => service.AtualizarAsync(new ListaDTO { Id = 5, Nome = "X" }, UserId));

        _repo.Verify(r => r.AtualizarAsync(It.IsAny<Lista>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Remover_valida_antes_e_recusado_nao_apaga()
    {
        var service = CriarService();
        service.RecusarExclusao = true;

        await Assert.ThrowsAsync<RegraNegocioException>(() => service.RemoverAsync(5, UserId));

        Assert.Equal(["excluir 5"], service.Validacoes);
        _repo.Verify(r => r.RemoverAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Remover_permitido_apaga()
    {
        await CriarService().RemoverAsync(5, UserId);

        _repo.Verify(r => r.RemoverAsync(5, UserId), Times.Once);
    }

    [Fact]
    public async Task ObterPorId_inexistente_e_null()
    {
        _repo.Setup(r => r.ObterPorIdAsync(9, UserId)).ReturnsAsync((Lista?)null);

        Assert.Null(await CriarService().ObterPorIdAsync(9, UserId));
    }

    [Fact]
    public async Task Listar_mapeia_o_que_o_repositorio_devolve()
    {
        _repo.Setup(r => r.ListarAsync(UserId)).ReturnsAsync([new Lista { Id = 1, Nome = "Mercado" }, new Lista { Id = 2, Nome = "Feira" }]);

        var lista = await CriarService().ListarAsync(UserId);

        Assert.Equal(["Mercado", "Feira"], lista.Select(l => l.Nome));
    }
}
