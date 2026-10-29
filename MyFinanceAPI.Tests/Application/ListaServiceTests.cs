using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #67: listas com campos configuráveis, resumo, pasta Arquivadas e duplicar.</summary>
public class ListaServiceTests
{
    private const int UserId = 7;

    // 28/10/2026 01:30 em UTC ainda é dia 27 em São Paulo.
    private static readonly DateTimeOffset Agora = new(2026, 10, 28, 1, 30, 0, TimeSpan.Zero);
    private static readonly DateOnly HojeSaoPaulo = new(2026, 10, 27);
    private static readonly Guid Serie = Guid.Parse("6f9c1a3e-0000-4000-8000-000000000012");

    private sealed class RelogioFalso(DateTimeOffset agora) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => agora;
    }

    private readonly Mock<IListaRepository> _listas = new();
    private readonly Mock<IItemListaRepository> _itens = new();
    private readonly UnitOfWorkFalso _uow = new();

    public ListaServiceTests()
    {
        _itens.Setup(r => r.ListarQueRepetemAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync([]);
        _itens.Setup(r => r.ListarOcorrenciasAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync([]);
    }

    [Fact]
    public async Task Proxima_data_do_resumo_considera_a_proxima_ocorrencia_pendente_de_quem_repete()
    {
        _listas.Setup(r => r.ListarComResumoAsync(UserId, false, HojeSaoPaulo)).ReturnsAsync(
        [
            new ListaComResumo { Lista = new Lista { Id = 11, Nome = "Agenda semanal" }, ProximaData = HojeSaoPaulo.AddDays(5) },
            new ListaComResumo { Lista = new Lista { Id = 12, Nome = "Mercado" }, ProximaData = HojeSaoPaulo.AddDays(1) },
        ]);
        _itens.Setup(r => r.ListarQueRepetemAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync(
        [
            new ItemLista { Id = 3, ListaId = 11, Descricao = "Academia", Data = HojeSaoPaulo.AddDays(-7), Repeticao = RepeticaoItem.Diaria },
            new ItemLista { Id = 4, ListaId = 12, Descricao = "Feira", Data = HojeSaoPaulo.AddDays(-7), Repeticao = RepeticaoItem.Semanal, DiasSemana = [7] },
        ]);
        _itens.Setup(r => r.ListarOcorrenciasAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync(
            [new OcorrenciaItemLista { ItemId = 3, Data = HojeSaoPaulo, Feito = true }]);

        var listas = (await CriarService().ListarAsync(UserId, arquivadas: false)).ToList();

        // Academia: hoje já feita, então amanhã (antes do dia +5 da consulta). Feira (domingo): depois do +1, fica o +1.
        Assert.Equal(HojeSaoPaulo.AddDays(1), listas[0].Resumo!.ProximaData);
        Assert.Equal(HojeSaoPaulo.AddDays(1), listas[1].Resumo!.ProximaData);
    }

    private ListaService CriarService() =>
        new(_listas.Object, _itens.Object, MapperFactory.Create(), _uow, new RelogioFalso(Agora));

    private static ListaDTO Dto(Action<ListaDTO>? ajuste = null)
    {
        var dto = new ListaDTO { Nome = "Mercado", UsaValor = true, ModoValor = ModoValorLista.Unitario, Modelo = ModeloLista.Compras };
        ajuste?.Invoke(dto);
        return dto;
    }

    [Fact]
    public async Task Listar_traz_o_resumo_e_usa_o_dia_de_Sao_Paulo()
    {
        _listas.Setup(r => r.ListarComResumoAsync(UserId, false, HojeSaoPaulo)).ReturnsAsync(
        [
            new ListaComResumo
            {
                Lista = new Lista { Id = 20, Nome = "Gastos do mês", UsaValor = true },
                Itens = 9, Feitos = 3, Total = 3597m, TotalFeito = 1200m, ProximaData = HojeSaoPaulo,
            },
        ]);

        var dto = Assert.Single(await CriarService().ListarAsync(UserId, arquivadas: false));

        Assert.Equal("Gastos do mês", dto.Nome);
        Assert.Equal(9, dto.Resumo!.Itens);
        Assert.Equal(3, dto.Resumo.Feitos);
        Assert.Equal(3597m, dto.Resumo.Total);
        Assert.Equal(1200m, dto.Resumo.TotalFeito);
        Assert.Equal(HojeSaoPaulo, dto.Resumo.ProximaData);
    }

    [Fact]
    public async Task Obter_traz_o_resumo_e_null_quando_nao_existe()
    {
        _listas.Setup(r => r.ObterComResumoAsync(20, UserId, HojeSaoPaulo))
            .ReturnsAsync(new ListaComResumo { Lista = new Lista { Id = 20, Nome = "X" }, Itens = 2 });

        Assert.Equal(2, (await CriarService().ObterPorIdAsync(20, UserId))!.Resumo!.Itens);
        Assert.Null(await CriarService().ObterPorIdAsync(99, UserId));
    }

    [Fact]
    public async Task Limite_ligado_sem_valor_e_recusado()
    {
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AdicionarAsync(Dto(d => { d.UsaValor = false; d.UsaLimite = true; d.Limite = 100m; }), UserId));

        Assert.Equal("O limite de gasto só pode ser ligado com o campo Valor ligado.", erro.Message);
    }

    [Fact]
    public async Task Limite_ligado_precisa_do_valor_do_limite()
    {
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AtualizarAsync(Dto(d => { d.Id = 3; d.UsaLimite = true; d.Limite = null; }), UserId));

        Assert.Equal("Informe o limite de gasto.", erro.Message);
    }

    [Fact]
    public async Task Limite_desligado_guarda_o_valor_do_limite()
    {
        _listas.Setup(r => r.AtualizarAsync(It.IsAny<Lista>(), UserId)).Returns(Task.CompletedTask);

        await CriarService().AtualizarAsync(Dto(d => { d.Id = 3; d.UsaLimite = false; d.Limite = 800m; }), UserId);

        _listas.Verify(r => r.AtualizarAsync(It.Is<Lista>(l => !l.UsaLimite && l.Limite == 800m), UserId));
    }

    [Theory]
    [InlineData("pi-cart")]
    [InlineData("fa fa-cart")]
    public async Task Icone_fora_do_PrimeIcons_e_recusado(string icone)
    {
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AdicionarAsync(Dto(d => d.Icone = icone), UserId));

        Assert.Equal($"Ícone inválido: {icone}.", erro.Message);
    }

    [Fact]
    public async Task Criar_nunca_nasce_arquivada()
    {
        _listas.Setup(r => r.CriarAsync(It.IsAny<Lista>())).ReturnsAsync((Lista l) => { l.Id = 30; return l; });

        var criada = await CriarService().AdicionarAsync(Dto(d => d.ArquivadaEm = DateTime.UtcNow), UserId);

        Assert.Equal(30, criada.Id);
        _listas.Verify(r => r.CriarAsync(It.Is<Lista>(l => l.ArquivadaEm == null && l.UsuarioId == UserId)));
    }

    [Fact]
    public async Task Arquivar_grava_a_hora_e_desarquivar_limpa()
    {
        await CriarService().ArquivarAsync(21, UserId);
        await CriarService().DesarquivarAsync(21, UserId);

        _listas.Verify(r => r.DefinirArquivadaAsync(21, UserId, Agora.UtcDateTime));
        _listas.Verify(r => r.DefinirArquivadaAsync(21, UserId, null));
    }

    [Fact]
    public async Task Duplicar_copia_a_configuracao_e_os_itens_desmarcados_na_mesma_operacao()
    {
        var original = new Lista
        {
            Id = 12, UsuarioId = UserId, Nome = "Lista de compras", Icone = "pi pi-shopping-cart", UsaQuantidade = true,
            UsaValor = true, ModoValor = ModoValorLista.Unitario, UsaLimite = true, Limite = 500m,
            Modelo = ModeloLista.Compras, ArquivadaEm = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), SerieId = Serie,
        };
        _listas.Setup(r => r.ObterPorIdAsync(12, UserId)).ReturnsAsync(original);
        _listas.Setup(r => r.CriarAsync(It.IsAny<Lista>()))
            .Callback(() => _uow.Registrar("Lista.Criar"))
            .ReturnsAsync((Lista l) => { l.Id = 40; return l; });
        _itens.Setup(r => r.ListarDaListaAsync(12, UserId)).ReturnsAsync(
        [
            new ItemLista { Id = 1, ListaId = 12, UsuarioId = UserId, Descricao = "Arroz", Feito = true, FeitoEm = DateTime.UtcNow, Quantidade = 2, Valor = 6.5m, Ordem = 1 },
            new ItemLista { Id = 2, ListaId = 12, UsuarioId = UserId, Descricao = "Feijão", Observacao = "carioca" },
        ]);
        List<ItemLista>? copiados = null;
        _itens.Setup(r => r.CriarVariosAsync(It.IsAny<IEnumerable<ItemLista>>()))
            .Callback<IEnumerable<ItemLista>>(i => { _uow.Registrar("Itens.Criar"); copiados = i.ToList(); })
            .Returns(Task.CompletedTask);

        var copia = await CriarService().DuplicarAsync(12, new DuplicarListaDTO(), UserId);

        Assert.Equal(40, copia.Id);
        Assert.Equal("Lista de compras (cópia)", copia.Nome);
        Assert.Null(copia.ArquivadaEm);
        Assert.Equal(Serie, copia.SerieId);
        Assert.Equal(12, copia.ListaOrigemId);
        Assert.Equal(new int?[] { 1, 2 }, copiados!.Select(i => i.ItemOrigemId));
        Assert.True(copia.UsaQuantidade && copia.UsaValor && copia.UsaLimite);
        Assert.Equal(500m, copia.Limite);
        Assert.Equal(ModoValorLista.Unitario, copia.ModoValor);
        Assert.Equal("pi pi-shopping-cart", copia.Icone);
        Assert.Equal(new[] { "Arroz", "Feijão" }, copiados!.Select(i => i.Descricao));
        Assert.All(copiados!, i =>
        {
            Assert.Equal(0, i.Id);
            Assert.Equal(40, i.ListaId);
            Assert.Equal(UserId, i.UsuarioId);
            Assert.False(i.Feito);
            Assert.Null(i.FeitoEm);
        });
        Assert.Equal(6.5m, copiados![0].Valor);
        Assert.Equal("carioca", copiados[1].Observacao);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Duplicar_usa_o_nome_informado()
    {
        _listas.Setup(r => r.ObterPorIdAsync(12, UserId)).ReturnsAsync(new Lista { Id = 12, Nome = "Gastos de outubro" });
        _listas.Setup(r => r.CriarAsync(It.IsAny<Lista>())).ReturnsAsync((Lista l) => l);
        _itens.Setup(r => r.ListarDaListaAsync(12, UserId)).ReturnsAsync([]);

        var copia = await CriarService().DuplicarAsync(12, new DuplicarListaDTO { Nome = "  Gastos de novembro " }, UserId);

        Assert.Equal("Gastos de novembro", copia.Nome);
    }

    [Fact]
    public async Task Duplicar_sem_valores_sem_itens_e_arquivando_a_original()
    {
        _listas.Setup(r => r.ObterPorIdAsync(12, UserId)).ReturnsAsync(new Lista { Id = 12, Nome = "Mercado", SerieId = Serie });
        _listas.Setup(r => r.CriarAsync(It.IsAny<Lista>())).ReturnsAsync((Lista l) => { l.Id = 41; return l; });
        _listas.Setup(r => r.DefinirArquivadaAsync(12, UserId, It.IsAny<DateTime?>()))
            .Callback(() => _uow.Registrar("Original.Arquivar")).Returns(Task.CompletedTask);
        _itens.Setup(r => r.ListarDaListaAsync(12, UserId)).ReturnsAsync(
            [new ItemLista { Id = 1, ListaId = 12, Descricao = "Arroz", Quantidade = 2, Valor = 6.5m }]);
        List<ItemLista>? copiados = null;
        _itens.Setup(r => r.CriarVariosAsync(It.IsAny<IEnumerable<ItemLista>>()))
            .Callback<IEnumerable<ItemLista>>(i => copiados = i.ToList()).Returns(Task.CompletedTask);

        await CriarService().DuplicarAsync(12, new DuplicarListaDTO { ManterValores = false, ArquivarOriginal = true }, UserId);

        var item = Assert.Single(copiados!);
        Assert.Null(item.Valor);
        Assert.Equal(2, item.Quantidade);
        _listas.Verify(r => r.DefinirArquivadaAsync(12, UserId, Agora.UtcDateTime));
        Assert.Empty(_uow.Fora);

        copiados = null;
        await CriarService().DuplicarAsync(12, new DuplicarListaDTO { CopiarItens = false }, UserId);
        Assert.Null(copiados);
        _itens.Verify(r => r.CriarVariosAsync(It.IsAny<IEnumerable<ItemLista>>()), Times.Once);
    }

    [Fact]
    public async Task Duplicar_lista_de_outro_usuario_da_nao_encontrada()
    {
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().DuplicarAsync(99, new DuplicarListaDTO(), UserId));
    }

    [Fact]
    public async Task Serie_traz_as_listas_da_serie_com_resumo()
    {
        _listas.Setup(r => r.ObterPorIdAsync(12, UserId)).ReturnsAsync(new Lista { Id = 12, Nome = "Mercado", SerieId = Serie });
        _listas.Setup(r => r.ListarDaSerieAsync(Serie, UserId, HojeSaoPaulo)).ReturnsAsync(
        [
            new ListaComResumo { Lista = new Lista { Id = 5, Nome = "Mercado out", SerieId = Serie }, Total = 100m },
            new ListaComResumo { Lista = new Lista { Id = 12, Nome = "Mercado", SerieId = Serie }, Total = 120m },
        ]);

        var serie = (await CriarService().ListarDaSerieAsync(12, UserId)).ToList();

        Assert.Equal(new[] { 5, 12 }, serie.Select(l => l.Id));
        Assert.Equal(100m, serie[0].Resumo!.Total);
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().ListarDaSerieAsync(99, UserId));
    }

    [Fact]
    public async Task Ligar_a_serie_de_outra_lista_e_sair_para_uma_serie_propria()
    {
        var outra = Guid.NewGuid();
        _listas.Setup(r => r.ObterPorIdAsync(12, UserId)).ReturnsAsync(new Lista { Id = 12, Nome = "Mercado", SerieId = Serie });
        _listas.Setup(r => r.ObterPorIdAsync(5, UserId)).ReturnsAsync(new Lista { Id = 5, Nome = "Feira", SerieId = outra });

        await CriarService().DefinirSerieAsync(12, comListaId: 5, UserId);
        await CriarService().DefinirSerieAsync(12, comListaId: null, UserId);

        _listas.Verify(r => r.DefinirSerieAsync(12, UserId, outra));
        _listas.Verify(r => r.DefinirSerieAsync(12, UserId, It.Is<Guid>(g => g != Guid.Empty && g != Serie && g != outra)));
    }

    [Fact]
    public async Task Ligar_a_lista_alheia_ou_inexistente_da_nao_encontrada()
    {
        _listas.Setup(r => r.ObterPorIdAsync(12, UserId)).ReturnsAsync(new Lista { Id = 12, Nome = "Mercado", SerieId = Serie });

        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().DefinirSerieAsync(12, comListaId: 99, UserId));
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().DefinirSerieAsync(98, comListaId: null, UserId));
        _listas.Verify(r => r.DefinirSerieAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid>()), Times.Never);
    }

    private void SerieDeTres()
    {
        var listas = new[]
        {
            new Lista { Id = 5, Nome = "Compras set", SerieId = Serie, ModoValor = ModoValorLista.Unitario },
            new Lista { Id = 9, Nome = "Compras out", SerieId = Serie, ModoValor = ModoValorLista.Unitario },
            new Lista { Id = 12, Nome = "Compras nov", SerieId = Serie, ModoValor = ModoValorLista.Unitario },
        };
        foreach (var l in listas)
            _listas.Setup(r => r.ObterPorIdAsync(l.Id, UserId)).ReturnsAsync(l);
        _listas.Setup(r => r.ListarDaSerieAsync(Serie, UserId, HojeSaoPaulo))
            .ReturnsAsync(listas.Select(l => new ListaComResumo { Lista = l }).ToList());
        _itens.Setup(r => r.ListarDasListasAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync(
        [
            new ItemLista { Id = 50, ListaId = 5, Descricao = "Arroz", Quantidade = 1, Valor = 4m },
            new ItemLista { Id = 90, ListaId = 9, Descricao = "Arroz", Quantidade = 1, Valor = 5m, ItemOrigemId = 50 },
            new ItemLista { Id = 120, ListaId = 12, Descricao = "Arroz", Quantidade = 2, Valor = 6m, ItemOrigemId = 90, Feito = true },
        ]);
    }

    [Fact]
    public async Task Comparar_sem_outra_lista_usa_a_anterior_da_serie()
    {
        SerieDeTres();

        var c = await CriarService().CompararAsync(12, com: null, soFeitos: false, UserId);

        Assert.Equal(9, c.Anterior.Id);
        Assert.Equal("Compras nov", c.Atual.Nome);
        Assert.Equal(5m, c.Anterior.Total);
        Assert.Equal(12m, c.Atual.Total);
        Assert.Equal(1m, c.Efeitos.Preco);
        Assert.Equal(6m, c.Efeitos.Quantidade);
    }

    [Fact]
    public async Task Comparar_com_lista_escolhida_da_serie_e_so_feitos()
    {
        SerieDeTres();

        var c = await CriarService().CompararAsync(12, com: 5, soFeitos: true, UserId);

        Assert.Equal(5, c.Anterior.Id);
        Assert.Equal(0m, c.Anterior.Total);
        Assert.Equal(12m, c.Atual.Total);
        Assert.Equal(12m, c.Efeitos.Novos);
    }

    [Fact]
    public async Task Comparar_sem_anterior_ou_com_lista_de_outra_serie_e_recusado()
    {
        SerieDeTres();
        _listas.Setup(r => r.ObterPorIdAsync(30, UserId)).ReturnsAsync(new Lista { Id = 30, Nome = "Feira", SerieId = Guid.NewGuid() });

        var semAnterior = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().CompararAsync(5, null, false, UserId));
        var outraSerie = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().CompararAsync(12, 30, false, UserId));
        var mesma = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().CompararAsync(12, 12, false, UserId));

        Assert.Equal("Não há lista anterior na série.", semAnterior.Message);
        Assert.Equal("As listas não são da mesma série.", outraSerie.Message);
        Assert.Equal("Escolha outra lista para comparar.", mesma.Message);
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().CompararAsync(12, 99, false, UserId));
    }
}
