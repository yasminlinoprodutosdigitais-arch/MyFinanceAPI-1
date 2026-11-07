using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #76, fase 1 (05/11/2026): prévia ao vivo e aplicar uma regra nos lançamentos que já existem (só nos sem
/// categoria), a sugestão de categoria pelo histórico e as "Sugestões do seu extrato" (textos repetidos sem regra).
/// </summary>
public class ClassificacaoServiceTests
{
    private const int UserId = 7;
    private const int Alimentacao = 3;
    private const int Transporte = 4;

    private readonly Mock<IRegraClassificacaoRepository> _regras = new();
    private readonly Mock<IExtratoBancarioItemRepository> _itens = new();
    private readonly List<ExtratoBancarioItem> _lancamentos = new();
    private List<ExtratoBancarioItem> _gravados = new();
    private IReadOnlyDictionary<int, int>? _uso;

    public ClassificacaoServiceTests()
    {
        _itens.Setup(r => r.ListarParaClassificacaoAsync(UserId)).ReturnsAsync(() => _lancamentos);
        _itens.Setup(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<ExtratoBancarioItem>>()))
            .Callback<IEnumerable<ExtratoBancarioItem>>(l => _gravados = l.ToList())
            .Returns(Task.CompletedTask);
        _regras.Setup(r => r.RegistrarUsoAsync(UserId, It.IsAny<IReadOnlyDictionary<int, int>>()))
            .Callback<int, IReadOnlyDictionary<int, int>>((_, u) => _uso = u)
            .Returns(Task.CompletedTask);
    }

    private static ExtratoBancarioItem Item(int id, string? descricao, int? categoria = null, string? pessoa = null, string data = "2026-10-01", int? banco = null) => new()
    {
        Id = id, UsuarioId = UserId, Descricao = descricao, NomePessoaTransacao = pessoa, CategoriaId = categoria,
        Categoria = categoria is int c ? new Categoria { Id = c, Nome = c == Alimentacao ? "Alimentação" : "Transporte" } : null,
        TipoLancamento = TipoLancamentoExtrato.Saida, Valor = 10, DataMovimentacao = DateOnly.Parse(data), BancoId = banco
    };

    private ClassificacaoService Servico() => new(_regras.Object, _itens.Object);

    // ---------- prévia ----------

    [Fact]
    public async Task Previa_conta_os_que_batem_os_sem_categoria_e_os_conflitos_com_exemplos()
    {
        _lancamentos.AddRange([
            Item(1, "UBER *TRIP", data: "2026-02-03"), Item(2, null, pessoa: "Uber", data: "2026-02-01"),
            Item(3, "UBER *EATS", Alimentacao, data: "2026-02-07"), Item(4, "Uber X", Transporte, data: "2026-02-08"), Item(5, "Padaria")
        ]);

        var previa = await Servico().PreviaAsync(UserId, new PreviaRegraDTO { Texto = " uber ", CategoriaId = Transporte });

        Assert.Equal((4, 2, 1), (previa.Total, previa.SemCategoria, previa.Conflitos));
        Assert.Equal([1, 2, 4, 3], previa.Exemplos.Select(e => e.Id));   // sem categoria primeiro, depois os mais recentes
        Assert.Equal("Uber", previa.Exemplos[1].Texto);                  // sem descrição: o nome da pessoa
        var conflito = Assert.Single(previa.ExemplosConflito);
        Assert.Equal((3, "Alimentação"), (conflito.Id, conflito.CategoriaNome));
    }

    [Fact]
    public async Task Previa_respeita_campo_operador_e_conta_e_texto_curto_nao_conta()
    {
        _lancamentos.AddRange([Item(1, "POSTO SHELL", banco: 5), Item(2, "AUTO POSTO", banco: 5), Item(3, "POSTO IPIRANGA", banco: 6)]);

        var previa = await Servico().PreviaAsync(UserId, new PreviaRegraDTO
        {
            Texto = "posto", Campo = CamposRegra.Descricao, Operador = OperadoresRegra.ComecaCom, BancoId = 5
        });

        Assert.Equal([1], previa.Exemplos.Select(e => e.Id));
        Assert.Equal(0, (await Servico().PreviaAsync(UserId, new PreviaRegraDTO { Texto = "P" })).Total);
    }

    // ---------- aplicar ----------

    [Fact]
    public async Task Aplicar_grava_categoria_tipo_e_pessoa_so_nos_sem_categoria_e_conta_o_uso()
    {
        _regras.Setup(r => r.ObterPorIdAsync(9, UserId)).ReturnsAsync(new RegraClassificacao
        {
            Id = 9, UsuarioId = UserId, Texto = "IFOOD", CategoriaId = Alimentacao, TipoMovimentacaoId = 5, PessoaMovimentacaoId = 20, Ativa = false
        });
        var comTipo = Item(2, "IFOOD Mercado");
        comTipo.TipoMovimentacaoId = 6;
        _lancamentos.AddRange([Item(1, "IFOOD *Restaurante"), comTipo, Item(3, "Ifood", Transporte), Item(4, "Uber")]);

        var resultado = await Servico().AplicarAsync(UserId, 9);   // desligada: "aplicar de novo" é pedido explícito

        Assert.Equal(2, resultado.Quantidade);
        Assert.Equal([1, 2], resultado.ItemIds);
        Assert.Equal([1, 2], _gravados.Select(i => i.Id));
        Assert.All(_gravados, i => Assert.Equal(Alimentacao, i.CategoriaId));
        Assert.Equal(5, _gravados[0].TipoMovimentacaoId);
        Assert.Equal(6, _gravados[1].TipoMovimentacaoId);      // tipo já escolhido fica
        Assert.Equal(20, _gravados[0].PessoaMovimentacaoId);
        Assert.Equal(Transporte, _lancamentos[2].CategoriaId); // com categoria não muda
        Assert.Equal(2, _uso![9]);
    }

    [Fact]
    public async Task Aplicar_sem_alvos_nao_grava_nem_conta_uso()
    {
        _regras.Setup(r => r.ObterPorIdAsync(9, UserId)).ReturnsAsync(new RegraClassificacao { Id = 9, UsuarioId = UserId, Texto = "IFOOD", CategoriaId = 1, Ativa = true });
        _lancamentos.Add(Item(1, "Uber"));

        Assert.Equal(0, (await Servico().AplicarAsync(UserId, 9)).Quantidade);
        _itens.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<ExtratoBancarioItem>>()), Times.Never);
        Assert.Null(_uso);
    }

    [Fact]
    public async Task Aplicar_regra_de_outro_usuario_e_nao_encontrado()
    {
        await Assert.ThrowsAsync<NaoEncontradoException>(() => Servico().AplicarAsync(UserId, 99));
    }

    // ---------- sugestão de categoria ----------

    [Fact]
    public void Sugestao_pela_categoria_mais_usada_na_mesma_descricao_sem_numeros()
    {
        var historico = new[]
        {
            Item(1, "PADARIA 123", Alimentacao), Item(2, "Padaria 456", Alimentacao), Item(3, "PADARIA", Transporte),
            Item(4, null, Transporte, pessoa: "UBER TRIP"), Item(5, "Mercado X", Alimentacao)
        };
        var semCategoria = new[] { Item(10, "Padaria 789"), Item(11, null, pessoa: "Uber Trip"), Item(12, "Loja nova") };

        var sugestoes = ClassificacaoService.Sugerir(historico, semCategoria);

        Assert.Equal([(10, Alimentacao, 2), (11, Transporte, 1)], sugestoes.Select(s => (s.ItemId, s.CategoriaId, s.Vezes)));
    }

    // ---------- sugestões de regra ----------

    [Theory]
    [InlineData(null, "MERCADO LIVRE", "MERCADO LIVRE")]
    [InlineData("Compra no débito - IFOOD *RESTAURANTE 123", "Eu", "IFOOD")]
    [InlineData("UBER *TRIP HELP.UBER.COM", null, "UBER")]
    [InlineData("PARC03/05", "HAVAN ECOVILLE", "HAVAN ECOVILLE")]
    [InlineData(null, "MP *MEVARIEDADES", "MP *MEVARIEDADES")]
    [InlineData(null, "DL*GOOGLE", "DL*GOOGLE")]
    public void Texto_sugerido_igual_ao_do_front(string? descricao, string? pessoa, string esperado)
    {
        Assert.Equal(esperado, ClassificacaoService.TextoSugerido(descricao, pessoa));
    }

    [Fact]
    public void Sugere_textos_repetidos_sem_categoria_sem_regra_com_a_categoria_mais_usada()
    {
        var todos = new[]
        {
            Item(1, null, pessoa: "MERCADO LIVRE"), Item(2, null, pessoa: "Mercado Livre"), Item(3, null, pessoa: "MERCADO LIVRE"),
            Item(4, null, pessoa: "NETFLIX"), Item(5, null, pessoa: "Netflix"), Item(6, null, Alimentacao, pessoa: "NETFLIX"),
            Item(7, null, pessoa: "Loja única"),                                            // só 1: não sugere
            Item(8, "UBER *TRIP"), Item(9, "UBER *EATS"),                                   // já tem regra ligada que pega
            Item(10, null, pessoa: "SHOPEE"), Item(11, null, pessoa: "SHOPEE")              // texto já tem regra (desligada)
        };
        var regras = new[]
        {
            new RegraClassificacao { Id = 1, Texto = "UBER", Ativa = true, CategoriaId = Transporte },
            new RegraClassificacao { Id = 2, Texto = "SHOPEE", Ativa = false, CategoriaId = Transporte }
        };

        var sugestoes = ClassificacaoService.SugerirRegras(todos, regras);

        Assert.Equal([("MERCADO LIVRE", 3, (int?)null, 0), ("NETFLIX", 2, Alimentacao, 1)],
            sugestoes.Select(s => (s.Texto, s.Quantidade, s.CategoriaId, s.VezesCategoria)));
        Assert.Equal("Alimentação", sugestoes[1].CategoriaNome);
    }
}
