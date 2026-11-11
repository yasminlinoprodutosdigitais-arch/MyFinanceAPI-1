using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #79 (10/11/2026): a tela "Nomes do extrato" — os nomes que vieram nos extratos, se uma regra os classifica,
/// a sugestão pelo histórico, classificar (cria a regra "igual a") e juntar regras de nomes parecidos.
/// </summary>
public class NomesExtratoServiceTests
{
    private const int UserId = 7;
    private const int Mercado = 3;
    private const int Lazer = 4;
    private const int Compras = 9;

    private readonly Mock<IRegraClassificacaoRepository> _regras = new();
    private readonly Mock<IExtratoBancarioItemRepository> _itens = new();
    private readonly List<ExtratoBancarioItem> _lancamentos = new();
    private readonly List<RegraClassificacao> _cadastradas = new();
    private readonly List<RegraClassificacao> _criadas = new();
    private readonly List<RegraClassificacao> _alteradas = new();
    private readonly List<int> _removidas = new();
    private List<ExtratoBancarioItem> _gravados = new();
    private int _id;

    public NomesExtratoServiceTests()
    {
        _itens.Setup(r => r.ListarParaClassificacaoAsync(UserId)).ReturnsAsync(() => _lancamentos);
        _itens.Setup(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<ExtratoBancarioItem>>()))
            .Callback<IEnumerable<ExtratoBancarioItem>>(l => _gravados = l.ToList()).Returns(Task.CompletedTask);
        _regras.Setup(r => r.ListarAsync(UserId)).ReturnsAsync(() => _cadastradas);
        _regras.Setup(r => r.CriarAsync(It.IsAny<RegraClassificacao>()))
            .ReturnsAsync((RegraClassificacao r) => { r.Id = 500 + _criadas.Count; _criadas.Add(r); return r; });
        _regras.Setup(r => r.AtualizarAsync(It.IsAny<RegraClassificacao>(), UserId))
            .Callback<RegraClassificacao, int>((r, _) => _alteradas.Add(r)).Returns(Task.CompletedTask);
        _regras.Setup(r => r.RemoverAsync(It.IsAny<int>(), UserId))
            .ReturnsAsync((int id, int _) => { _removidas.Add(id); return _cadastradas.First(r => r.Id == id); });
        _regras.Setup(r => r.RegistrarUsoAsync(UserId, It.IsAny<IReadOnlyDictionary<int, int>>())).Returns(Task.CompletedTask);
    }

    private NomesExtratoService Servico() => new(_regras.Object, _itens.Object, new UnitOfWorkFake());

    private ExtratoBancarioItem Item(string nome, int? categoria = null, string data = "2026-10-01", int? tipo = null, bool semelhanca = false)
    {
        var item = new ExtratoBancarioItem
        {
            Id = ++_id, UsuarioId = UserId, Descricao = "Compra no débito - " + nome, NomePessoaTransacao = nome,
            CategoriaId = categoria, TipoMovimentacaoId = tipo, ClassificadoPorSemelhanca = semelhanca,
            Categoria = categoria is int c ? new Categoria { Id = c, Nome = "C" + c } : null,
            TipoLancamento = TipoLancamentoExtrato.Saida, Valor = 10, DataMovimentacao = DateOnly.Parse(data)
        };
        _lancamentos.Add(item);
        return item;
    }

    private RegraClassificacao Regra(int id, string texto, int? categoria, string operador = OperadoresRegra.Igual,
        string campo = CamposRegra.NomeExtrato, int? tipo = null)
    {
        var regra = new RegraClassificacao
        {
            Id = id, UsuarioId = UserId, Texto = texto, Campo = campo, Operador = operador, CategoriaId = categoria,
            TipoMovimentacaoId = tipo, Ativa = true, Ordem = id
        };
        _cadastradas.Add(regra);
        return regra;
    }

    // ---------- listar ----------

    [Fact]
    public void Junta_os_lancamentos_pelo_nome_com_contagem_e_ultimo_uso()
    {
        Item("CIRCUITO", data: "2026-10-01");
        Item("Circuito", Mercado, "2026-11-03");
        Item("PADARIA");

        var nomes = NomesExtratoService.Montar(_lancamentos, _cadastradas);

        var circuito = nomes.Single(n => n.Texto == "CIRCUITO");
        Assert.Equal((2, 1, new DateOnly(2026, 11, 3)), (circuito.Lancamentos, circuito.SemCategoria, circuito.UltimoUso));
        Assert.Equal(2, nomes.Count);
        Assert.Equal("CIRCUITO", nomes[0].Texto);   // mais lançamentos primeiro
    }

    [Fact]
    public void Cobertura_igual_outra_semelhanca_e_nenhuma()
    {
        Item("CIRCUITO"); Item("SUPERMERCADO BOM"); Item("BARDOISIRMAOS"); Item("PADARIA");
        Regra(1, "CIRCUITO", Mercado);
        Regra(2, "SUPERMERCADO", Mercado, OperadoresRegra.Contem, CamposRegra.DescricaoOuPessoa);
        Regra(3, "BAR DOIS IRMAOS", Lazer, tipo: 6);

        var nomes = NomesExtratoService.Montar(_lancamentos, _cadastradas).ToDictionary(n => n.Texto);

        Assert.Equal((CoberturaNome.Igual, 1, (int?)Mercado), (nomes["CIRCUITO"].Cobertura, nomes["CIRCUITO"].RegraId, nomes["CIRCUITO"].CategoriaId));
        Assert.Equal((CoberturaNome.Outra, 2), (nomes["SUPERMERCADO BOM"].Cobertura, nomes["SUPERMERCADO BOM"].RegraId));
        var bar = nomes["BARDOISIRMAOS"];
        Assert.Equal((CoberturaNome.Semelhanca, 3, "BAR DOIS IRMAOS", (int?)Lazer, (int?)6),
            (bar.Cobertura, bar.RegraId, bar.RegraTexto, bar.CategoriaId, bar.TipoMovimentacaoId));
        Assert.Equal(CoberturaNome.Nenhuma, nomes["PADARIA"].Cobertura);
    }

    [Fact]
    public void Sugestao_vem_do_historico_do_nome_com_o_tipo_mais_usado_na_categoria()
    {
        Item("PADARIA", Mercado, tipo: 2); Item("PADARIA", Mercado, tipo: 2); Item("PADARIA", Lazer); Item("PADARIA");

        var padaria = NomesExtratoService.Montar(_lancamentos, _cadastradas).Single();

        Assert.Equal(((int?)Mercado, (int?)2), (padaria.SugestaoCategoriaId, padaria.SugestaoTipoMovimentacaoId));
    }

    [Fact]
    public void Sem_historico_e_sem_regra_nao_sugere()
    {
        Item("PADARIA");

        var padaria = NomesExtratoService.Montar(_lancamentos, _cadastradas).Single();

        Assert.Null(padaria.SugestaoCategoriaId);
    }

    [Fact]
    public void Ignora_o_Eu_e_lancamento_sem_nome()
    {
        _lancamentos.Add(new ExtratoBancarioItem { Id = 90, Descricao = "", NomePessoaTransacao = "EU", TipoLancamento = "Saída" });
        _lancamentos.Add(new ExtratoBancarioItem { Id = 91, Descricao = null, NomePessoaTransacao = null, TipoLancamento = "Saída" });

        Assert.Empty(NomesExtratoService.Montar(_lancamentos, _cadastradas));
    }

    // ---------- classificar ----------

    [Fact]
    public async Task Classificar_cria_a_regra_igual_e_aplica_nos_sem_categoria_e_por_semelhanca()
    {
        var semCategoria = Item("PADARIA");
        var parecido = Item("PADARIA", Lazer, semelhanca: true);
        var escolhido = Item("PADARIA", Compras);

        var r = await Servico().ClassificarAsync(UserId, new ClassificarNomesDTO
        {
            Itens = [new ClassificarNomeDTO { Texto = "padaria", CategoriaId = Mercado, TipoMovimentacaoId = 2 }],
            AplicarAosImportados = true
        });

        var regra = Assert.Single(_criadas);
        Assert.Equal(("PADARIA", CamposRegra.NomeExtrato, OperadoresRegra.Igual, (int?)Mercado, (int?)2),
            (regra.Texto, regra.Campo, regra.Operador, regra.CategoriaId, regra.TipoMovimentacaoId));
        Assert.Equal([semCategoria.Id, parecido.Id], _gravados.Select(i => i.Id).Order());
        Assert.All(_gravados, i => Assert.Equal((Mercado, false), (i.CategoriaId!.Value, i.ClassificadoPorSemelhanca)));
        Assert.Equal(Compras, escolhido.CategoriaId);   // a escolha manual não muda
        Assert.Equal((1, 2), (r.Regras, r.Lancamentos));
    }

    [Fact]
    public async Task Classificar_sem_aplicar_so_grava_a_regra()
    {
        Item("PADARIA");

        var r = await Servico().ClassificarAsync(UserId, new ClassificarNomesDTO
        {
            Itens = [new ClassificarNomeDTO { Texto = "PADARIA", CategoriaId = Mercado }],
            AplicarAosImportados = false
        });

        Assert.Single(_criadas);
        Assert.Empty(_gravados);
        Assert.Equal(0, r.Lancamentos);
    }

    [Fact]
    public async Task Classificar_nome_que_ja_tem_regra_igual_altera_a_regra()
    {
        Regra(1, "PADARIA", Lazer);

        await Servico().ClassificarAsync(UserId, new ClassificarNomesDTO
        {
            Itens = [new ClassificarNomeDTO { Texto = "PADARIA", CategoriaId = Mercado }], AplicarAosImportados = false
        });

        Assert.Empty(_criadas);
        Assert.Equal(Mercado, Assert.Single(_alteradas).CategoriaId);
    }

    [Fact]
    public async Task Classificar_sem_categoria_nem_tipo_e_recusado()
    {
        await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().ClassificarAsync(UserId, new ClassificarNomesDTO
        {
            Itens = [new ClassificarNomeDTO { Texto = "PADARIA" }]
        }));
    }

    // ---------- juntar parecidos ----------

    [Fact]
    public void Grupos_juntam_regras_igual_parecidas_e_dizem_se_divergem()
    {
        Regra(1, "BAR DOIS IRMAOS", Lazer);
        Regra(2, "BARDOISIRMAOS", Mercado);
        Regra(3, "FARMACIA LED FARMA LT", Mercado, campo: CamposRegra.Pessoa);
        Regra(4, "FARMACIA LED FARMA LTDA", Mercado);
        Regra(5, "FARMACIA", Mercado, OperadoresRegra.Contem);
        Regra(6, "PADARIA", Mercado);
        Item("BARDOISIRMAOS"); Item("BARDOISIRMAOS");

        var grupos = NomesExtratoService.Grupos(_cadastradas, _lancamentos);

        Assert.Equal(2, grupos.Count);
        var bar = grupos[0];   // os que divergem primeiro
        Assert.True(bar.Divergem);
        Assert.Equal([1, 2], bar.Regras.Select(r => r.Id).Order());
        Assert.Equal(2, bar.Regras.Single(r => r.Id == 2).Lancamentos);
        Assert.False(grupos[1].Divergem);
        Assert.Equal([3, 4], grupos[1].Regras.Select(r => r.Id).Order());
    }

    [Fact]
    public async Task Juntar_mantem_uma_regra_apaga_as_outras_e_aplica()
    {
        Regra(1, "BAR DOIS IRMAOS", Lazer);
        Regra(2, "BARDOISIRMAOS", Mercado);
        var daRegraApagada = Item("BARDOISIRMAOS", Mercado);
        var semCategoria = Item("BAR DOIS IRMAOS LTDA");
        var outro = Item("PADARIA", Mercado);

        await Servico().JuntarAsync(UserId, new JuntarNomesDTO { RegraIds = [1, 2], ManterRegraId = 1, AplicarAosImportados = true });

        Assert.Equal([2], _removidas);
        Assert.Equal([daRegraApagada.Id, semCategoria.Id], _gravados.Select(i => i.Id).Order());
        Assert.All(_gravados, i => Assert.Equal(Lazer, i.CategoriaId));
        Assert.Equal(Mercado, outro.CategoriaId);
    }

    [Fact]
    public async Task Juntar_recusa_regra_que_nao_e_do_grupo_ou_do_usuario()
    {
        Regra(1, "BAR DOIS IRMAOS", Lazer);

        await Assert.ThrowsAsync<RegraNegocioException>(() =>
            Servico().JuntarAsync(UserId, new JuntarNomesDTO { RegraIds = [1, 99], ManterRegraId = 1 }));
        await Assert.ThrowsAsync<RegraNegocioException>(() =>
            Servico().JuntarAsync(UserId, new JuntarNomesDTO { RegraIds = [1], ManterRegraId = 2 }));
    }

    private sealed class UnitOfWorkFake : IUnitOfWork
    {
        public Task ExecuteAsync(Func<Task> operacao) => operacao();
        public Task<T> ExecuteAsync<T>(Func<Task<T>> operacao) => operacao();
    }
}
