using Moq;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #76, fase 6: a nota fica no lançamento e já classifica quando o total bate.</summary>
public class SalvarNotaFiscalTests
{
    private const int UserId = 7;
    private readonly Mock<IRegraClassificacaoRepository> _regras = new();
    private readonly Mock<IExtratoBancarioItemRepository> _itens = new();
    private readonly Mock<IPessoaMovimentacaoRepository> _pessoas = new();
    private readonly Mock<INotaFiscalRepository> _notas = new();
    private readonly UnitOfWorkFalso _uow = new();
    private readonly ExtratoBancarioItem _lancamento = new() { Id = 10, UsuarioId = UserId, Valor = 200, TipoLancamento = TipoLancamentoExtrato.Saida };
    private NotaFiscal? _gravada;
    private IReadOnlyList<ExtratoItemParte>? _partes;

    public SalvarNotaFiscalTests()
    {
        _itens.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(_lancamento);
        _notas.Setup(r => r.SubstituirAsync(It.IsAny<NotaFiscal>())).Callback<NotaFiscal>(n => { _uow.Registrar("Nota"); _gravada = n; }).Returns(Task.CompletedTask);
        _itens.Setup(r => r.SubstituirPartesAsync(10, It.IsAny<IReadOnlyList<ExtratoItemParte>>()))
            .Callback<int, IReadOnlyList<ExtratoItemParte>>((_, p) => { _uow.Registrar("Partes"); _partes = p; }).Returns(Task.CompletedTask);
    }

    private NotaFiscalService Servico() => new(_regras.Object, _notas.Object, _itens.Object, _pessoas.Object, _uow);

    private static NotaFiscalDTO Nota(decimal total, params (decimal valor, int? cat)[] itens) => new()
    {
        Estabelecimento = "Circuito", Total = total,
        Itens = itens.Select((i, n) => new NotaFiscalItemDTO { Nome = "Item " + n, Quantidade = 1, Valor = i.valor, CategoriaId = i.cat }).ToList()
    };

    [Fact]
    public async Task Duas_categorias_divide_o_lancamento_somando_os_itens()
    {
        var r = await Servico().SalvarAsync(UserId, 10, Nota(200, (120, 8), (30, 9), (50, 8)));

        Assert.True(r.Classificado);
        Assert.Equal([(170m, (int?)8), (30m, 9)], _partes!.Select(p => (p.Valor, p.CategoriaId)));
        Assert.Equal(3, _gravada!.Itens.Count);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Uma_categoria_so_classifica_sem_dividir()
    {
        var r = await Servico().SalvarAsync(UserId, 10, Nota(200, (150, 8), (50, 8)));

        Assert.True(r.Classificado);
        Assert.Equal(8, _lancamento.CategoriaId);
        Assert.Null(_partes);
    }

    [Fact]
    public async Task Total_diferente_guarda_a_nota_e_nao_classifica()
    {
        var r = await Servico().SalvarAsync(UserId, 10, Nota(190, (150, 8), (40, 9)));

        Assert.False(r.Classificado);
        Assert.NotNull(_gravada);
        Assert.Null(_partes);
        Assert.Contains("diferente", r.Mensagem);
    }

    [Fact]
    public async Task Ja_dividido_mantem_a_divisao()
    {
        _lancamento.Partes.Add(new ExtratoItemParte { Valor = 200 });

        var r = await Servico().SalvarAsync(UserId, 10, Nota(200, (150, 8), (50, 9)));

        Assert.False(r.Classificado);
        Assert.Null(_partes);
    }

    [Fact]
    public async Task Pessoa_dos_acertos_continua_nas_partes()
    {
        _lancamento.PessoaMovimentacaoId = 5;
        _pessoas.Setup(r => r.ObterPorIdAsync(5, UserId)).ReturnsAsync(new PessoaMovimentacao { Id = 5, EntraNosAcertos = true });

        await Servico().SalvarAsync(UserId, 10, Nota(200, (150, 8), (50, 9)));

        Assert.All(_partes!, p => Assert.Equal(5, p.PessoaMovimentacaoId));
    }

    [Fact]
    public async Task Recusa_categoria_de_outro_usuario_sem_gravar()
    {
        _itens.Setup(r => r.TemCategoriaForaDoUsuarioAsync(It.Is<IReadOnlyCollection<int>>(c => c.Contains(9)), UserId)).ReturnsAsync(true);

        await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().SalvarAsync(UserId, 10, Nota(200, (150, 8), (50, 9))));

        Assert.Null(_gravada);
        Assert.Null(_partes);
    }

    [Fact]
    public void Lancamento_leva_os_nomes_dos_itens_para_a_busca()
    {
        var mapper = MapperFactory.Create();
        Assert.Empty(mapper.Map<ExtratoBancarioItemDTO>(_lancamento).ItensNota);

        _lancamento.NotaFiscal = new NotaFiscal { Itens = [new NotaFiscalItem { Nome = "Ração" }, new NotaFiscalItem { Nome = "Sabão" }] };

        Assert.Equal(["Ração", "Sabão"], mapper.Map<ExtratoBancarioItemDTO>(_lancamento).ItensNota);
    }

    [Fact]
    public async Task Item_sem_categoria_em_todos_nao_classifica()
    {
        var r = await Servico().SalvarAsync(UserId, 10, Nota(200, (200, null)));

        Assert.False(r.Classificado);
        Assert.Null(_lancamento.CategoriaId);
    }

    [Fact]
    public async Task Recusa_lancamento_de_outro_usuario_entrada_e_nota_sem_itens()
    {
        await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().SalvarAsync(UserId, 10, Nota(200)));
        _lancamento.TipoLancamento = TipoLancamentoExtrato.Entrada;
        await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().SalvarAsync(UserId, 10, Nota(200, (200, 8))));
        _lancamento.TipoLancamento = TipoLancamentoExtrato.Saida;
        _lancamento.UsuarioId = 99;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Servico().SalvarAsync(UserId, 10, Nota(200, (200, 8))));
    }
}
