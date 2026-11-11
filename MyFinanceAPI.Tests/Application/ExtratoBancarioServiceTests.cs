using System.Text;
using Moq;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Testes da importação de extrato (BE-P1-09): CSV do Nubank (Data,Valor,Identificador,Descrição),
/// centavos preservados (BE-P0-07), idempotência por ChaveImportacao e reparo de itens truncados.
/// </summary>
public class ExtratoBancarioServiceTests
{
    private const int UserId = 7;
    private const int BancoId = 3;
    private const int TipoCartaoId = 2;
    private const int ExtratoIdGerado = 42;
    private const string Cabecalho = "Data,Valor,Identificador,Descrição";

    private readonly Mock<IExtratoBancarioRepository> _extratoRepo = new();
    private readonly Mock<IExtratoBancarioItemRepository> _itemRepo = new();
    private readonly Mock<IPessoaMovimentacaoRepository> _pessoaRepo = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepo = new();
    private readonly Mock<IRegraClassificacaoRepository> _regrasRepo = new();
    private readonly List<RegraClassificacao> _regras = new();
    private readonly Mock<IBancoService> _bancoService = new();
    private readonly UnitOfWorkFalso _uow = new();

    private readonly HashSet<string> _chavesExistentes = new();
    private readonly List<ExtratoBancarioItem> _candidatosTruncados = new();
    private readonly List<PessoaMovimentacao> _pessoasCadastradas = new();

    private readonly List<ImportacaoResumo> _importacoesAnteriores = new();
    private readonly List<BancoDTO> _bancosDoUsuario = new();

    private ExtratoBancario? _extratoCriado;
    private List<ExtratoBancarioItem> _itensCriados = new();
    private readonly List<ExtratoBancarioItem> _itensAtualizados = new();
    private readonly List<PessoaMovimentacao> _pessoasCriadas = new();

    public ExtratoBancarioServiceTests()
    {
        _regrasRepo.Setup(r => r.ListarAtivasAsync(UserId)).ReturnsAsync(() => _regras);
        _extratoRepo.Setup(r => r.ListarImportacoesAsync(UserId, null)).ReturnsAsync(() => _importacoesAnteriores);
        _bancoService.Setup(s => s.ListarAsync(UserId)).ReturnsAsync(() => _bancosDoUsuario);
        _bancoService.Setup(s => s.ObterPorIdAsync(BancoId, UserId))
            .ReturnsAsync(new BancoDTO { Id = BancoId, NomeBanco = "Nubank", TipoCartaoId = TipoCartaoId });

        _itemRepo.Setup(r => r.ObterChavesImportacaoAsync(UserId, BancoId)).ReturnsAsync(() => _chavesExistentes);
        _itemRepo.Setup(r => r.ObterCandidatosTruncadosAsync(UserId, BancoId)).ReturnsAsync(() => _candidatosTruncados);
        _itemRepo.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ExtratoBancarioItem>>()))
            .Callback<IEnumerable<ExtratoBancarioItem>>(itens => { _uow.Registrar("Itens.CreateRange"); _itensCriados = itens.ToList(); })
            .Returns(Task.CompletedTask);
        _itemRepo.Setup(r => r.UpdateAsync(It.IsAny<ExtratoBancarioItem>()))
            .Callback<ExtratoBancarioItem>(i => { _uow.Registrar("Item.Update"); _itensAtualizados.Add(i); })
            .Returns(Task.CompletedTask);

        _extratoRepo.Setup(r => r.CreateAsync(It.IsAny<ExtratoBancario>()))
            .Callback<ExtratoBancario>(e => { _uow.Registrar("Extrato.Create"); e.Id = ExtratoIdGerado; _extratoCriado = e; })
            .ReturnsAsync((ExtratoBancario e) => e);

        _pessoaRepo.Setup(r => r.VerificaPossuiPessoa(It.IsAny<string?>(), UserId))
            .Returns((string? nome, int _) => Task.FromResult<IEnumerable<PessoaMovimentacao>>(
                _pessoasCadastradas.Where(p => p.NomePessoa == nome).ToList()));
        _pessoaRepo.Setup(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), UserId))
            .ReturnsAsync((PessoaMovimentacao p, int _) =>
            {
                _uow.Registrar("Pessoa.Create");
                p.Id = 900 + _pessoasCriadas.Count;
                _pessoasCriadas.Add(p);
                return p;
            });
    }

    private ExtratoBancarioService Servico() => new(
        _extratoRepo.Object, _itemRepo.Object, _pessoaRepo.Object, _categoriaRepo.Object, _regrasRepo.Object, MapperFactory.Create(), _bancoService.Object, _uow);

    private static MemoryStream Csv(string[] linhas) => new(Encoding.UTF8.GetBytes(string.Join("\n", linhas)));

    private Task<MyFinanceAPI.Application.DTO.Movimentacoes.PreviaImportacaoDTO> Prever(int? banco, params string[] linhas) =>
        Servico().PreverImportacaoAsync(Csv(linhas), UserId, banco);

    private Task<ExtratoImportacaoResultadoDTO> ImportarCom(MyFinanceAPI.Application.DTO.Movimentacoes.OpcoesImportacao opcoes, params string[] linhas) =>
        Servico().ImportarExtratoAsync(Csv(linhas), "NU_2026-10.csv", UserId, BancoId, opcoes);

    private static string Chave(string id, string data, decimal valor, string descricao) =>
        ExtratoBancarioService.GerarChaveImportacao(id, DateOnly.Parse(data), valor, descricao);

    // ---------- prévia (card #74) ----------

    [Fact]
    public async Task Previa_nao_grava_e_marca_o_ja_importado()
    {
        _chavesExistentes.Add(Chave("a", "2026-10-01", -10.00m, "Compra - LOJA"));

        var p = await Prever(BancoId, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA", "02/10/2026,25.50,b,Pix recebido - FULANO");

        Assert.Null(_extratoCriado);
        Assert.Empty(_itensCriados);
        Assert.Equal((1, 1, 25.50m, 0m, "2026-10"), (p.Novos, p.Duplicados, p.Entradas, p.Saidas, p.MesReferencia));
        Assert.Equal([true, false], p.Linhas.Select(l => l.Duplicado));
        Assert.Equal("Entrada", p.Linhas[1].TipoLancamento);
        Assert.False(p.BancoDetectado);
    }

    [Fact]
    public async Task Previa_de_encargo_nao_cria_o_titular_nem_a_categoria()
    {
        await Prever(BancoId, Cabecalho, "01/10/2026,-3.50,a,IOF de compra internacional");

        _pessoaRepo.Verify(r => r.ObterTitularAsync(It.IsAny<int>()), Times.Never);
        _categoriaRepo.Verify(r => r.CriarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task Previa_sem_banco_reconhece_pelo_formato_preferindo_o_tipo_certo()
    {
        _bancosDoUsuario.AddRange([
            new BancoDTO { Id = 1, NomeBanco = "Caixa", Ativo = true },
            new BancoDTO { Id = BancoId, NomeBanco = "Nubank", Ativo = true }
        ]);

        var p = await Prever(null, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA");

        Assert.Equal((BancoId, true, "Nubank"), (p.BancoId, p.BancoDetectado, p.Formato));
        Assert.Single(p.Linhas);
    }

    [Fact]
    public async Task Previa_sem_banco_e_sem_reconhecer_devolve_vazia_para_a_tela_pedir()
    {
        var p = await Prever(null, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA");
        Assert.Null(p.BancoId);
        Assert.Empty(p.Linhas);
    }

    [Fact]
    public async Task Previa_avisa_mes_ja_importado_e_ignora_importacao_desfeita()
    {
        var resumo = new ImportacaoResumo(9, BancoId, DateTime.UtcNow, "x.csv", new DateOnly(2026, 10, 1), 0, DateTime.UtcNow, null, null, 1, 0, 0, 0, null);
        _importacoesAnteriores.Add(resumo);
        Assert.False((await Prever(BancoId, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA")).MesJaImportado);

        _importacoesAnteriores[0] = resumo with { DesfeitoEm = null };
        Assert.True((await Prever(BancoId, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA")).MesJaImportado);
    }

    [Fact]
    public async Task Confirmar_pula_as_desmarcadas_e_grava_a_categoria_escolhida()
    {
        _itemRepo.Setup(r => r.TemCategoriaForaDoUsuarioAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync(false);
        var opcoes = new MyFinanceAPI.Application.DTO.Movimentacoes.OpcoesImportacao
        {
            Pular = [Chave("b", "2026-10-02", -5.00m, "Compra - MERCADO")],
            Categorias = new() { [Chave("a", "2026-10-01", -10.00m, "Compra - LOJA")] = 77 }
        };

        var r = await ImportarCom(opcoes, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA", "02/10/2026,-5.00,b,Compra - MERCADO");

        Assert.Equal((1, 1), (r.QuantidadeCriados, r.QuantidadeDesmarcados));
        Assert.Equal(77, _itensCriados.Single().CategoriaId);
    }

    [Fact]
    public async Task Confirmar_com_categoria_de_outro_usuario_e_recusado_sem_gravar()
    {
        _itemRepo.Setup(r => r.TemCategoriaForaDoUsuarioAsync(It.IsAny<IReadOnlyCollection<int>>(), UserId)).ReturnsAsync(true);
        var opcoes = new MyFinanceAPI.Application.DTO.Movimentacoes.OpcoesImportacao { Categorias = new() { ["x"] = 999 } };

        await Assert.ThrowsAsync<RegraNegocioException>(() => ImportarCom(opcoes, Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA"));
        Assert.Null(_extratoCriado);
    }

    private Task<ExtratoImportacaoResultadoDTO> Importar(params string[] linhas)
    {
        var csv = string.Join("\n", linhas);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var service = new ExtratoBancarioService(
            _extratoRepo.Object, _itemRepo.Object, _pessoaRepo.Object, _categoriaRepo.Object, _regrasRepo.Object, MapperFactory.Create(), _bancoService.Object, _uow);
        return service.ImportarExtratoAsync(stream, "NU_2026-10.csv", UserId, BancoId);
    }

    // ---------- valores e cabeçalho ----------

    [Fact]
    public async Task Importa_as_linhas_preservando_os_centavos_e_o_sinal()
    {
        var resultado = await Importar(
            Cabecalho,
            "01/10/2026,-1234.56,id-1,Compra no débito - PADARIA",
            "03/10/2026,100.10,id-2,Transferência recebida - FULANO");

        Assert.Equal(ExtratoIdGerado, resultado.ExtratoId);
        Assert.Equal(2, resultado.QuantidadeCriados);
        Assert.Equal(0, resultado.QuantidadeIgnorados);
        Assert.Equal(-1134.46m, resultado.ValorTotal);

        Assert.Equal(2, _itensCriados.Count);
        var saida = _itensCriados.Single(i => i.Identificador == "id-1");
        Assert.Equal(1234.56m, saida.Valor);
        Assert.Equal("Saída", saida.TipoLancamento);
        Assert.Equal(new DateOnly(2026, 10, 1), saida.DataMovimentacao);
        var entrada = _itensCriados.Single(i => i.Identificador == "id-2");
        Assert.Equal(100.10m, entrada.Valor);
        Assert.Equal("Entrada", entrada.TipoLancamento);

        Assert.All(_itensCriados, i =>
        {
            Assert.Equal(ExtratoIdGerado, i.ExtratoBancarioId);
            Assert.Equal(UserId, i.UsuarioId);
            Assert.Equal(BancoId, i.BancoId);
            Assert.Equal(TipoCartaoId, i.TipoCartaoId);
            Assert.False(string.IsNullOrEmpty(i.ChaveImportacao));
            Assert.False(string.IsNullOrEmpty(i.ChaveDescricao));
        });
    }

    [Fact]
    public async Task Cabecalho_do_extrato_tem_periodo_quantidade_e_total()
    {
        await Importar(
            Cabecalho,
            "15/10/2026,-10.00,a,Compra - LOJA",
            "02/10/2026,-5.25,b,Compra - MERCADO",
            "28/10/2026,50.00,c,Pix recebido - CICLANO");

        Assert.NotNull(_extratoCriado);
        Assert.Equal(new DateOnly(2026, 10, 2), _extratoCriado!.DataInicioPeriodo);
        Assert.Equal(new DateOnly(2026, 10, 28), _extratoCriado.DataFimPeriodo);
        Assert.Equal(3, _extratoCriado.QuantidadeLancamentos);
        Assert.Equal(34.75m, _extratoCriado.ValorTotal);
        Assert.Equal(BancoId, _extratoCriado.BancoId);
        Assert.Equal("NU_2026-10.csv", _extratoCriado.NomeArquivoOrigem);
        Assert.Equal(UserId, _extratoCriado.UsuarioId);
    }

    [Fact]
    public async Task Arquivo_vazio_nao_grava_nada()
    {
        var resultado = await Importar("");

        Assert.Equal("Arquivo vazio ou inválido.", resultado.Mensagem);
        _extratoRepo.Verify(r => r.CreateAsync(It.IsAny<ExtratoBancario>()), Times.Never);
    }

    [Fact]
    public async Task Linhas_invalidas_sao_ignoradas_e_contadas()
    {
        var resultado = await Importar(
            Cabecalho,
            "2026-10-01,-10.00,a,Data em formato errado",
            "01/10/2026,dez reais,b,Valor inválido",
            "01/10/2026,-10.00,sem-descricao",
            "",
            "01/10/2026,-10.00,ok,Compra - LOJA");

        Assert.Equal(1, resultado.QuantidadeCriados);
        Assert.Equal(3, resultado.QuantidadeIgnorados);   // a linha em branco não conta
        Assert.Single(_itensCriados);
    }

    [Fact]
    public async Task Descricao_com_virgula_fica_inteira()
    {
        await Importar(Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA, filial centro");

        Assert.Equal("Compra - LOJA, filial centro", Assert.Single(_itensCriados).Descricao);
    }

    // ---------- idempotência ----------

    [Fact]
    public async Task Linha_repetida_no_mesmo_arquivo_entra_uma_vez()
    {
        var resultado = await Importar(
            Cabecalho,
            "01/10/2026,-10.00,a,Compra - LOJA",
            "01/10/2026,-10.00,a,Compra - LOJA");

        Assert.Equal(1, resultado.QuantidadeCriados);
        Assert.Single(_itensCriados);
        Assert.Contains("já importados: 1", resultado.Mensagem);
    }

    [Fact]
    public async Task Estorno_com_mesmo_identificador_nao_e_tratado_como_repetido()
    {
        var resultado = await Importar(
            Cabecalho,
            "01/10/2026,-10.00,a,Compra - LOJA",
            "01/10/2026,10.00,a,Compra - LOJA");

        Assert.Equal(2, resultado.QuantidadeCriados);
    }

    [Fact]
    public async Task Reimportar_o_mesmo_arquivo_nao_cria_extrato_nem_itens()
    {
        _chavesExistentes.Add(ExtratoBancarioService.GerarChaveImportacao(
            "a", new DateOnly(2026, 10, 1), -10.00m, "Compra - LOJA"));

        var resultado = await Importar(Cabecalho, "01/10/2026,-10.00,a,Compra - LOJA");

        Assert.Equal(0, resultado.QuantidadeCriados);
        Assert.Equal("Nenhum lançamento novo: 1 já importado(s).", resultado.Mensagem);
        _extratoRepo.Verify(r => r.CreateAsync(It.IsAny<ExtratoBancario>()), Times.Never);
        _itemRepo.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ExtratoBancarioItem>>()), Times.Never);
    }

    // ---------- reparo dos itens truncados (BE-P0-07) ----------

    [Fact]
    public async Task Item_antigo_sem_centavos_e_corrigido_em_vez_de_duplicado()
    {
        var truncado = new ExtratoBancarioItem
        {
            Id = 77,
            Identificador = "a",
            DataMovimentacao = new DateOnly(2026, 9, 5),
            TipoLancamento = "Saída",
            Descricao = "Compra - LOJA",
            Valor = 1234m,
            UsuarioId = UserId,
        };
        _candidatosTruncados.Add(truncado);

        var resultado = await Importar(Cabecalho, "05/09/2026,-1234.56,a,Compra - LOJA");

        var corrigido = Assert.Single(_itensAtualizados);
        Assert.Same(truncado, corrigido);
        Assert.Equal(1234.56m, corrigido.Valor);
        Assert.Equal(
            ExtratoBancarioService.GerarChaveImportacao("a", new DateOnly(2026, 9, 5), -1234.56m, "Compra - LOJA"),
            corrigido.ChaveImportacao);
        Assert.Contains("1 corrigido(s) (centavos)", resultado.Mensagem);
        _itemRepo.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ExtratoBancarioItem>>()), Times.Never);
    }

    [Fact]
    public async Task Candidato_truncado_com_outro_valor_inteiro_nao_e_corrigido()
    {
        _candidatosTruncados.Add(new ExtratoBancarioItem
        {
            Identificador = "a",
            DataMovimentacao = new DateOnly(2026, 9, 5),
            TipoLancamento = "Saída",
            Descricao = "Compra - LOJA",
            Valor = 999m,
        });

        var resultado = await Importar(Cabecalho, "05/09/2026,-1234.56,a,Compra - LOJA");

        Assert.Empty(_itensAtualizados);
        Assert.Equal(1, resultado.QuantidadeCriados);
    }

    // ---------- pessoa da movimentação ----------

    [Fact]
    public async Task Nome_do_extrato_igual_a_uma_pessoa_nao_liga_a_pessoa()
    {
        // Card #79: o nome do extrato não é pessoa; pessoa só por regra, à mão ou o "Eu".
        _pessoasCadastradas.Add(new PessoaMovimentacao
        {
            Id = 15, NomePessoa = "PADARIA", CategoriaId = 4, TipoMovimentacaoId = 9, UsuarioId = UserId,
        });

        await Importar(Cabecalho, "01/10/2026,-12.50,a,Compra no débito - Padaria");

        var item = Assert.Single(_itensCriados);
        Assert.Null(item.PessoaMovimentacaoId);
        Assert.Equal("PADARIA", item.NomePessoaTransacao);
        Assert.Null(item.CategoriaId);
        Assert.Empty(_pessoasCriadas);
    }

    [Fact]
    public async Task Nome_parecido_com_regra_igual_entra_classificado_por_semelhanca()
    {
        _regras.Add(new RegraClassificacao { Id = 3, Texto = "BAR DOIS IRMAOS", Campo = CamposRegra.NomeExtrato, Operador = OperadoresRegra.Igual, CategoriaId = 8, TipoMovimentacaoId = 2, Ativa = true });

        await Importar(Cabecalho, "01/10/2026,-40.00,a,Compra no débito - BARDOISIRMAOS", "02/10/2026,-40.00,b,Compra no débito - BAR DOIS IRMAOS");

        var parecido = _itensCriados.Single(i => i.Identificador == "a");
        Assert.Equal((8, 2, true), (parecido.CategoriaId, parecido.TipoMovimentacaoId, parecido.ClassificadoPorSemelhanca));
        var igual = _itensCriados.Single(i => i.Identificador == "b");
        Assert.Equal((8, false), (igual.CategoriaId, igual.ClassificadoPorSemelhanca));
    }

    [Fact]
    public async Task Entidade_html_do_banco_e_decodificada_sem_mudar_a_chave()
    {
        await Importar(Cabecalho, "01/10/2026,-40.00,a,Compra no débito - AGENCIA Z&AMP;A");

        var item = Assert.Single(_itensCriados);
        Assert.Equal("Compra no débito - AGENCIA Z&A", item.Descricao);
        Assert.Equal("AGENCIA Z&A", item.NomePessoaTransacao);
    }

    [Fact]
    public async Task Nome_novo_fica_so_no_lancamento_sem_cadastrar_pessoa()
    {
        // Card #76, fase 2: estabelecimento (ou nome ainda não cadastrado) não vira pessoa.
        await Importar(Cabecalho, "01/10/2026,-12.50,a,Transferência enviada pelo Pix - Maria Souza - BANCO X");

        Assert.Empty(_pessoasCriadas);
        _pessoaRepo.Verify(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), It.IsAny<int>()), Times.Never);
        var item = Assert.Single(_itensCriados);
        Assert.Null(item.PessoaMovimentacaoId);
        Assert.Equal("MARIA SOUZA", item.NomePessoaTransacao);
        Assert.Null(item.CategoriaId);
        Assert.Null(item.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Descricao_sem_pessoa_gera_item_sem_pessoa()
    {
        await Importar(Cabecalho, "01/10/2026,-500.00,a,Pagamento de fatura");

        var item = Assert.Single(_itensCriados);
        Assert.Null(item.PessoaMovimentacaoId);
        Assert.Null(item.NomePessoaTransacao);
        _pessoaRepo.Verify(r => r.VerificaPossuiPessoa(It.IsAny<string?>(), It.IsAny<int>()), Times.Never);
        _pessoaRepo.Verify(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), It.IsAny<int>()), Times.Never);
    }

    // ---------- chaves ----------

    /// <summary>
    /// A mesma fórmula está no SQL da migration Integridade. Se este valor mudar,
    /// os itens já gravados deixam de ser reconhecidos e a reimportação duplica lançamentos.
    /// </summary>
    [Fact]
    public void GerarChaveImportacao_e_estavel()
    {
        var chave = ExtratoBancarioService.GerarChaveImportacao(
            " abc-123 ", new DateOnly(2026, 10, 1), -1234.56m, " Compra no débito - PADARIA ");

        Assert.Equal("9c4d39b512c5ffd057a0db5295f997afc154981708f5506b95865ed0a871ec92", chave);
    }

    [Fact]
    public void GerarChaveImportacao_diferencia_o_sinal_e_ignora_casas_decimais_extras()
    {
        var data = new DateOnly(2026, 10, 1);

        Assert.NotEqual(
            ExtratoBancarioService.GerarChaveImportacao("abc-123", data, -1234.56m, "x"),
            ExtratoBancarioService.GerarChaveImportacao("abc-123", data, 1234.56m, "x"));
        Assert.Equal(
            ExtratoBancarioService.GerarChaveImportacao("abc-123", data, 10.5m, "x"),
            ExtratoBancarioService.GerarChaveImportacao("abc-123", data, 10.50m, "x"));
    }

    [Theory]
    [InlineData("Compra no débito - Padaria 123", "COMPRA NO DÉBITO - PADARIA")]
    [InlineData("  pix   enviado  ", "PIX ENVIADO")]
    [InlineData("", "")]
    public void GerarChaveDescricao_normaliza(string descricao, string esperado)
    {
        Assert.Equal(esperado, ExtratoBancarioService.GerarChaveDescricao(descricao));
    }

    // ---------- transação (BE-P1-05) ----------

    [Fact]
    public async Task Importacao_grava_correcoes_pessoas_cabecalho_e_itens_na_mesma_operacao()
    {
        _candidatosTruncados.Add(new ExtratoBancarioItem
        {
            Identificador = "a",
            DataMovimentacao = new DateOnly(2026, 9, 5),
            TipoLancamento = "Saída",
            Descricao = "Compra - LOJA",
            Valor = 1234m,
        });

        await Importar(
            Cabecalho,
            "05/09/2026,-1234.56,a,Compra - LOJA",
            "06/09/2026,-20.00,b,Transferência enviada pelo Pix - Maria Souza - BANCO X");

        Assert.Single(_itensAtualizados);
        Assert.Empty(_pessoasCriadas);
        Assert.NotNull(_extratoCriado);
        Assert.NotEmpty(_itensCriados);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Remove_apaga_itens_e_cabecalho_na_mesma_operacao()
    {
        _extratoRepo.Setup(r => r.GetByIdAsync(ExtratoIdGerado, UserId))
            .ReturnsAsync(new ExtratoBancario { Id = ExtratoIdGerado, UsuarioId = UserId });
        _itemRepo.Setup(r => r.RemoveByExtratoIdAsync(ExtratoIdGerado))
            .Callback(() => _uow.Registrar("Itens.Remove"))
            .Returns(Task.CompletedTask);
        _extratoRepo.Setup(r => r.DeleteAsync(ExtratoIdGerado, UserId))
            .Callback(() => _uow.Registrar("Extrato.Remove"))
            .Returns(Task.CompletedTask);
        var service = new ExtratoBancarioService(
            _extratoRepo.Object, _itemRepo.Object, _pessoaRepo.Object, _categoriaRepo.Object, _regrasRepo.Object, MapperFactory.Create(), _bancoService.Object, _uow);

        await service.Remove(ExtratoIdGerado, UserId);

        _itemRepo.Verify(r => r.RemoveByExtratoIdAsync(ExtratoIdGerado), Times.Once);
        _extratoRepo.Verify(r => r.DeleteAsync(ExtratoIdGerado, UserId), Times.Once);   // versão que checa o dono (BE-P2-04)
        Assert.Empty(_uow.Fora);
    }

    // ---------- encargos (card #45, parte 2) ----------

    private readonly PessoaMovimentacao _eu = new() { Id = 1, NomePessoa = "EU", EhTitular = true, UsuarioId = UserId };
    private readonly List<Categoria> _categoriasCriadas = new();

    private void PrepararEncargos(Categoria? existente = null)
    {
        _pessoaRepo.Setup(r => r.ObterTitularAsync(UserId)).ReturnsAsync(_eu);
        _categoriaRepo.Setup(r => r.ObterPorNomeAsync(UserId, ExtratoBancarioService.CategoriaEncargos, "Despesa"))
            .ReturnsAsync(() => existente ?? _categoriasCriadas.FirstOrDefault());
        _categoriaRepo.Setup(r => r.CriarAsync(It.IsAny<Categoria>()))
            .ReturnsAsync((Categoria c) => { c.Id = 50; _categoriasCriadas.Add(c); return c; });
    }

    [Theory]
    [InlineData("IOF de compra internacional")]
    [InlineData("DEBITO IOF - IOF")]
    [InlineData("Juros de dívida encerrada")]
    [InlineData("Anuidade - Parcela 3/12")]
    [InlineData("Encargos de atraso")]
    [InlineData("Tarifa de saque")]
    [InlineData("Multa por atraso")]
    [InlineData("Juros de mora")]
    public void Reconhece_encargo_pela_palavra_inteira(string descricao)
    {
        Assert.True(ExtratoBancarioService.EhEncargo(descricao));
    }

    [Theory]
    [InlineData("Compra no débito - AMORA DOCES")]
    [InlineData("Transferência enviada pelo Pix - MORAES LTDA")]
    [InlineData("Compra - JUROSCARD LOJA")]
    [InlineData("")]
    [InlineData(null)]
    public void Nao_confunde_com_encargo(string? descricao)
    {
        Assert.False(ExtratoBancarioService.EhEncargo(descricao));
    }

    [Fact]
    public async Task Encargo_entra_como_Eu_na_categoria_de_encargos_sem_criar_pessoa()
    {
        PrepararEncargos();

        await Importar(
            Cabecalho,
            "05/10/2026,-3.50,id-1,DEBITO IOF - IOF",
            "06/10/2026,-12.00,id-2,Juros de dívida encerrada",
            "07/10/2026,-20.00,id-3,Compra no débito - AMORA DOCES");

        var encargos = _itensCriados.Where(i => i.Identificador is "id-1" or "id-2").ToList();
        Assert.Equal(2, encargos.Count);
        Assert.All(encargos, i =>
        {
            Assert.Equal(_eu.Id, i.PessoaMovimentacaoId);
            Assert.Equal("EU", i.NomePessoaTransacao);
            Assert.Equal(50, i.CategoriaId);
        });

        // A categoria é criada uma vez (Despesa, Essencial); a compra comum fica só com o nome (fase 2 do #76).
        var categoria = Assert.Single(_categoriasCriadas);
        Assert.Equal(ExtratoBancarioService.CategoriaEncargos, categoria.Nome);
        Assert.Equal("Despesa", categoria.Subcategoria);
        Assert.Equal(1, categoria.NaturezaOperacao);
        Assert.True(categoria.Ativo);
        Assert.Equal(UserId, categoria.UsuarioId);
        Assert.Empty(_pessoasCriadas);
        var compra = _itensCriados.Single(i => i.Identificador == "id-3");
        Assert.Equal(("AMORA DOCES", (int?)null), (compra.NomePessoaTransacao, compra.PessoaMovimentacaoId));
    }

    [Fact]
    public async Task Encargo_reaproveita_a_categoria_que_ja_existe()
    {
        PrepararEncargos(new Categoria { Id = 33, Nome = "Tarifas e encargos", Subcategoria = "Despesa", UsuarioId = UserId });

        await Importar(Cabecalho, "05/10/2026,-3.50,id-1,Tarifa de saque");

        Assert.Equal(33, Assert.Single(_itensCriados).CategoriaId);
        _categoriaRepo.Verify(r => r.CriarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task Sem_encargo_nao_cria_o_Eu_nem_a_categoria()
    {
        PrepararEncargos();

        await Importar(Cabecalho, "07/10/2026,-20.00,id-3,Compra no débito - PADARIA");

        _pessoaRepo.Verify(r => r.ObterTitularAsync(It.IsAny<int>()), Times.Never);
        _categoriaRepo.Verify(r => r.ObterPorNomeAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // ---------- regras de classificação (card #76, fase 1) ----------

    [Fact]
    public async Task Regra_classifica_na_importacao()
    {
        _regras.Add(new RegraClassificacao { Id = 1, Texto = "IFOOD", CategoriaId = 8, Ativa = true });
        _regras.Add(new RegraClassificacao { Id = 2, Texto = "UBER", CategoriaId = 9, TipoMovimentacaoId = 4, Ativa = true });

        await Importar(Cabecalho,
            "01/10/2026,-30.00,a,Compra no débito - IFOOD",
            "02/10/2026,-15.00,b,Uber *Trip",
            "03/10/2026,-5.00,c,Compra no débito - PADARIA");

        var ifood = _itensCriados.Single(i => i.Identificador == "a");
        Assert.Equal((8, (int?)null, (int?)null), (ifood.CategoriaId, ifood.TipoMovimentacaoId, ifood.PessoaMovimentacaoId));
        var uber = _itensCriados.Single(i => i.Identificador == "b");
        Assert.Equal((9, 4), (uber.CategoriaId, uber.TipoMovimentacaoId));
        Assert.Null(_itensCriados.Single(i => i.Identificador == "c").CategoriaId);
        // Uso contado por regra ("aplicada N vezes").
        _regrasRepo.Verify(r => r.RegistrarUsoAsync(UserId, It.Is<IReadOnlyDictionary<int, int>>(u => u.Count == 2 && u[1] == 1 && u[2] == 1)), Times.Once);
    }

    [Fact]
    public async Task Regra_com_pessoa_liga_a_pessoa_sem_criar_outra_pelo_nome()
    {
        _regras.Add(new RegraClassificacao
        {
            Id = 1, Texto = "PIX ENVIADO MARIA", CategoriaId = 8, PessoaMovimentacaoId = 40, Ativa = true,
            PessoaMovimentacao = new PessoaMovimentacao { Id = 40, NomePessoa = "MARIA SILVA" }
        });

        await Importar(Cabecalho, "01/10/2026,-30.00,a,Pix enviado Maria - MARIA S");

        var item = Assert.Single(_itensCriados);
        Assert.Equal((40, "MARIA SILVA", 8), (item.PessoaMovimentacaoId, item.NomePessoaTransacao, item.CategoriaId));
        Assert.Empty(_pessoasCriadas);
    }

    [Fact]
    public async Task Encargo_vence_a_regra()
    {
        PrepararEncargos();
        _regras.Add(new RegraClassificacao { Id = 1, Texto = "IOF", CategoriaId = 99, Ativa = true });

        await Importar(Cabecalho, "05/10/2026,-3.50,id-1,IOF de compra internacional");

        Assert.Equal(50, Assert.Single(_itensCriados).CategoriaId);
    }

    // ---------- listagem (BE-P4-05) ----------

    [Fact]
    public async Task Listagem_le_os_bancos_uma_vez_e_preenche_o_nome_sem_mexer_na_entidade()
    {
        var extratos = new List<ExtratoBancario>
        {
            new() { Id = 1, BancoId = BancoId, UsuarioId = UserId },
            new() { Id = 2, BancoId = 77, UsuarioId = UserId },
            new() { Id = 3, BancoId = BancoId, UsuarioId = UserId },
        };
        _extratoRepo.Setup(r => r.GetByUserIdAsync(UserId, "2026-10")).ReturnsAsync(extratos);
        _bancoService.Setup(s => s.ListarAsync(UserId))
            .ReturnsAsync(new[] { new BancoDTO { Id = BancoId, NomeBanco = "Nubank" } });
        var service = new ExtratoBancarioService(
            _extratoRepo.Object, _itemRepo.Object, _pessoaRepo.Object, _categoriaRepo.Object, _regrasRepo.Object, MapperFactory.Create(), _bancoService.Object, _uow);

        var lista = (await service.GetExtratoBancario(UserId, "2026-10")).ToList();

        Assert.Equal(new[] { "Nubank", "Banco não encontrado", "Nubank" }, lista.Select(e => e.BancoNome));
        _bancoService.Verify(s => s.ListarAsync(UserId), Times.Once);
        _bancoService.Verify(s => s.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        Assert.All(extratos, e => Assert.Null(e.BancoNome));
    }
}
