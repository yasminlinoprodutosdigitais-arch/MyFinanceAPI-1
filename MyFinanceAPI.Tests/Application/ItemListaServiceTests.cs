using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #67: itens sempre pela lista (/listas/{id}/itens), feito_em gravado pelo serviço e ações em lote.
/// </summary>
public class ItemListaServiceTests
{
    private const int UserId = 7;
    private const int ListaId = 12;

    private static readonly DateTimeOffset Agora = new(2026, 10, 28, 15, 0, 0, TimeSpan.Zero);
    private static readonly DateTime Ontem = new(2026, 10, 27, 10, 0, 0, DateTimeKind.Utc);

    private sealed class RelogioFalso(DateTimeOffset agora) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => agora;
    }

    private readonly Mock<IItemListaRepository> _itens = new();
    private readonly Mock<IListaRepository> _listas = new();
    private readonly UnitOfWorkFalso _uow = new();

    public ItemListaServiceTests()
    {
        _listas.Setup(r => r.ObterPorIdAsync(ListaId, UserId)).ReturnsAsync(new Lista { Id = ListaId, UsuarioId = UserId, Nome = "Mercado" });
        _itens.Setup(r => r.CriarAsync(It.IsAny<ItemLista>())).ReturnsAsync((ItemLista i) => { i.Id = 100; return i; });
    }

    private ItemListaService CriarService() =>
        new(_itens.Object, _listas.Object, MapperFactory.Create(), _uow, new RelogioFalso(Agora));

    private void ItemExistente(int id, int listaId = ListaId, bool feito = false, DateTime? feitoEm = null) =>
        _itens.Setup(r => r.ObterPorIdAsync(id, UserId)).ReturnsAsync(new ItemLista
        {
            Id = id, ListaId = listaId, UsuarioId = UserId, Descricao = "Arroz", Feito = feito, FeitoEm = feitoEm,
        });

    [Fact]
    public async Task Listar_de_lista_de_outro_usuario_da_nao_encontrada()
    {
        var erro = await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().ListarDaListaAsync(99, UserId));

        Assert.Equal("Lista não encontrada.", erro.Message);
    }

    [Fact]
    public async Task Adicionar_usa_a_lista_da_rota_e_grava_feito_em_se_ja_nasce_feito()
    {
        var criado = await CriarService().AdicionarNaListaAsync(ListaId,
            new ItemListaDTO { ListaId = 999, Descricao = "Café", Feito = true, FeitoEm = Ontem }, UserId);

        Assert.Equal(100, criado.Id);
        _itens.Verify(r => r.CriarAsync(It.Is<ItemLista>(i =>
            i.ListaId == ListaId && i.UsuarioId == UserId && i.Feito && i.FeitoEm == Agora.UtcDateTime)));
    }

    [Fact]
    public async Task Adicionar_pendente_nao_tem_feito_em()
    {
        await CriarService().AdicionarNaListaAsync(ListaId, new ItemListaDTO { Descricao = "Café", FeitoEm = Ontem }, UserId);

        _itens.Verify(r => r.CriarAsync(It.Is<ItemLista>(i => !i.Feito && i.FeitoEm == null)));
    }

    [Fact]
    public async Task Adicionar_em_lista_alheia_da_nao_encontrada()
    {
        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            CriarService().AdicionarNaListaAsync(99, new ItemListaDTO { Descricao = "Café" }, UserId));
        _itens.Verify(r => r.CriarAsync(It.IsAny<ItemLista>()), Times.Never);
    }

    [Fact]
    public async Task Marcar_feito_pelo_PUT_grava_agora()
    {
        ItemExistente(5);

        await CriarService().AtualizarNaListaAsync(ListaId, 5, new ItemListaDTO { Descricao = "Arroz", Feito = true }, UserId);

        _itens.Verify(r => r.AtualizarAsync(It.Is<ItemLista>(i => i.Id == 5 && i.ListaId == ListaId && i.FeitoEm == Agora.UtcDateTime), UserId));
    }

    [Fact]
    public async Task Editar_item_ja_feito_mantem_o_feito_em_original()
    {
        ItemExistente(5, feito: true, feitoEm: Ontem);

        await CriarService().AtualizarNaListaAsync(ListaId, 5, new ItemListaDTO { Descricao = "Arroz integral", Feito = true }, UserId);

        _itens.Verify(r => r.AtualizarAsync(It.Is<ItemLista>(i => i.FeitoEm == Ontem), UserId));
    }

    [Fact]
    public async Task Desmarcar_pelo_PUT_limpa_o_feito_em()
    {
        ItemExistente(5, feito: true, feitoEm: Ontem);

        await CriarService().AtualizarNaListaAsync(ListaId, 5, new ItemListaDTO { Descricao = "Arroz", Feito = false }, UserId);

        _itens.Verify(r => r.AtualizarAsync(It.Is<ItemLista>(i => !i.Feito && i.FeitoEm == null), UserId));
    }

    [Fact]
    public async Task Item_de_outra_lista_nao_e_alterado_nem_excluido_por_esta_rota()
    {
        ItemExistente(5, listaId: 13);

        var erro = await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            CriarService().AtualizarNaListaAsync(ListaId, 5, new ItemListaDTO { Descricao = "Arroz" }, UserId));
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().RemoverDaListaAsync(ListaId, 5, UserId));

        Assert.Equal("Item da lista não encontrado.", erro.Message);
        _itens.Verify(r => r.AtualizarAsync(It.IsAny<ItemLista>(), It.IsAny<int>()), Times.Never);
        _itens.Verify(r => r.RemoverAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Remover_item_da_lista()
    {
        ItemExistente(5);

        await CriarService().RemoverDaListaAsync(ListaId, 5, UserId);

        _itens.Verify(r => r.RemoverAsync(5, UserId));
    }

    [Fact]
    public async Task Lote_marca_os_ids_sem_repetir_e_devolve_a_quantidade()
    {
        _itens.Setup(r => r.MarcarFeitoAsync(ListaId, It.IsAny<IReadOnlyCollection<int>>(), true, Agora.UtcDateTime, UserId)).ReturnsAsync(2);

        var afetados = await CriarService().MarcarFeitoAsync(ListaId, [3, 4, 3], feito: true, UserId);

        Assert.Equal(2, afetados);
        _itens.Verify(r => r.MarcarFeitoAsync(ListaId, It.Is<IReadOnlyCollection<int>>(ids => ids.OrderBy(x => x).SequenceEqual(new[] { 3, 4 })),
            true, Agora.UtcDateTime, UserId));
    }

    [Fact]
    public async Task Lote_sem_ids_e_recusado()
    {
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().MarcarFeitoAsync(ListaId, [], true, UserId));

        Assert.Equal("Informe os itens.", erro.Message);
    }

    [Fact]
    public async Task Desmarcar_todos_e_excluir_concluidos_conferem_a_lista()
    {
        _itens.Setup(r => r.DesmarcarTodosAsync(ListaId, Agora.UtcDateTime, UserId)).ReturnsAsync(47);
        _itens.Setup(r => r.ExcluirConcluidosAsync(ListaId, UserId)).ReturnsAsync(3);

        Assert.Equal(47, await CriarService().DesmarcarTodosAsync(ListaId, UserId));
        Assert.Equal(3, await CriarService().ExcluirConcluidosAsync(ListaId, UserId));
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().DesmarcarTodosAsync(99, UserId));
        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().ExcluirConcluidosAsync(99, UserId));
    }

    // ---- Agenda (card #67, parte 3) ----

    private static readonly DateOnly Seg = new(2026, 10, 26);

    private void ItemSemanal(int id = 5, DateOnly? ate = null) =>
        _itens.Setup(r => r.ObterPorIdAsync(id, UserId)).ReturnsAsync(new ItemLista
        {
            Id = id, ListaId = ListaId, UsuarioId = UserId, Descricao = "Academia", Data = Seg, Hora = new TimeOnly(7, 0),
            Repeticao = RepeticaoItem.Semanal, DiasSemana = [1, 3, 5], RepetirAte = ate,
        });

    [Theory]
    [InlineData(RepeticaoItem.Diaria, null, null, "Informe a data da primeira ocorrência.")]
    [InlineData(RepeticaoItem.Semanal, "2026-10-26", null, "Escolha os dias da semana.")]
    [InlineData(RepeticaoItem.Semanal, "2026-10-26", new[] { 0 }, "Os dias da semana vão de 1 (segunda) a 7 (domingo).")]
    public async Task Regra_de_repeticao_invalida_e_recusada(string repeticao, string? data, int[]? dias, string mensagem)
    {
        var dto = new ItemListaDTO
        {
            Descricao = "Academia", Repeticao = repeticao, DiasSemana = dias,
            Data = data is null ? null : DateOnly.Parse(data),
        };

        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().AdicionarNaListaAsync(ListaId, dto, UserId));

        Assert.Equal(mensagem, erro.Message);
    }

    [Fact]
    public async Task Repetir_ate_antes_da_data_e_recusado_e_dias_so_ficam_na_semanal()
    {
        var erro = await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().AdicionarNaListaAsync(ListaId,
            new ItemListaDTO { Descricao = "X", Repeticao = RepeticaoItem.Diaria, Data = Seg, RepetirAte = Seg.AddDays(-1) }, UserId));
        Assert.Equal("Repetir até não pode ser antes da data.", erro.Message);

        await CriarService().AdicionarNaListaAsync(ListaId,
            new ItemListaDTO { Descricao = "X", Repeticao = RepeticaoItem.Mensal, Data = Seg, DiasSemana = [1] }, UserId);
        _itens.Verify(r => r.CriarAsync(It.Is<ItemLista>(i => i.DiasSemana == null && i.Repeticao == RepeticaoItem.Mensal)));
    }

    [Fact]
    public async Task Periodo_expande_as_repeticoes_com_as_excecoes()
    {
        _itens.Setup(r => r.ListarDaListaAsync(ListaId, UserId)).ReturnsAsync(
        [
            new ItemLista { Id = 5, ListaId = ListaId, Descricao = "Academia", Data = Seg, Hora = new TimeOnly(7, 0), Repeticao = RepeticaoItem.Semanal, DiasSemana = [1, 3, 5] },
            new ItemLista { Id = 6, ListaId = ListaId, Descricao = "Dentista", Data = Seg.AddDays(3), Hora = new TimeOnly(14, 30) },
            new ItemLista { Id = 7, ListaId = ListaId, Descricao = "Sem data" },
        ]);
        _itens.Setup(r => r.ListarOcorrenciasAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync(
            [new OcorrenciaItemLista { ItemId = 5, Data = Seg.AddDays(2), Feito = true }]);

        var semana = await CriarService().ListarPeriodoAsync(ListaId, Seg, Seg.AddDays(6), UserId);

        Assert.Equal(new[] { "Academia 26/10 -", "Academia 28/10 F", "Dentista 29/10 -", "Academia 30/10 -" },
            semana.Select(o => $"{o.Descricao} {o.Data:dd/MM} {(o.Feito ? "F" : "-")}"));
        Assert.True(semana[0].Repete);
        Assert.False(semana[2].Repete);
        Assert.Equal(5, semana[0].ItemId);
    }

    [Fact]
    public async Task Periodo_de_mais_de_366_dias_ou_invertido_e_recusado()
    {
        await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().ListarPeriodoAsync(ListaId, Seg, Seg.AddDays(367), UserId));
        await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().ListarPeriodoAsync(ListaId, Seg, Seg.AddDays(-1), UserId));
    }

    [Fact]
    public async Task Marcar_feita_so_esta_ocorrencia_grava_a_excecao_com_a_hora()
    {
        ItemSemanal();
        var ocorrencia = new OcorrenciaItemLista();
        _itens.Setup(r => r.GravarOcorrenciaAsync(5, Seg.AddDays(2), UserId, It.IsAny<Action<OcorrenciaItemLista>>()))
            .Callback<int, DateOnly, int, Action<OcorrenciaItemLista>>((_, _, _, alterar) => alterar(ocorrencia))
            .Returns(Task.CompletedTask);

        await CriarService().AtualizarOcorrenciaAsync(ListaId, 5, Seg.AddDays(2), new AlterarOcorrenciaDTO { Feito = true, Hora = new TimeOnly(8, 0) }, UserId);

        Assert.True(ocorrencia.Feito);
        Assert.Equal(Agora.UtcDateTime, ocorrencia.FeitoEm);
        Assert.Equal(new TimeOnly(8, 0), ocorrencia.NovaHora);
        Assert.Null(ocorrencia.NovaData);
        Assert.False(ocorrencia.Pulada);
    }

    [Fact]
    public async Task Mover_para_a_propria_data_original_limpa_a_mudanca_e_pular_marca_pulada()
    {
        ItemSemanal();
        var ocorrencia = new OcorrenciaItemLista { NovaData = Seg.AddDays(1) };
        _itens.Setup(r => r.GravarOcorrenciaAsync(5, Seg, UserId, It.IsAny<Action<OcorrenciaItemLista>>()))
            .Callback<int, DateOnly, int, Action<OcorrenciaItemLista>>((_, _, _, alterar) => alterar(ocorrencia))
            .Returns(Task.CompletedTask);

        await CriarService().AtualizarOcorrenciaAsync(ListaId, 5, Seg, new AlterarOcorrenciaDTO { Data = Seg, Pulada = true }, UserId);

        Assert.Null(ocorrencia.NovaData);
        Assert.True(ocorrencia.Pulada);
    }

    [Fact]
    public async Task Ocorrencia_fora_da_regra_ou_de_item_que_nao_repete_e_recusada()
    {
        ItemSemanal();
        ItemExistente(6);

        var foraDaRegra = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AtualizarOcorrenciaAsync(ListaId, 5, Seg.AddDays(1), new AlterarOcorrenciaDTO { Feito = true }, UserId));
        var naoRepete = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            CriarService().AtualizarOcorrenciaAsync(ListaId, 6, Seg, new AlterarOcorrenciaDTO { Feito = true }, UserId));

        Assert.Equal("Esta data não é uma ocorrência do item.", foraDaRegra.Message);
        Assert.Equal("Este item não se repete: altere o próprio item.", naoRepete.Message);
    }

    [Fact]
    public async Task Esta_e_as_proximas_encerra_a_regra_antiga_e_cria_a_nova_na_mesma_operacao()
    {
        ItemSemanal();
        _itens.Setup(r => r.AtualizarAsync(It.IsAny<ItemLista>(), UserId)).Callback(() => _uow.Registrar("Antigo")).Returns(Task.CompletedTask);
        _itens.Setup(r => r.ApagarOcorrenciasAsync(5, Seg.AddDays(9), UserId)).Callback(() => _uow.Registrar("Excecoes")).ReturnsAsync(1);
        var novo = new ItemListaDTO { Descricao = "Academia", Repeticao = RepeticaoItem.Semanal, DiasSemana = [2, 4], Hora = new TimeOnly(18, 0) };

        var criado = await CriarService().DividirAsync(ListaId, 5, Seg.AddDays(9), novo, UserId);

        _itens.Verify(r => r.AtualizarAsync(It.Is<ItemLista>(i => i.Id == 5 && i.RepetirAte == Seg.AddDays(8) && i.DiasSemana!.SequenceEqual(new[] { 1, 3, 5 })), UserId));
        _itens.Verify(r => r.CriarAsync(It.Is<ItemLista>(i => i.ListaId == ListaId && i.Data == Seg.AddDays(9) && i.Hora == new TimeOnly(18, 0) && i.ItemOrigemId == 5)));
        Assert.Equal(100, criado.Id);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Esta_e_as_proximas_na_primeira_ocorrencia_so_atualiza_o_item()
    {
        ItemSemanal();

        var resultado = await CriarService().DividirAsync(ListaId, 5, Seg, new ItemListaDTO { Descricao = "Academia", Repeticao = RepeticaoItem.Diaria }, UserId);

        Assert.Equal(5, resultado.Id);
        _itens.Verify(r => r.AtualizarAsync(It.Is<ItemLista>(i => i.Id == 5 && i.Repeticao == RepeticaoItem.Diaria && i.Data == Seg), UserId));
        _itens.Verify(r => r.CriarAsync(It.IsAny<ItemLista>()), Times.Never);
    }

    [Fact]
    public async Task Encerrar_a_partir_de_uma_ocorrencia_ou_excluir_desde_a_primeira()
    {
        ItemSemanal();

        await CriarService().EncerrarAsync(ListaId, 5, Seg.AddDays(4), UserId);
        await CriarService().EncerrarAsync(ListaId, 5, Seg, UserId);

        _itens.Verify(r => r.AtualizarAsync(It.Is<ItemLista>(i => i.RepetirAte == Seg.AddDays(3)), UserId));
        _itens.Verify(r => r.ApagarOcorrenciasAsync(5, Seg.AddDays(4), UserId));
        _itens.Verify(r => r.RemoverAsync(5, UserId));
    }

    [Fact]
    public async Task Desmarcar_todos_limpa_tambem_as_ocorrencias_na_mesma_operacao()
    {
        _itens.Setup(r => r.DesmarcarTodosAsync(ListaId, Agora.UtcDateTime, UserId)).Callback(() => _uow.Registrar("Itens")).ReturnsAsync(2);
        _itens.Setup(r => r.DesmarcarOcorrenciasAsync(ListaId, UserId)).Callback(() => _uow.Registrar("Ocorrencias")).ReturnsAsync(3);

        Assert.Equal(5, await CriarService().DesmarcarTodosAsync(ListaId, UserId));
        Assert.Empty(_uow.Fora);
    }
}
