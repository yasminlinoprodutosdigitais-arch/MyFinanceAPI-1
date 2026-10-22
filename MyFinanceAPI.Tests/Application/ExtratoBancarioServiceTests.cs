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
    private readonly Mock<IBancoService> _bancoService = new();
    private readonly UnitOfWorkFalso _uow = new();

    private readonly HashSet<string> _chavesExistentes = new();
    private readonly List<ExtratoBancarioItem> _candidatosTruncados = new();
    private readonly List<PessoaMovimentacao> _pessoasCadastradas = new();

    private ExtratoBancario? _extratoCriado;
    private List<ExtratoBancarioItem> _itensCriados = new();
    private readonly List<ExtratoBancarioItem> _itensAtualizados = new();
    private readonly List<PessoaMovimentacao> _pessoasCriadas = new();

    public ExtratoBancarioServiceTests()
    {
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

    private Task<ExtratoImportacaoResultadoDTO> Importar(params string[] linhas)
    {
        var csv = string.Join("\n", linhas);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var service = new ExtratoBancarioService(
            _extratoRepo.Object, _itemRepo.Object, _pessoaRepo.Object, MapperFactory.Create(), _bancoService.Object, _uow);
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
    public async Task Pessoa_ja_cadastrada_e_reaproveitada_com_categoria_e_tipo()
    {
        _pessoasCadastradas.Add(new PessoaMovimentacao
        {
            Id = 15, NomePessoa = "PADARIA", CategoriaId = 4, TipoMovimentacaoId = 9, UsuarioId = UserId,
        });

        await Importar(Cabecalho, "01/10/2026,-12.50,a,Compra no débito - Padaria");

        var item = Assert.Single(_itensCriados);
        Assert.Equal(15, item.PessoaMovimentacaoId);
        Assert.Equal("PADARIA", item.NomePessoaTransacao);
        Assert.Equal(4, item.CategoriaId);
        Assert.Equal(9, item.TipoMovimentacaoId);
        Assert.Empty(_pessoasCriadas);
    }

    [Fact]
    public async Task Pessoa_nova_e_cadastrada_em_maiusculas_sem_categoria()
    {
        await Importar(Cabecalho, "01/10/2026,-12.50,a,Transferência enviada pelo Pix - Maria Souza - BANCO X");

        var nova = Assert.Single(_pessoasCriadas);
        Assert.Equal("MARIA SOUZA", nova.NomePessoa);
        var item = Assert.Single(_itensCriados);
        Assert.Equal(nova.Id, item.PessoaMovimentacaoId);
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
        Assert.Single(_pessoasCriadas);
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
            _extratoRepo.Object, _itemRepo.Object, _pessoaRepo.Object, MapperFactory.Create(), _bancoService.Object, _uow);

        await service.Remove(ExtratoIdGerado, UserId);

        _itemRepo.Verify(r => r.RemoveByExtratoIdAsync(ExtratoIdGerado), Times.Once);
        _extratoRepo.Verify(r => r.DeleteAsync(ExtratoIdGerado, UserId), Times.Once);   // versão que checa o dono (BE-P2-04)
        Assert.Empty(_uow.Fora);
    }
}
