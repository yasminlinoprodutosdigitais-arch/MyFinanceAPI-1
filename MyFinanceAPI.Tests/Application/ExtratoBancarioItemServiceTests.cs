using Moq;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Criação de item de extrato (lançamento manual): a categoria e o tipo escolhidos no formulário
/// prevalecem sobre o vínculo da pessoa; sem escolha, o item herda o vínculo (decisão de 14/10/2026).
/// Antes a escolha era descartada quando havia pessoa.
/// </summary>
public class ExtratoBancarioItemServiceTests
{
    private const int UserId = 7;
    private const int Mercado = 25;
    private const int Outros = 10;
    private const int Lanche = 27;

    private readonly Mock<IExtratoBancarioItemRepository> _itemRepo = new();
    private readonly Mock<IPessoaMovimentacaoRepository> _pessoaRepo = new();
    private readonly Mock<IRegraClassificacaoRepository> _regrasRepo = new();
    private readonly List<RegraClassificacao> _regras = new();
    private readonly List<PessoaMovimentacao> _pessoasCadastradas = new();
    private readonly List<PessoaMovimentacao> _pessoasCriadas = new();
    private ExtratoBancarioItem? _itemCriado;

    public ExtratoBancarioItemServiceTests()
    {
        _itemRepo.Setup(r => r.CreateAsync(It.IsAny<ExtratoBancarioItem>()))
            .Callback<ExtratoBancarioItem>(i => _itemCriado = i)
            .ReturnsAsync((ExtratoBancarioItem i) => i);
        _regrasRepo.Setup(r => r.ListarAtivasAsync(UserId)).ReturnsAsync(() => _regras);
        _pessoaRepo.Setup(r => r.VerificaPossuiPessoa(It.IsAny<string?>(), UserId))
            .Returns((string? nome, int _) => Task.FromResult<IEnumerable<PessoaMovimentacao>>(
                _pessoasCadastradas.Where(p => p.NomePessoa == nome).ToList()));
        _pessoaRepo.Setup(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), UserId))
            .ReturnsAsync((PessoaMovimentacao p, int _) =>
            {
                p.Id = 900;
                _pessoasCriadas.Add(p);
                return p;
            });
    }

    private Task Criar(ExtratoBancarioItemDTO dto) =>
        new ExtratoBancarioItemService(_itemRepo.Object, _pessoaRepo.Object, _regrasRepo.Object, MapperFactory.Create()).AddAsync(dto, UserId);

    private static ExtratoBancarioItemDTO Lancamento(string? pessoa, int? categoriaId) => new()
    {
        DataMovimentacao = new DateOnly(2026, 12, 1),
        Valor = 12.34m,
        TipoLancamento = "Saída",
        BancoId = 5,
        TipoCartaoId = 2,
        NomePessoaTransacao = pessoa,
        CategoriaId = categoriaId
    };

    private void PessoaComVinculo(string nome, int? categoriaId, int? tipoId) =>
        _pessoasCadastradas.Add(new PessoaMovimentacao { Id = 88, NomePessoa = nome, CategoriaId = categoriaId, TipoMovimentacaoId = tipoId });

    [Fact]
    public async Task Nome_digitado_igual_a_uma_pessoa_nao_liga_nem_herda_o_padrao()
    {
        // Card #79: o nome é o do estabelecimento; a pessoa só vem pelo id.
        PessoaComVinculo("CARLOS", Mercado, Lanche);

        await Criar(Lancamento("CARLOS", null));

        Assert.Null(_itemCriado!.PessoaMovimentacaoId);
        Assert.Equal("CARLOS", _itemCriado.NomePessoaTransacao);
        Assert.Null(_itemCriado.CategoriaId);
        Assert.Null(_itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Pessoa_pelo_id_liga_e_mantem_o_nome_e_a_categoria_escolhida()
    {
        _pessoaRepo.Setup(r => r.ObterPorIdAsync(88, UserId)).ReturnsAsync(new PessoaMovimentacao { Id = 88, NomePessoa = "CARLOS" });
        var dto = Lancamento("CIRCUITO", Outros);
        dto.PessoaMovimentacaoId = 88;

        await Criar(dto);

        Assert.Equal(88, _itemCriado!.PessoaMovimentacaoId);
        Assert.Equal("CIRCUITO", _itemCriado.NomePessoaTransacao);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
    }

    [Fact]
    public async Task Pessoa_de_outro_usuario_e_recusada()
    {
        var dto = Lancamento("CIRCUITO", Outros);
        dto.PessoaMovimentacaoId = 99;

        await Assert.ThrowsAsync<RegraNegocioException>(() => Criar(dto));
    }

    [Fact]
    public async Task Manual_com_nome_parecido_com_regra_igual_classifica_por_semelhanca()
    {
        _regras.Add(new RegraClassificacao { Id = 1, Texto = "BAR DOIS IRMAOS", Campo = CamposRegra.NomeExtrato, Operador = OperadoresRegra.Igual, CategoriaId = Lanche, Ativa = true });

        await Criar(Lancamento("BARDOISIRMAOS", null));

        Assert.Equal(Lanche, _itemCriado!.CategoriaId);
        Assert.True(_itemCriado.ClassificadoPorSemelhanca);
    }

    [Fact]
    public async Task Nome_novo_com_categoria_escolhida_fica_so_no_lancamento()
    {
        // Card #76, fase 2: o nome digitado que não é pessoa cadastrada fica só no lançamento.
        await Criar(Lancamento("PADARIA NOVA", Outros));

        Assert.Null(_itemCriado!.PessoaMovimentacaoId);
        Assert.Equal("PADARIA NOVA", _itemCriado.NomePessoaTransacao);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
        Assert.Empty(_pessoasCriadas);
    }

    [Fact]
    public async Task Pessoa_nova_sem_categoria_fica_sem_categoria()
    {
        await Criar(Lancamento("PADARIA NOVA", null));

        Assert.Null(_itemCriado!.PessoaMovimentacaoId);
        Assert.Null(_itemCriado.CategoriaId);
        Assert.Null(_itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Pessoa_enviada_como_objeto_liga_pelo_id_sem_herdar_o_padrao()
    {
        _pessoaRepo.Setup(r => r.ObterPorIdAsync(88, UserId)).ReturnsAsync(new PessoaMovimentacao { Id = 88, NomePessoa = "CARLOS" });
        var dto = Lancamento(null, Outros);
        dto.PessoaMovimentacao = new PessoaMovimentacaoDTO { Id = 88, NomePessoa = "CARLOS", CategoriaId = Mercado, TipoMovimentacaoId = Lanche };

        await Criar(dto);

        Assert.Equal(88, _itemCriado!.PessoaMovimentacaoId);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
        Assert.Null(_itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Sem_pessoa_mantem_a_categoria_escolhida()
    {
        await Criar(Lancamento(null, Outros));

        Assert.Null(_itemCriado!.PessoaMovimentacaoId);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
    }

    [Fact]
    public async Task O_vinculo_existente_nao_e_alterado()
    {
        PessoaComVinculo("DALLE PIZZA CIC", Mercado, Lanche);

        await Criar(Lancamento("DALLE PIZZA CIC", Outros));

        Assert.Equal(Mercado, _pessoasCadastradas[0].CategoriaId);
        _pessoaRepo.Verify(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), It.IsAny<int>()), Times.Never);
    }

    // ---------- regras no lançamento manual (card #76, fase 1) ----------

    [Fact]
    public async Task Manual_sem_categoria_escolhida_usa_a_regra()
    {
        _regras.Add(new RegraClassificacao { Id = 1, Texto = "PIZZA", CategoriaId = Outros, TipoMovimentacaoId = Lanche, Ativa = true });

        await Criar(Lancamento("DALLE PIZZA CIC", null));

        Assert.Equal((Outros, Lanche), (_itemCriado!.CategoriaId, _itemCriado.TipoMovimentacaoId));
        Assert.False(_itemCriado.ClassificadoPorSemelhanca);
    }

    [Fact]
    public async Task Manual_com_categoria_escolhida_ignora_a_regra()
    {
        _regras.Add(new RegraClassificacao { Id = 1, Texto = "PIZZA", CategoriaId = Outros, Ativa = true });

        await Criar(Lancamento("Pizza do bairro", Mercado));

        Assert.Equal(Mercado, _itemCriado!.CategoriaId);
    }

    // ---------- edição: trocar a pessoa (card #45, parte 2) ----------

    private readonly ExtratoBancarioItem _existente = new()
    {
        Id = 5, UsuarioId = UserId, DataMovimentacao = new DateOnly(2026, 12, 1), Valor = 30, TipoLancamento = "Saída",
        PessoaMovimentacaoId = null, NomePessoaTransacao = "PADARIA"
    };

    private Task Editar(string? pessoa, int? pessoaId, int? categoriaId)
    {
        _itemRepo.Setup(r => r.GetByIdAsync(_existente.Id)).ReturnsAsync(_existente);
        _pessoaRepo.Setup(r => r.ObterPorIdAsync(88, UserId))
            .ReturnsAsync(new PessoaMovimentacao { Id = 88, NomePessoa = "CARLOS", CategoriaId = Mercado, TipoMovimentacaoId = Lanche });
        var dto = Lancamento(pessoa, categoriaId);
        dto.Id = _existente.Id;
        dto.PessoaMovimentacaoId = pessoaId;
        dto.AlteraVinculoPessoa = true;
        return new ExtratoBancarioItemService(_itemRepo.Object, _pessoaRepo.Object, _regrasRepo.Object, MapperFactory.Create()).UpdateAsync(dto, UserId);
    }

    [Fact]
    public async Task Editar_liga_a_pessoa_cadastrada_pelo_id_sem_mexer_no_padrao_dela()
    {
        await Editar("CARLOS", 88, Outros);

        Assert.Equal(88, _existente.PessoaMovimentacaoId);
        Assert.Equal(Outros, _existente.CategoriaId);
        _pessoaRepo.Verify(r => r.AtualizarAsync(It.IsAny<PessoaMovimentacao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Editar_com_nome_novo_fica_so_no_lancamento()
    {
        await Editar("Ágatha", null, Outros);

        Assert.Empty(_pessoasCriadas);
        Assert.Null(_existente.PessoaMovimentacaoId);
        Assert.Equal("Ágatha", _existente.NomePessoaTransacao);
        Assert.Equal(Outros, _existente.CategoriaId);
    }

    [Fact]
    public async Task Editar_com_nome_de_pessoa_cadastrada_sem_id_nao_liga()
    {
        // Card #79: pessoa só pelo id.
        PessoaComVinculo("GIOVANNE", Mercado, Lanche);

        await Editar("GIOVANNE", null, Outros);

        Assert.Null(_existente.PessoaMovimentacaoId);
        Assert.Empty(_pessoasCriadas);
    }

    [Fact]
    public async Task Editar_a_pessoa_de_um_importado_nao_muda_o_nome_do_extrato()
    {
        _existente.ChaveImportacao = "abc";
        _existente.NomePessoaTransacao = "CIRCUITO";

        await Editar("CARLOS", 88, Outros);

        Assert.Equal(88, _existente.PessoaMovimentacaoId);
        Assert.Equal("CIRCUITO", _existente.NomePessoaTransacao);
    }

    [Fact]
    public async Task Trocar_a_categoria_tira_o_por_semelhanca()
    {
        _existente.CategoriaId = Mercado;
        _existente.ClassificadoPorSemelhanca = true;

        await Editar(null, null, Outros);

        Assert.False(_existente.ClassificadoPorSemelhanca);
    }

    // ---------- pagamento que abate uma compra (card #45, parte 3) ----------

    private ExtratoBancarioItemDTO Pagamento(int? abateItemId, int pessoaId = 88)
    {
        _pessoaRepo.Setup(r => r.ObterPorIdAsync(88, UserId))
            .ReturnsAsync(new PessoaMovimentacao { Id = 88, NomePessoa = "CARLOS" });
        return new ExtratoBancarioItemDTO
        {
            DataMovimentacao = new DateOnly(2026, 12, 10), Valor = 50, TipoLancamento = TipoLancamentoExtrato.Entrada, BancoId = 5,
            PessoaMovimentacao = new PessoaMovimentacaoDTO { Id = pessoaId, NomePessoa = "CARLOS" },
            AbateItemId = abateItemId
        };
    }

    private void Compra(int id, string tipo = TipoLancamentoExtrato.Saida, int? pessoaId = 88, int usuarioId = UserId) =>
        _itemRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new ExtratoBancarioItem
        {
            Id = id, UsuarioId = usuarioId, TipoLancamento = tipo, PessoaMovimentacaoId = pessoaId, Valor = 120,
            DataMovimentacao = new DateOnly(2026, 11, 3)
        });

    [Fact]
    public async Task Pagamento_grava_a_compra_que_abate()
    {
        Compra(40);

        await Criar(Pagamento(40));

        Assert.Equal(40, _itemCriado!.AbateItemId);
    }

    [Fact]
    public async Task Pagamento_sem_compra_abate_o_saldo_geral()
    {
        await Criar(Pagamento(null));

        Assert.Null(_itemCriado!.AbateItemId);
    }

    [Theory]
    [InlineData("entrada")]      // a "compra" é uma entrada
    [InlineData("outra-pessoa")] // compra de outra pessoa
    [InlineData("outro-usuario")]
    [InlineData("inexistente")]
    public async Task Pagamento_recusa_compra_que_nao_e_saida_da_mesma_pessoa(string caso)
    {
        switch (caso)
        {
            case "entrada": Compra(40, tipo: TipoLancamentoExtrato.Entrada); break;
            case "outra-pessoa": Compra(40, pessoaId: 99); break;
            case "outro-usuario": Compra(40, usuarioId: 1); break;
        }

        await Assert.ThrowsAsync<RegraNegocioException>(() => Criar(Pagamento(40)));
        _itemRepo.Verify(r => r.CreateAsync(It.IsAny<ExtratoBancarioItem>()), Times.Never);
    }

    [Fact]
    public async Task So_entrada_abate_compra()
    {
        Compra(40);
        var saida = Pagamento(40);
        saida.TipoLancamento = TipoLancamentoExtrato.Saida;

        await Assert.ThrowsAsync<RegraNegocioException>(() => Criar(saida));
    }

    [Fact]
    public async Task Editar_tambem_valida_e_grava_a_compra_abatida()
    {
        Compra(40);
        _itemRepo.Setup(r => r.GetByIdAsync(_existente.Id)).ReturnsAsync(_existente);
        _existente.TipoLancamento = TipoLancamentoExtrato.Entrada;
        _existente.PessoaMovimentacaoId = 88;
        var dto = Pagamento(40);
        dto.Id = _existente.Id;
        dto.PessoaMovimentacaoId = 88;

        await new ExtratoBancarioItemService(_itemRepo.Object, _pessoaRepo.Object, _regrasRepo.Object, MapperFactory.Create()).UpdateAsync(dto, UserId);

        Assert.Equal(40, _existente.AbateItemId);
    }

    [Fact]
    public async Task Editar_sem_id_tira_a_pessoa()
    {
        _existente.PessoaMovimentacaoId = 88;

        await Editar("PADARIA", null, Outros);

        Assert.Null(_existente.PessoaMovimentacaoId);
    }
}
