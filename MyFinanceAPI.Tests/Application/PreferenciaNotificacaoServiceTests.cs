using Moq;
using MyFinanceAPI.Application.DTO.Notificacoes;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #73: preferências de notificação em jsonb, com padrões no que faltar.</summary>
public class PreferenciaNotificacaoServiceTests
{
    private const int UserId = 7;
    private readonly Mock<IPreferenciaNotificacaoRepository> _repo = new();

    private PreferenciaNotificacaoService CriarService() => new(_repo.Object);

    [Fact]
    public async Task Sem_linha_volta_os_padroes()
    {
        var p = await CriarService().ObterAsync(UserId);

        Assert.True(p.ContasAVencer.Ativo);
        Assert.Equal(1, p.ContasAVencer.DiasAntes);
        Assert.True(p.Vencidas.SoUltimos90Dias);
        Assert.Equal(3, p.Fatura.DiasAntes);
        Assert.Equal(0, p.Agenda.DiasAntes);
        Assert.Equal(30, p.Acertos.DiasEmAberto);
    }

    [Fact]
    public async Task Config_salva_parcial_completa_com_padroes()
    {
        _repo.Setup(r => r.ObterAsync(UserId)).ReturnsAsync(new PreferenciaNotificacao
        {
            UsuarioId = UserId, Config = """{"fatura":{"ativo":false,"diasAntes":5}}"""
        });

        var p = await CriarService().ObterAsync(UserId);

        Assert.False(p.Fatura.Ativo);
        Assert.Equal(5, p.Fatura.DiasAntes);
        Assert.Equal(1, p.ContasAVencer.DiasAntes);
        Assert.Equal(30, p.Acertos.DiasEmAberto);
    }

    [Theory]
    [InlineData("{oops")]
    [InlineData("")]
    [InlineData("null")]
    public void Config_invalida_ou_vazia_vira_padrao(string config)
    {
        var p = PreferenciaNotificacaoService.Ler(config);

        Assert.Equal(1, p.ContasAVencer.DiasAntes);
        Assert.True(p.Agenda.Ativo);
    }

    [Fact]
    public async Task Salvar_grava_jsonb_em_camelCase_e_devolve_o_salvo()
    {
        string? gravado = null;
        _repo.Setup(r => r.SalvarConfigAsync(UserId, It.IsAny<string>())).Callback((int _, string c) => gravado = c).Returns(Task.CompletedTask);
        var dto = new PreferenciasNotificacaoDTO { Agenda = new() { Ativo = false, DiasAntes = 1 }, Acertos = null! };

        var salvo = await CriarService().SalvarAsync(UserId, dto);

        Assert.NotNull(gravado);
        Assert.Contains("\"agenda\":{\"ativo\":false,\"diasAntes\":1}", gravado);
        Assert.Equal(30, salvo.Acertos.DiasEmAberto);
        var relido = PreferenciaNotificacaoService.Ler(gravado);
        Assert.False(relido.Agenda.Ativo);
    }

    [Theory]
    [InlineData(4, 3, 0, 30)]
    [InlineData(1, 2, 0, 30)]
    [InlineData(1, 3, 2, 30)]
    [InlineData(1, 3, 0, 45)]
    public async Task Antecedencia_fora_das_opcoes_e_recusada(int vencer, int fatura, int agenda, int acertos)
    {
        var dto = new PreferenciasNotificacaoDTO
        {
            ContasAVencer = new() { DiasAntes = vencer },
            Fatura = new() { DiasAntes = fatura },
            Agenda = new() { DiasAntes = agenda },
            Acertos = new() { DiasEmAberto = acertos }
        };

        await Assert.ThrowsAsync<RegraNegocioException>(() => CriarService().SalvarAsync(UserId, dto));
        _repo.Verify(r => r.SalvarConfigAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
