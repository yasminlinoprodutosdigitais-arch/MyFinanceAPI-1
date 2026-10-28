using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Testes de caracterização do TransactionService (BE-P1-09): fixam o comportamento ATUAL
/// da criação de transações e da máquina de estados de parcelas (UpdateParcelaAtual).
/// Se um teste quebrar depois de uma mudança, a mudança de regra precisa ser intencional.
/// </summary>
public class TransacaoServiceTests
{
    private const int UserId = 7;
    private const int AccountId = 100;
    private const int TransactionId = 500;

    private readonly Mock<ITransacaoRepository> _repo = new();
    private readonly Mock<IContaService> _accountService = new();
    private readonly List<ContaDTO> _parcelasAtualizadas = new();
    private readonly UnitOfWorkFalso _uow = new();

    private TransacaoService CriarService()
    {
        _accountService
            .Setup(s => s.UpdateParcela(It.IsAny<ContaDTO>(), UserId))
            .Callback<ContaDTO, int>((conta, _) =>
            {
                _uow.Registrar("UpdateParcela");
                _parcelasAtualizadas.Add(conta);
            })
            .Returns(Task.CompletedTask);

        return new TransacaoService(_repo.Object, MapperFactory.Create(), _accountService.Object, _uow);
    }

    private ContaDTO ContaParcelada(int parcelaAtual, int quantidadeParcelas, bool ativo = true)
    {
        var conta = new ContaDTO
        {
            Id = AccountId,
            Nome = "Notebook",
            Valor = 3000m,
            EhParcelado = true,
            ParcelaAtual = parcelaAtual,
            QuantidadeParcelas = quantidadeParcelas,
            Ativo = ativo,
            DataOperacao = new List<int> { 10 },
        };
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(conta);
        return conta;
    }

    private void TransacaoGravada(string status, int parcelaAtual)
    {
        _repo.Setup(r => r.GetTransactionById(TransactionId, UserId)).ReturnsAsync(new Transacao
        {
            Id = TransactionId,
            ContaId = AccountId,
            Nome = "Notebook",
            Valor = 300m,
            Situacao = status,
            EhParcelado = true,
            ParcelaAtual = parcelaAtual,
            QuantidadeParcelas = 10,
            UsuarioId = UserId,
            Data = new DateOnly(2026, 10, 10),
        });
    }

    /// <summary>Parcelas pagas da conta, sem contar a transação editada (BE-P4-19).</summary>
    private void ParcelasPagas(params int[] parcelas) =>
        _repo.Setup(r => r.GetParcelasPagas(AccountId, UserId, TransactionId)).ReturnsAsync(parcelas.ToList());

    private static int[] Ate(int ultima) => Enumerable.Range(1, ultima).ToArray();

    private static TransacaoDTO Edicao(string novoStatus, int parcelaAtual = 3, bool ehParcelado = true) => new()
    {
        Id = TransactionId,
        ContaId = AccountId,
        Nome = "Notebook",
        Valor = 300m,
        Situacao = novoStatus,
        EhParcelado = ehParcelado,
        ParcelaAtual = parcelaAtual,
        QuantidadeParcelas = 10,
        Data = new DateOnly(2026, 10, 10),
    };

    // ---------- Add ----------

    [Fact]
    public async Task Add_divide_o_valor_da_conta_entre_os_vencimentos_e_limita_o_dia_ao_fim_do_mes()
    {
        var conta = new ContaDTO
        {
            Id = AccountId,
            Valor = 300m,
            CategoriaId = 4,
            DataOperacao = new List<int> { 5, 31 },
            Vencimentos = new List<ContaVencimento>
            {
                new() { ContaId = AccountId, Dia = 5 },
                new() { ContaId = AccountId, Dia = 31 },
            },
        };
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(conta);
        List<Transacao>? criadas = null;
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => criadas = t)
            .Returns(Task.CompletedTask);

        await CriarService().Add(new TransacaoDTO
        {
            ContaId = AccountId,
            Nome = "Aluguel",
            Situacao = "PENDENTE",
            Data = new DateOnly(2026, 2, 1),
        }, UserId);

        Assert.NotNull(criadas);
        Assert.Equal(2, criadas!.Count);
        Assert.All(criadas, t =>
        {
            Assert.Equal(150m, t.Valor);
            Assert.Equal(AccountId, t.ContaId);
            Assert.Equal(4, t.CategoriaId);
            Assert.Equal(UserId, t.UsuarioId);
        });
        Assert.Equal(new DateOnly(2026, 2, 5), criadas[0].Data);
        Assert.Equal(new DateOnly(2026, 2, 28), criadas[1].Data); // dia 31 em fevereiro → último dia
    }

    // BE-P4-01: os dois casos davam 500 (InvalidOperationException do (int) e DivideByZeroException).

    [Fact]
    public async Task Add_com_conta_nova_sem_categoria_e_recusado_sem_criar_a_conta()
    {
        _accountService.Setup(s => s.GetAccountById(0, UserId)).ReturnsAsync((ContaDTO)null!);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(new TransacaoDTO
        {
            Nome = "Farmácia",
            Valor = 50m,
            Situacao = "PENDENTE",
            Data = new DateOnly(2026, 10, 5),
        }, UserId));

        Assert.Contains("categoria", ex.Message, StringComparison.OrdinalIgnoreCase);
        _accountService.Verify(s => s.Add(It.IsAny<ContaDTO>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Add_com_conta_sem_vencimentos_e_recusado()
    {
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(new ContaDTO
        {
            Id = AccountId,
            Valor = 300m,
            CategoriaId = 4,
            Vencimentos = new List<ContaVencimento>(),
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(Lancamento(), UserId));

        Assert.Contains("vencimento", ex.Message, StringComparison.OrdinalIgnoreCase);
        _repo.Verify(r => r.Create(It.IsAny<List<Transacao>>()), Times.Never);
    }

    [Fact]
    public async Task Add_devolve_as_transacoes_criadas_uma_por_vencimento()
    {
        // Uma por vencimento da conta; o 201 leva todas, com o Location da primeira (BE-P3-05).
        ContaParceladaParaLancar(parcelaAtual: 1, quantidadeParcelas: 10, [], vencimentos: 2);
        var proximoId = 900;
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => t.ForEach(x => x.Id = proximoId++))
            .Returns(Task.CompletedTask);

        var criadas = await CriarService().Add(Lancamento(), UserId);

        Assert.Equal([900, 901], criadas.Select(t => t.Id));
        Assert.All(criadas, t => Assert.Equal(AccountId, t.ContaId));
    }

    [Fact]
    public async Task Add_devolve_transacoes_que_viram_JSON_mesmo_com_a_conta_rastreada()
    {
        // Na gravação o EF liga a conta rastreada às transações novas (Account.Transactions ↔
        // Transaction.Account). Devolvidas assim, o JSON do 201 entrava em ciclo — com a transação
        // já gravada (achado na validação do BE-P3, 17/10/2026).
        ContaParceladaParaLancar(parcelaAtual: 1, quantidadeParcelas: 10, []);
        var contaRastreada = new Conta { Id = AccountId, Nome = "Notebook", Transacoes = new List<Transacao>() };
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => t.ForEach(x =>
            {
                x.Id = 900;
                x.Conta = contaRastreada;
                contaRastreada.Transacoes.Add(x);
            }))
            .Returns(Task.CompletedTask);

        var criadas = await CriarService().Add(Lancamento(), UserId);

        var json = System.Text.Json.JsonSerializer.Serialize(criadas, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
        Assert.Contains("\"id\":900", json);
    }

    // ---------- Add: valor e dias só deste mês, pelo lote (card #48) ----------

    private void ContaDoLote(params int[] dias)
    {
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(new ContaDTO
        {
            Id = AccountId,
            Valor = 300m,
            CategoriaId = 4,
            DataOperacao = dias.ToList(),
            Vencimentos = dias.Select(d => new ContaVencimento { ContaId = AccountId, Dia = d }).ToList(),
        });
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => _criadas = t)
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Lote_com_valor_do_mes_divide_ele_entre_os_vencimentos_e_nao_mexe_na_conta()
    {
        ContaDoLote(5, 20);
        var lancamento = Lancamento();
        lancamento.ValorDoMes = 410m;

        await CriarService().Add(lancamento, UserId);

        Assert.All(_criadas!, t => Assert.Equal(205m, t.Valor));
        _accountService.Verify(s => s.Update(It.IsAny<ContaDTO>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Lote_com_dias_usa_os_dias_pedidos_no_lugar_dos_da_conta()
    {
        ContaDoLote(5);
        var lancamento = Lancamento();
        lancamento.DiasVencimento = [25, 12];

        await CriarService().Add(lancamento, UserId);

        Assert.Equal([new DateOnly(2026, 11, 12), new DateOnly(2026, 11, 25)], _criadas!.Select(t => t.Data));
        Assert.All(_criadas!, t => Assert.Equal(150m, t.Valor)); // valor da conta dividido entre os dias pedidos
    }

    [Fact]
    public async Task Lote_com_dias_aceita_conta_sem_vencimento_cadastrado()
    {
        ContaDoLote();
        var lancamento = Lancamento();
        lancamento.DiasVencimento = [10];

        await CriarService().Add(lancamento, UserId);

        var criada = Assert.Single(_criadas!);
        Assert.Equal(new DateOnly(2026, 11, 10), criada.Data);
        Assert.Equal(300m, criada.Valor);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Lote_com_valor_do_mes_zero_ou_negativo_e_recusado(int valor)
    {
        ContaDoLote(5);
        var lancamento = Lancamento();
        lancamento.ValorDoMes = valor;

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(lancamento, UserId));

        Assert.Contains("valor", ex.Message, StringComparison.OrdinalIgnoreCase);
        _repo.Verify(r => r.Create(It.IsAny<List<Transacao>>()), Times.Never);
    }

    [Fact]
    public async Task Lote_com_dia_repetido_e_recusado()
    {
        ContaDoLote(5);
        var lancamento = Lancamento();
        lancamento.DiasVencimento = [7, 7];

        await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(lancamento, UserId));
        _repo.Verify(r => r.Create(It.IsAny<List<Transacao>>()), Times.Never);
    }

    // ---------- Add: lançamento avulso, sem cadastrar a conta (card #47) ----------

    private static TransacaoDTO Avulso(List<int>? dias = null) => new()
    {
        Nome = "Presente da Ágatha",
        Valor = 300m,
        CategoriaId = 4,
        Situacao = "PENDENTE",
        Data = new DateOnly(2026, 11, 1),
        DiasVencimento = dias ?? [10, 20],
        CadastrarConta = false,
    };

    [Fact]
    public async Task Avulso_nao_cria_conta_e_grava_um_lancamento_por_dia_com_o_valor_dividido()
    {
        List<Transacao>? criadas = null;
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => criadas = t)
            .Returns(Task.CompletedTask);

        var devolvidas = await CriarService().Add(Avulso(), UserId);

        _accountService.Verify(s => s.Add(It.IsAny<ContaDTO>(), It.IsAny<int>()), Times.Never);
        Assert.Equal(2, criadas!.Count);
        Assert.All(criadas, t =>
        {
            Assert.Null(t.ContaId);
            Assert.Equal(150m, t.Valor);
            Assert.Equal(4, t.CategoriaId);
            Assert.Equal(UserId, t.UsuarioId);
            Assert.Equal("PENDENTE", t.Situacao);
            Assert.False(t.EhParcelado);
            Assert.Null(t.ParcelaAtual);
        });
        Assert.Equal([new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 20)], criadas.Select(t => t.Data));
        Assert.Equal(2, devolvidas.Count);
    }

    [Fact]
    public async Task Avulso_no_dia_31_de_um_mes_curto_cai_no_ultimo_dia()
    {
        List<Transacao>? criadas = null;
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => criadas = t)
            .Returns(Task.CompletedTask);
        var dto = Avulso([31]);
        dto.Data = new DateOnly(2026, 2, 1);

        await CriarService().Add(dto, UserId);

        Assert.Equal(new DateOnly(2026, 2, 28), Assert.Single(criadas!).Data);
    }

    [Fact]
    public async Task Avulso_sem_categoria_e_recusado()
    {
        var dto = Avulso();
        dto.CategoriaId = null;

        await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(dto, UserId));

        _repo.Verify(r => r.Create(It.IsAny<List<Transacao>>()), Times.Never);
    }

    // ---------- Add: número da parcela ----------

    private List<Transacao>? _criadas;

    private void ContaParceladaParaLancar(int parcelaAtual, int quantidadeParcelas, int[] lancadas, int vencimentos = 1)
    {
        var conta = new ContaDTO
        {
            Id = AccountId,
            Valor = 300m,
            CategoriaId = 4,
            EhParcelado = true,
            ParcelaAtual = parcelaAtual,
            QuantidadeParcelas = quantidadeParcelas,
            Vencimentos = Enumerable.Range(1, vencimentos)
                .Select(i => new ContaVencimento { ContaId = AccountId, Dia = 10 + i })
                .ToList(),
        };
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(conta);
        _repo.Setup(r => r.GetParcelasLancadas(AccountId, UserId)).ReturnsAsync(lancadas.ToList());
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => _criadas = t)
            .Returns(Task.CompletedTask);
    }

    /// <summary>O payload do front é ignorado: o número sai da conta e do que já foi lançado.</summary>
    private static TransacaoDTO Lancamento() => new()
    {
        ContaId = AccountId,
        Nome = "Notebook",
        Situacao = "PENDENTE",
        EhParcelado = false,
        ParcelaAtual = 99,
        QuantidadeParcelas = null,
        Data = new DateOnly(2026, 11, 1),
    };

    [Theory]
    [InlineData(4, new[] { 1, 2, 3 }, 4)]        // normal: as anteriores pagas
    [InlineData(4, new[] { 1, 2, 3, 4 }, 5)]     // dois meses seguidos sem pagar
    [InlineData(4, new[] { 1, 2, 3, 5 }, 4)]     // pendente 4 excluída com a 5 lançada: preenche o buraco
    [InlineData(3, new[] { 1, 2 }, 3)]           // paga 3 excluída (a conta voltou para 3)
    [InlineData(1, new int[0], 1)]               // primeira parcela
    public async Task Add_parcelado_usa_a_menor_parcela_ainda_nao_lancada_a_partir_da_conta(
        int parcelaDaConta, int[] lancadas, int esperada)
    {
        ContaParceladaParaLancar(parcelaDaConta, quantidadeParcelas: 10, lancadas);

        await CriarService().Add(Lancamento(), UserId);

        var criada = Assert.Single(_criadas!);
        Assert.Equal(esperada, criada.ParcelaAtual);
        Assert.True(criada.EhParcelado);
        Assert.Equal(10, criada.QuantidadeParcelas);
    }

    [Fact]
    public async Task Add_parcelado_com_todas_as_parcelas_lancadas_e_recusado()
    {
        ContaParceladaParaLancar(parcelaAtual: 10, quantidadeParcelas: 10, Enumerable.Range(1, 10).ToArray());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(Lancamento(), UserId));

        Assert.Equal("Todas as parcelas desta conta já foram lançadas.", ex.Message);
        _repo.Verify(r => r.Create(It.IsAny<List<Transacao>>()), Times.Never);
    }

    [Fact]
    public async Task Add_parcelado_com_varios_vencimentos_usa_o_mesmo_numero_em_todos()
    {
        ContaParceladaParaLancar(parcelaAtual: 4, quantidadeParcelas: 10, new[] { 1, 2, 3 }, vencimentos: 2);

        await CriarService().Add(Lancamento(), UserId);

        Assert.Equal(2, _criadas!.Count);
        Assert.All(_criadas, t => Assert.Equal(4, t.ParcelaAtual));
    }

    [Fact]
    public async Task Add_nao_parcelado_nao_consulta_parcelas_e_grava_sem_numero()
    {
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(new ContaDTO
        {
            Id = AccountId,
            Valor = 100m,
            EhParcelado = false,
            Vencimentos = new List<ContaVencimento> { new() { ContaId = AccountId, Dia = 5 } },
        });
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => _criadas = t)
            .Returns(Task.CompletedTask);

        await CriarService().Add(Lancamento(), UserId);

        var criada = Assert.Single(_criadas!);
        Assert.False(criada.EhParcelado);
        Assert.Null(criada.ParcelaAtual);
        Assert.Null(criada.QuantidadeParcelas);
        _repo.Verify(r => r.GetParcelasLancadas(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ---------- Update: pagar parcela ----------

    [Theory]
    [InlineData("PENDENTE", "PAGO NO PRAZO")]
    [InlineData("PENDENTE", "PAGO ATRASADO")]
    [InlineData("AGUARDANDO", "PAGO NO PRAZO")]
    [InlineData("AGUARDANDO", "PAGO ATRASADO")]
    public async Task Update_pagar_parcela_avanca_a_parcela_atual_da_conta(string statusAnterior, string novoStatus)
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada(statusAnterior, parcelaAtual: 3);
        ParcelasPagas(1, 2);

        await CriarService().Update(Edicao(novoStatus), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(4, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
        _repo.Verify(r => r.Update(It.Is<Transacao>(t => t.Id == TransactionId && t.Situacao == novoStatus), UserId), Times.Once);
    }

    [Fact]
    public async Task Update_pagar_a_ultima_parcela_inativa_a_conta_sem_passar_do_total()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 10);
        ParcelasPagas(Ate(9));

        await CriarService().Update(Edicao("PAGO NO PRAZO", parcelaAtual: 10), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.False(conta.Ativo);
        Assert.Equal(10, conta.ParcelaAtual);
    }

    // ---------- Update: estornar pagamento ----------

    [Theory]
    [InlineData("PAGO NO PRAZO", "PENDENTE")]
    [InlineData("PAGO ATRASADO", "AGUARDANDO")]
    public async Task Update_desfazer_pagamento_volta_a_parcela_atual_para_a_da_transacao(string statusAnterior, string novoStatus)
    {
        ContaParcelada(parcelaAtual: 4, quantidadeParcelas: 10);
        TransacaoGravada(statusAnterior, parcelaAtual: 3);
        ParcelasPagas(1, 2);

        await CriarService().Update(Edicao(novoStatus), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(3, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
    }

    /// <summary>
    /// Caso real (conta 59 no dev): pagar a 9 de 10 leva a conta para 10; desfazer esse pagamento
    /// precisa voltar a conta para 9, mesmo ela estando na última parcela. Antes ela ficava em 10,
    /// e relançar a 9 dava "Todas as parcelas desta conta já foram lançadas".
    /// </summary>
    [Fact]
    public async Task Update_desfazer_pagamento_da_penultima_com_a_conta_na_ultima_volta_para_a_penultima()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 9);
        ParcelasPagas(Ate(8));

        await CriarService().Update(Edicao("PENDENTE", parcelaAtual: 9), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(9, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
    }

    [Fact]
    public async Task Delete_da_penultima_paga_com_a_conta_na_ultima_volta_para_a_penultima()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 9);
        ParcelasPagas(Ate(8));

        await CriarService().Delete(TransactionId, UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(9, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
    }

    [Fact]
    public async Task Update_desfazer_pagamento_da_ultima_parcela_reativa_a_conta()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10, ativo: false);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 10);
        ParcelasPagas(Ate(9));

        await CriarService().Update(Edicao("PENDENTE", parcelaAtual: 10), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.True(conta.Ativo);
        Assert.Equal(10, conta.ParcelaAtual);
    }

    // ---------- BE-P4-19: a conta é recalculada pelo que está pago, não pelo ponteiro ----------

    /// <summary>Antes: a conta estava na última, então pagar a 9 a inativava com a 10 em aberto.</summary>
    [Fact]
    public async Task Pagar_uma_anterior_com_a_conta_na_ultima_nao_inativa_a_conta()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 9);
        ParcelasPagas(Ate(8));

        await CriarService().Update(Edicao("PAGO NO PRAZO", parcelaAtual: 9), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(10, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
    }

    /// <summary>Antes: pagar a 5 adiantada levava a conta para 4 (ponteiro + 1) com a 3 em aberto.</summary>
    [Fact]
    public async Task Pagar_fora_de_ordem_mantem_a_conta_na_menor_parcela_em_aberto()
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 5);
        ParcelasPagas(1, 2);

        await CriarService().Update(Edicao("PAGO NO PRAZO", parcelaAtual: 5), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(3, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
    }

    /// <summary>Antes: pagar a última inativava a conta mesmo com a 8 e a 9 em aberto.</summary>
    [Fact]
    public async Task Pagar_a_ultima_com_anteriores_em_aberto_nao_inativa()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 10);
        ParcelasPagas(Ate(7));

        await CriarService().Update(Edicao("PAGO NO PRAZO", parcelaAtual: 10), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(8, conta.ParcelaAtual);
        Assert.True(conta.Ativo);
    }

    // ---------- Update: sem efeito na conta ----------

    [Theory]
    [InlineData("PENDENTE", "PENDENTE")]
    [InlineData("PAGO NO PRAZO", "PAGO ATRASADO")]   // pago → pago não mexe na parcela
    [InlineData("PENDENTE", "AGUARDANDO")]           // pendente → aguardando também não
    public async Task Update_sem_transicao_de_pagamento_nao_altera_a_conta(string statusAnterior, string novoStatus)
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada(statusAnterior, parcelaAtual: 3);

        await CriarService().Update(Edicao(novoStatus), UserId);

        Assert.Empty(_parcelasAtualizadas);
        _repo.Verify(r => r.Update(It.IsAny<Transacao>(), UserId), Times.Once);
    }

    [Fact]
    public async Task Update_transacao_nao_parcelada_nem_consulta_a_conta()
    {
        await CriarService().Update(Edicao("PAGO NO PRAZO", ehParcelado: false), UserId);

        _accountService.Verify(s => s.GetAccountById(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        Assert.Empty(_parcelasAtualizadas);
        _repo.Verify(r => r.Update(It.IsAny<Transacao>(), UserId), Times.Once);
    }

    // ---------- Update: número da parcela repetido (card [DB] Transações duplicadas, 24/10/2026) ----------

    private void ParcelasLancadas(params int[] parcelas) =>
        _repo.Setup(r => r.GetParcelasLancadas(AccountId, UserId)).ReturnsAsync(parcelas.ToList());

    [Fact]
    public async Task Update_trocando_para_parcela_ja_lancada_e_recusado_sem_gravar()
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 4);
        ParcelasLancadas(1, 2, 3, 4);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Update(Edicao("PENDENTE", parcelaAtual: 2), UserId));

        Assert.Equal("A parcela 2 já está lançada nesta conta.", ex.Message);
        _repo.Verify(r => r.Update(It.IsAny<Transacao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Update_trocando_para_parcela_livre_grava()
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 4);
        ParcelasLancadas(1, 2, 3, 4);

        await CriarService().Update(Edicao("PENDENTE", parcelaAtual: 5), UserId);

        _repo.Verify(r => r.Update(It.Is<Transacao>(t => t.ParcelaAtual == 5), UserId), Times.Once);
    }

    /// <summary>
    /// As duplicatas antigas (de antes do número calculado no backend) ficam como estão, por decisão dela:
    /// editar ou pagar uma delas sem mexer no número continua possível.
    /// </summary>
    [Fact]
    public async Task Update_sem_mudar_o_numero_de_uma_duplicata_antiga_grava()
    {
        ContaParcelada(parcelaAtual: 5, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 5);
        ParcelasLancadas(1, 2, 3, 4, 5, 5, 6);
        ParcelasPagas(1, 2, 3, 4, 5, 6);

        await CriarService().Update(Edicao("PAGO NO PRAZO", parcelaAtual: 5), UserId);

        _repo.Verify(r => r.Update(It.Is<Transacao>(t => t.Situacao == "PAGO NO PRAZO"), UserId), Times.Once);
        _repo.Verify(r => r.GetParcelasLancadas(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_de_parcela_paga_volta_a_parcela_atual_e_remove()
    {
        ContaParcelada(parcelaAtual: 4, quantidadeParcelas: 10);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 3);
        ParcelasPagas(1, 2);

        await CriarService().Delete(TransactionId, UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(3, conta.ParcelaAtual);
        _repo.Verify(r => r.Remove(TransactionId, UserId), Times.Once);
    }

    /// <summary>
    /// A ParcelaAtual da conta é a próxima parcela a lançar e só anda quando uma parcela é paga.
    /// Excluir uma parcela que nunca foi paga não deve mexer nela; senão a próxima inclusão
    /// (o front envia a ParcelaAtual da conta) sairia com o número da parcela excluída.
    /// </summary>
    [Theory]
    [InlineData("PENDENTE")]
    [InlineData("AGUARDANDO")]
    public async Task Delete_de_parcela_nao_paga_nao_altera_a_parcela_atual_da_conta(string status)
    {
        ContaParcelada(parcelaAtual: 4, quantidadeParcelas: 10);
        TransacaoGravada(status, parcelaAtual: 6);

        await CriarService().Delete(TransactionId, UserId);

        Assert.Empty(_parcelasAtualizadas);
        _repo.Verify(r => r.Remove(TransactionId, UserId), Times.Once);
    }

    [Fact]
    public async Task Delete_da_ultima_parcela_paga_reativa_a_conta()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10, ativo: false);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 10);
        ParcelasPagas(Ate(9));

        await CriarService().Delete(TransactionId, UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.True(conta.Ativo);
        Assert.Equal(10, conta.ParcelaAtual);
    }

    [Fact]
    public async Task Delete_de_transacao_inexistente_lanca_excecao_e_nao_remove()
    {
        // Contrato do repositório: não achou → NaoEncontradoException (nunca null). O service só a deixa passar (BE-P4-14).
        _repo.Setup(r => r.GetTransactionById(TransactionId, UserId))
            .ThrowsAsync(new NaoEncontradoException("Transação não encontrada."));

        var ex = await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().Delete(TransactionId, UserId));

        Assert.Contains("não encontrada", ex.Message);
        _repo.Verify(r => r.Remove(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ---------- Transação (BE-P1-05): as gravações de uma operação valem juntas ----------

    private void RegistrarGravacoesDaTransacao()
    {
        _repo.Setup(r => r.Update(It.IsAny<Transacao>(), UserId))
            .Callback(() => _uow.Registrar("Transaction.Update"))
            .ReturnsAsync(new Transacao());
        _repo.Setup(r => r.Remove(TransactionId, UserId))
            .Callback(() => _uow.Registrar("Transaction.Remove"))
            .ReturnsAsync(new Transacao());
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback(() => _uow.Registrar("Transaction.Create"))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Update_de_pagamento_grava_conta_e_transacao_na_mesma_operacao()
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 3);
        ParcelasPagas(1, 2);
        RegistrarGravacoesDaTransacao();

        await CriarService().Update(Edicao("PAGO NO PRAZO"), UserId);

        Assert.Single(_parcelasAtualizadas);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Delete_de_parcela_paga_grava_conta_e_remove_na_mesma_operacao()
    {
        ContaParcelada(parcelaAtual: 4, quantidadeParcelas: 10);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 3);
        ParcelasPagas(1, 2);
        RegistrarGravacoesDaTransacao();

        await CriarService().Delete(TransactionId, UserId);

        Assert.Single(_parcelasAtualizadas);
        _repo.Verify(r => r.Remove(TransactionId, UserId), Times.Once);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Add_com_conta_nova_cria_conta_e_transacoes_na_mesma_operacao()
    {
        _accountService.Setup(s => s.GetAccountById(It.IsAny<int>(), UserId)).ReturnsAsync((ContaDTO)null!);
        _accountService.Setup(s => s.Add(It.IsAny<ContaDTO>(), UserId))
            .Callback(() => _uow.Registrar("Account.Add"))
            .ReturnsAsync(new ContaDTO
            {
                Id = AccountId,
                Valor = 80m,
                CategoriaId = 4,
                Vencimentos = new List<ContaVencimento> { new() { ContaId = AccountId, Dia = 15 } },
            });
        RegistrarGravacoesDaTransacao();

        await CriarService().Add(new TransacaoDTO
        {
            Nome = "Farmácia",
            Valor = 80m,
            CategoriaId = 4,
            Situacao = "PENDENTE",
            Data = new DateOnly(2026, 10, 15),
        }, UserId);

        _repo.Verify(r => r.Create(It.IsAny<List<Transacao>>()), Times.Once);
        Assert.Empty(_uow.Fora);
    }

    // ---------- Add: conta nova com vários dias (modal "Adicionar nova conta", 17/10/2026) ----------

    private ContaDTO? _contaNovaPedida;

    /// <summary>Sem conta: devolve a conta "gravada" com os vencimentos pedidos, como o ContaService faz.</summary>
    private void ContaNovaDevolveOsDiasPedidos()
    {
        _accountService.Setup(s => s.GetAccountById(It.IsAny<int>(), UserId)).ReturnsAsync((ContaDTO)null!);
        _accountService.Setup(s => s.Add(It.IsAny<ContaDTO>(), UserId))
            .ReturnsAsync((ContaDTO pedida, int _) =>
            {
                _contaNovaPedida = pedida;
                return new ContaDTO
                {
                    Id = AccountId,
                    Valor = pedida.Valor,
                    CategoriaId = pedida.CategoriaId,
                    Vencimentos = pedida.DataOperacao.Select(d => new ContaVencimento { ContaId = AccountId, Dia = d }).ToList(),
                };
            });
        _repo.Setup(r => r.Create(It.IsAny<List<Transacao>>()))
            .Callback<List<Transacao>>(t => _criadas = t)
            .Returns(Task.CompletedTask);
    }

    private static TransacaoDTO ContaAvulsa(List<int>? dias) => new()
    {
        Nome = "Escola",
        Valor = 300m,
        CategoriaId = 4,
        Situacao = "PENDENTE",
        Data = new DateOnly(2026, 10, 1),
        DiasVencimento = dias,
    };

    [Fact]
    public async Task Add_com_conta_nova_e_varios_dias_cria_a_conta_com_os_dias_e_uma_transacao_por_dia()
    {
        ContaNovaDevolveOsDiasPedidos();

        await CriarService().Add(ContaAvulsa([20, 5]), UserId);

        Assert.Equal([5, 20], _contaNovaPedida!.DataOperacao);
        Assert.Equal([new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 20)], _criadas!.Select(t => t.Data));
        Assert.All(_criadas!, t => Assert.Equal(150m, t.Valor)); // valor da conta dividido entre os dias
    }

    [Fact]
    public async Task Add_com_conta_nova_sem_dias_usa_o_dia_da_data()
    {
        ContaNovaDevolveOsDiasPedidos();

        var avulsa = ContaAvulsa(null);
        avulsa.Data = new DateOnly(2026, 10, 15);

        await CriarService().Add(avulsa, UserId);

        Assert.Equal([15], _contaNovaPedida!.DataOperacao);
    }

    [Theory]
    [InlineData(new[] { 5, 5 })]
    [InlineData(new[] { 0 })]
    [InlineData(new[] { 32 })]
    public async Task Add_com_conta_nova_e_dias_invalidos_e_recusado(int[] dias)
    {
        ContaNovaDevolveOsDiasPedidos();

        await Assert.ThrowsAsync<ArgumentException>(() => CriarService().Add(ContaAvulsa(dias.ToList()), UserId));

        _accountService.Verify(s => s.Add(It.IsAny<ContaDTO>(), It.IsAny<int>()), Times.Never);
    }

    // Card #22: aba Lançamentos do Cadastro de Contas.
    [Fact]
    public async Task Lancamentos_da_conta_vem_do_mais_recente_para_o_mais_antigo()
    {
        Transacao T(int id, DateOnly data) =>
            new() { Id = id, UsuarioId = UserId, ContaId = AccountId, Data = data, Nome = "Sofá", Situacao = StatusTransacao.PagoNoPrazo };
        _repo.Setup(r => r.GetTransacoesDaConta(AccountId, UserId)).ReturnsAsync(
        [
            T(1, new DateOnly(2026, 8, 5)),
            T(2, new DateOnly(2026, 10, 20)),
            T(3, new DateOnly(2026, 10, 5)),
            T(4, new DateOnly(2026, 9, 5)),
        ]);

        var lancamentos = await CriarService().GetTransacoesDaConta(AccountId, UserId);

        Assert.Equal([2, 3, 4, 1], lancamentos.Select(l => l.Id));
    }

    [Fact]
    public async Task Conta_sem_lancamentos_devolve_lista_vazia()
    {
        _repo.Setup(r => r.GetTransacoesDaConta(AccountId, UserId)).ReturnsAsync([]);

        Assert.Empty(await CriarService().GetTransacoesDaConta(AccountId, UserId));
    }
}
