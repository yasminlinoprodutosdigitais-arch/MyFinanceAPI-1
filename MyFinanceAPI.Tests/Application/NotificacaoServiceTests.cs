using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #73: avisos do sino calculados na hora, com "novo" pelo "lido até".</summary>
public class NotificacaoServiceTests
{
    private const int UserId = 7;

    /// <summary>Quarta-feira, 11/11/2026, 12h em São Paulo.</summary>
    private static readonly DateTimeOffset Agora = new(2026, 11, 11, 15, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Hoje = new(2026, 11, 11);
    /// <summary>Meia-noite de 11/11 em São Paulo (UTC-3).</summary>
    private static readonly DateTime InicioDeHoje = new(2026, 11, 11, 3, 0, 0, DateTimeKind.Utc);

    private sealed class RelogioFalso(DateTimeOffset agora) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => agora;
    }

    private readonly Mock<IPreferenciaNotificacaoRepository> _pref = new();
    private readonly Mock<ITransacaoService> _transacoes = new();
    private readonly Mock<IResumoContasService> _resumo = new();
    private readonly Mock<IItemListaService> _agenda = new();
    private readonly Mock<IAcertosService> _acertos = new();

    public NotificacaoServiceTests()
    {
        _transacoes.Setup(t => t.GetContaVencida(It.IsAny<DateTime>(), UserId)).ReturnsAsync([]);
        _transacoes.Setup(t => t.BuscarContasMensais(It.IsAny<DateTime>(), UserId, false)).ReturnsAsync([]);
        _resumo.Setup(r => r.ObterAsync(UserId, It.IsAny<string>())).ReturnsAsync(new ResumoContasDTO());
        _agenda.Setup(a => a.ListarAgendaAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), UserId)).ReturnsAsync([]);
        _acertos.Setup(a => a.ObterAsync(UserId, null, null, null)).ReturnsAsync(new AcertosDTO());
    }

    private NotificacaoService CriarService() =>
        new(_pref.Object, _transacoes.Object, _resumo.Object, _agenda.Object, _acertos.Object, new RelogioFalso(Agora));

    private void Preferencias(string config, DateTime? lidasAte = null) =>
        _pref.Setup(p => p.ObterAsync(UserId)).ReturnsAsync(new PreferenciaNotificacao { UsuarioId = UserId, Config = config, LidasAte = lidasAte });

    private static TransacaoDTO Conta(int id, DateOnly data, decimal valor, string situacao = StatusTransacao.Pendente, string sub = "Despesa") =>
        new() { Id = id, Nome = $"Conta {id}", Data = data, Valor = valor, Situacao = situacao, SubcategoriaNome = sub, CategoriaNome = "Casa" };

    [Fact]
    public async Task Vencidas_viram_um_aviso_so_com_as_recentes_por_padrao()
    {
        _transacoes.Setup(t => t.GetContaVencida(It.IsAny<DateTime>(), UserId)).ReturnsAsync(
        [
            Conta(1, Hoje.AddDays(-2), 100), Conta(2, Hoje.AddDays(-30), 50), Conta(3, Hoje.AddDays(-100), 900)
        ]);

        var r = await CriarService().ListarAsync(UserId);

        var a = Assert.Single(r.Avisos);
        Assert.Equal("vencidas", a.Tipo);
        Assert.Equal("vencidas:2026-11-11", a.Id);
        Assert.Equal("2 contas vencidas", a.Titulo);
        Assert.Equal(150, a.Valor);
        Assert.Equal("Mais 1 há mais de 90 dias, fora deste aviso", a.Detalhe);
        Assert.Equal(InicioDeHoje, a.Desde);
    }

    [Fact]
    public async Task Vencidas_com_as_antigas_somam_igual_ao_selo_do_menu()
    {
        Preferencias("""{"vencidas":{"ativo":true,"soUltimos90Dias":false}}""");
        _transacoes.Setup(t => t.GetContaVencida(It.IsAny<DateTime>(), UserId)).ReturnsAsync(
        [
            Conta(1, Hoje.AddDays(-2), 100), Conta(3, Hoje.AddDays(-100), 900)
        ]);

        var a = Assert.Single((await CriarService().ListarAsync(UserId)).Avisos);

        Assert.Equal("2 contas vencidas", a.Titulo);
        Assert.Equal(1000, a.Valor);
        Assert.Equal("1 recente · 1 há mais de 90 dias", a.Detalhe);
    }

    [Fact]
    public async Task Contas_a_vencer_so_despesas_em_aberto_dentro_da_antecedencia()
    {
        _transacoes.Setup(t => t.BuscarContasMensais(It.IsAny<DateTime>(), UserId, false)).ReturnsAsync(
        [
            Conta(1, Hoje, 100), Conta(2, Hoje.AddDays(1), 20), Conta(3, Hoje.AddDays(2), 70),
            Conta(4, Hoje, 10, StatusTransacao.PagoNoPrazo), Conta(5, Hoje, 500, sub: "Receita"),
            Conta(6, Hoje.AddDays(-1), 15)
        ]);

        var avisos = (await CriarService().ListarAsync(UserId)).Avisos.Where(a => a.Tipo == "contas-a-vencer").ToList();

        Assert.Equal(["Conta 1 vence hoje", "Conta 2 vence amanhã"], avisos.OrderBy(a => a.Data).Select(a => a.Titulo));
        var amanha = avisos.Single(a => a.TransacaoId == 2);
        Assert.Equal(InicioDeHoje, amanha.Desde);
        Assert.Equal("vencer:2:2026-11-12", amanha.Id);
        Assert.Equal(InicioDeHoje.AddDays(-1), avisos.Single(a => a.TransacaoId == 1).Desde);
    }

    [Fact]
    public async Task Contas_a_vencer_na_virada_do_mes_busca_os_dois_meses()
    {
        Preferencias("""{"contasAVencer":{"ativo":true,"diasAntes":3}}""");
        var service = new NotificacaoService(_pref.Object, _transacoes.Object, _resumo.Object, _agenda.Object, _acertos.Object,
            new RelogioFalso(new DateTimeOffset(2026, 11, 29, 15, 0, 0, TimeSpan.Zero)));

        await service.ListarAsync(UserId);

        _transacoes.Verify(t => t.BuscarContasMensais(new DateTime(2026, 11, 1), UserId, false), Times.Once);
        _transacoes.Verify(t => t.BuscarContasMensais(new DateTime(2026, 12, 1), UserId, false), Times.Once);
    }

    [Fact]
    public async Task Fatura_avisa_fechamento_e_vencimento_dentro_da_antecedencia()
    {
        _resumo.Setup(r => r.ObterAsync(UserId, "2026-11")).ReturnsAsync(new ResumoContasDTO
        {
            Contas =
            [
                new() { BancoId = 1, Nome = "Nubank", EhCredito = true, Ativo = true, Fatura = 862.9m, DataFechamento = Hoje.AddDays(2), DataVencimento = Hoje.AddDays(9) },
                new() { BancoId = 2, Nome = "Inativo", EhCredito = true, Ativo = false, Fatura = 10, DataFechamento = Hoje },
                new() { BancoId = 3, Nome = "Zerado", EhCredito = true, Ativo = true, Fatura = 0, DataFechamento = Hoje },
                new() { BancoId = 4, Nome = "Débito", EhCredito = false, Ativo = true, Fatura = 10, DataFechamento = Hoje }
            ]
        });

        var a = Assert.Single((await CriarService().ListarAsync(UserId)).Avisos.Where(x => x.Tipo == "fatura"));

        Assert.Equal("Fatura do Nubank fecha em 2 dias", a.Titulo);
        Assert.Equal(862.9m, a.Valor);
        Assert.Equal(1, a.BancoId);
        Assert.Equal(InicioDeHoje.AddDays(-1), a.Desde);
    }

    [Fact]
    public async Task Agenda_so_itens_nao_feitos_com_hora_e_planejamento()
    {
        _agenda.Setup(a => a.ListarAgendaAsync(Hoje, Hoje, UserId)).ReturnsAsync(
        [
            new OcorrenciaDTO { ItemId = 1, ListaId = 9, ListaNome = "Casa", Data = Hoje, DataOriginal = Hoje, Hora = new TimeOnly(9, 0), Descricao = "Ligar pro síndico" },
            new OcorrenciaDTO { ItemId = 2, ListaId = 9, ListaNome = "Casa", Data = Hoje, DataOriginal = Hoje, Feito = true, Descricao = "Feito" }
        ]);

        var a = Assert.Single((await CriarService().ListarAsync(UserId)).Avisos.Where(x => x.Tipo == "agenda"));

        Assert.Equal("Ligar pro síndico hoje às 09:00", a.Titulo);
        Assert.Equal("Casa", a.Detalhe);
        Assert.Equal(9, a.ListaId);
        Assert.Equal("agenda:1:2026-11-11", a.Id);
    }

    [Fact]
    public async Task Acertos_uma_pessoa_que_deve_ha_mais_de_N_dias_vale_a_semana_desde_segunda()
    {
        _acertos.Setup(a => a.ObterAsync(UserId, null, null, null)).ReturnsAsync(new AcertosDTO
        {
            Pessoas =
            [
                new() { PessoaId = 1, Nome = "Carlos", Falta = 250, Compras = [new() { Data = Hoje.AddDays(-40) }, new() { Data = Hoje.AddDays(-50), Quitada = true }] },
                new() { PessoaId = 2, Nome = "Ana", Falta = 30, Compras = [new() { Data = Hoje.AddDays(-10) }] },
                new() { PessoaId = 3, Nome = "Eu", EhTitular = true, Falta = 99, Compras = [new() { Data = Hoje.AddDays(-90) }] },
                new() { PessoaId = 4, Nome = "Quitado", Falta = 0, Compras = [new() { Data = Hoje.AddDays(-90) }] }
            ]
        });

        var a = Assert.Single((await CriarService().ListarAsync(UserId)).Avisos.Where(x => x.Tipo == "acertos"));

        Assert.Equal("Carlos te deve R$ 250,00", a.Titulo.Replace(' ', ' '));
        Assert.Equal("Em aberto há 40 dias", a.Detalhe);
        Assert.Equal("acertos:2026-11-09", a.Id);
        Assert.Equal(1, a.PessoaId);
        Assert.Equal(InicioDeHoje.AddDays(-2), a.Desde);
    }

    [Fact]
    public async Task Acertos_de_varias_pessoas_viram_um_aviso_so_com_o_total()
    {
        _acertos.Setup(a => a.ObterAsync(UserId, null, null, null)).ReturnsAsync(new AcertosDTO
        {
            Pessoas =
            [
                new() { PessoaId = 1, Nome = "Carlos", Falta = 150, Compras = [new() { Data = Hoje.AddDays(-40) }] },
                new() { PessoaId = 2, Nome = "Ana", Falta = 60, Compras = [new() { Data = Hoje.AddDays(-35) }] }
            ]
        });

        var a = Assert.Single((await CriarService().ListarAsync(UserId)).Avisos.Where(x => x.Tipo == "acertos"));

        Assert.Equal("2 pessoas te devem R$ 210,00", a.Titulo.Replace('\u00A0', ' '));
        Assert.Equal("Há mais de 30 dias", a.Detalhe);
        Assert.Equal(210, a.Valor);
        Assert.Null(a.PessoaId);
    }

    [Fact]
    public async Task Pausado_ate_hoje_o_sino_fica_sem_avisos_e_nao_busca_nada()
    {
        Preferencias("""{"pausadoAte":"2026-11-11"}""");

        var r = await CriarService().ListarAsync(UserId);

        Assert.Empty(r.Avisos);
        Assert.Equal(0, r.Novos);
        Assert.Equal(Hoje, r.PausadoAte);
        _transacoes.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Pausa_que_ja_acabou_nao_vale()
    {
        Preferencias("""{"pausadoAte":"2026-11-10"}""");
        _transacoes.Setup(t => t.GetContaVencida(It.IsAny<DateTime>(), UserId)).ReturnsAsync([Conta(1, Hoje.AddDays(-2), 100)]);

        var r = await CriarService().ListarAsync(UserId);

        Assert.Single(r.Avisos);
        Assert.Null(r.PausadoAte);
    }

    [Fact]
    public async Task Novo_e_o_que_passou_a_valer_depois_do_lido_ate()
    {
        Preferencias("{}", lidasAte: InicioDeHoje.AddHours(-12));
        _transacoes.Setup(t => t.GetContaVencida(It.IsAny<DateTime>(), UserId)).ReturnsAsync([Conta(1, Hoje.AddDays(-2), 100)]);
        _transacoes.Setup(t => t.BuscarContasMensais(It.IsAny<DateTime>(), UserId, false)).ReturnsAsync([Conta(2, Hoje, 20)]);

        var r = await CriarService().ListarAsync(UserId);

        Assert.True(r.Avisos.Single(a => a.Tipo == "vencidas").Novo);
        Assert.False(r.Avisos.Single(a => a.Tipo == "contas-a-vencer").Novo);   // passou a valer ontem
        Assert.Equal(1, r.Novos);
        Assert.Equal(InicioDeHoje.AddHours(-12), r.LidasAte);
    }

    [Fact]
    public async Task Sem_lido_ate_tudo_e_novo()
    {
        _transacoes.Setup(t => t.GetContaVencida(It.IsAny<DateTime>(), UserId)).ReturnsAsync([Conta(1, Hoje.AddDays(-2), 100)]);

        var r = await CriarService().ListarAsync(UserId);

        Assert.Equal(1, r.Novos);
        Assert.Null(r.LidasAte);
    }

    [Fact]
    public async Task Tipo_desligado_nao_aparece_nem_e_buscado()
    {
        Preferencias("""{"vencidas":{"ativo":false},"contasAVencer":{"ativo":false,"diasAntes":1},"fatura":{"ativo":false,"diasAntes":3},"agenda":{"ativo":false},"acertos":{"ativo":false,"diasEmAberto":30}}""");

        var r = await CriarService().ListarAsync(UserId);

        Assert.Empty(r.Avisos);
        _transacoes.VerifyNoOtherCalls();
        _resumo.VerifyNoOtherCalls();
        _agenda.VerifyNoOtherCalls();
        _acertos.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Marcar_lidas_grava_agora()
    {
        await CriarService().MarcarLidasAsync(UserId);

        _pref.Verify(p => p.MarcarLidasAsync(UserId, Agora.UtcDateTime), Times.Once);
    }
}
