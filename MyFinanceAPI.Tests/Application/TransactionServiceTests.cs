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
public class TransactionServiceTests
{
    private const int UserId = 7;
    private const int AccountId = 100;
    private const int TransactionId = 500;

    private readonly Mock<ITransactionRepository> _repo = new();
    private readonly Mock<IAccountService> _accountService = new();
    private readonly List<AccountDTO> _parcelasAtualizadas = new();
    private readonly UnitOfWorkFalso _uow = new();

    private TransactionService CriarService()
    {
        _accountService
            .Setup(s => s.UpdateParcela(It.IsAny<AccountDTO>(), UserId))
            .Callback<AccountDTO, int>((conta, _) =>
            {
                _uow.Registrar("UpdateParcela");
                _parcelasAtualizadas.Add(conta);
            })
            .Returns(Task.CompletedTask);

        return new TransactionService(_repo.Object, MapperFactory.Create(), _accountService.Object, _uow);
    }

    private AccountDTO ContaParcelada(int parcelaAtual, int quantidadeParcelas, int status = 1)
    {
        var conta = new AccountDTO
        {
            Id = AccountId,
            Name = "Notebook",
            Value = 3000m,
            EhParcelado = true,
            ParcelaAtual = parcelaAtual,
            QuantidadeParcelas = quantidadeParcelas,
            Status = status,
            DataOperacao = new List<int> { 10 },
        };
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(conta);
        return conta;
    }

    private void TransacaoGravada(string status, int parcelaAtual)
    {
        _repo.Setup(r => r.GetTransactionById(TransactionId, UserId)).ReturnsAsync(new Transaction
        {
            Id = TransactionId,
            IdAccount = AccountId,
            Name = "Notebook",
            Value = 300m,
            Status = status,
            EhParcelado = true,
            ParcelaAtual = parcelaAtual,
            QuantidadeParcelas = 10,
            UserId = UserId,
            Date = new DateOnly(2026, 10, 10),
        });
    }

    private static TransactionDTO Edicao(string novoStatus, int parcelaAtual = 3, bool ehParcelado = true) => new()
    {
        Id = TransactionId,
        IdAccount = AccountId,
        Name = "Notebook",
        Value = 300m,
        Status = novoStatus,
        EhParcelado = ehParcelado,
        ParcelaAtual = parcelaAtual,
        QuantidadeParcelas = 10,
        Date = new DateOnly(2026, 10, 10),
    };

    // ---------- Add ----------

    [Fact]
    public async Task Add_divide_o_valor_da_conta_entre_os_vencimentos_e_limita_o_dia_ao_fim_do_mes()
    {
        var conta = new AccountDTO
        {
            Id = AccountId,
            Value = 300m,
            CategoryId = 4,
            DataOperacao = new List<int> { 5, 31 },
            ContaVencimentos = new List<ContaVencimento>
            {
                new() { ContaId = AccountId, Dia = 5 },
                new() { ContaId = AccountId, Dia = 31 },
            },
        };
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(conta);
        List<Transaction>? criadas = null;
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback<List<Transaction>>(t => criadas = t)
            .Returns(Task.CompletedTask);

        await CriarService().Add(new TransactionDTO
        {
            IdAccount = AccountId,
            Name = "Aluguel",
            Status = "PENDENTE",
            Date = new DateOnly(2026, 2, 1),
        }, UserId);

        Assert.NotNull(criadas);
        Assert.Equal(2, criadas!.Count);
        Assert.All(criadas, t =>
        {
            Assert.Equal(150m, t.Value);
            Assert.Equal(AccountId, t.IdAccount);
            Assert.Equal(4, t.CategoryId);
            Assert.Equal(UserId, t.UserId);
        });
        Assert.Equal(new DateOnly(2026, 2, 5), criadas[0].Date);
        Assert.Equal(new DateOnly(2026, 2, 28), criadas[1].Date); // dia 31 em fevereiro → último dia
    }

    [Fact]
    public async Task Add_devolve_as_transacoes_criadas_uma_por_vencimento()
    {
        // Uma por vencimento da conta; o 201 leva todas, com o Location da primeira (BE-P3-05).
        ContaParceladaParaLancar(parcelaAtual: 1, quantidadeParcelas: 10, [], vencimentos: 2);
        var proximoId = 900;
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback<List<Transaction>>(t => t.ForEach(x => x.Id = proximoId++))
            .Returns(Task.CompletedTask);

        var criadas = await CriarService().Add(Lancamento(), UserId);

        Assert.Equal([900, 901], criadas.Select(t => t.Id));
        Assert.All(criadas, t => Assert.Equal(AccountId, t.IdAccount));
    }

    [Fact]
    public async Task Add_devolve_transacoes_que_viram_JSON_mesmo_com_a_conta_rastreada()
    {
        // Na gravação o EF liga a conta rastreada às transações novas (Account.Transactions ↔
        // Transaction.Account). Devolvidas assim, o JSON do 201 entrava em ciclo — com a transação
        // já gravada (achado na validação do BE-P3, 17/10/2026).
        ContaParceladaParaLancar(parcelaAtual: 1, quantidadeParcelas: 10, []);
        var contaRastreada = new Account { Id = AccountId, Name = "Notebook", Transactions = new List<Transaction>() };
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback<List<Transaction>>(t => t.ForEach(x =>
            {
                x.Id = 900;
                x.Account = contaRastreada;
                contaRastreada.Transactions.Add(x);
            }))
            .Returns(Task.CompletedTask);

        var criadas = await CriarService().Add(Lancamento(), UserId);

        var json = System.Text.Json.JsonSerializer.Serialize(criadas, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
        Assert.Contains("\"id\":900", json);
    }

    // ---------- Add: número da parcela ----------

    private List<Transaction>? _criadas;

    private void ContaParceladaParaLancar(int parcelaAtual, int quantidadeParcelas, int[] lancadas, int vencimentos = 1)
    {
        var conta = new AccountDTO
        {
            Id = AccountId,
            Value = 300m,
            CategoryId = 4,
            EhParcelado = true,
            ParcelaAtual = parcelaAtual,
            QuantidadeParcelas = quantidadeParcelas,
            ContaVencimentos = Enumerable.Range(1, vencimentos)
                .Select(i => new ContaVencimento { ContaId = AccountId, Dia = 10 + i })
                .ToList(),
        };
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(conta);
        _repo.Setup(r => r.GetParcelasLancadas(AccountId, UserId)).ReturnsAsync(lancadas.ToList());
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback<List<Transaction>>(t => _criadas = t)
            .Returns(Task.CompletedTask);
    }

    /// <summary>O payload do front é ignorado: o número sai da conta e do que já foi lançado.</summary>
    private static TransactionDTO Lancamento() => new()
    {
        IdAccount = AccountId,
        Name = "Notebook",
        Status = "PENDENTE",
        EhParcelado = false,
        ParcelaAtual = 99,
        QuantidadeParcelas = null,
        Date = new DateOnly(2026, 11, 1),
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
        _repo.Verify(r => r.Create(It.IsAny<List<Transaction>>()), Times.Never);
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
        _accountService.Setup(s => s.GetAccountById(AccountId, UserId)).ReturnsAsync(new AccountDTO
        {
            Id = AccountId,
            Value = 100m,
            EhParcelado = false,
            ContaVencimentos = new List<ContaVencimento> { new() { ContaId = AccountId, Dia = 5 } },
        });
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback<List<Transaction>>(t => _criadas = t)
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

        await CriarService().Update(Edicao(novoStatus), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(4, conta.ParcelaAtual);
        Assert.Equal(1, conta.Status);
        _repo.Verify(r => r.Update(It.Is<Transaction>(t => t.Id == TransactionId && t.Status == novoStatus), UserId), Times.Once);
    }

    [Fact]
    public async Task Update_pagar_a_ultima_parcela_inativa_a_conta_sem_passar_do_total()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 10);

        await CriarService().Update(Edicao("PAGO NO PRAZO", parcelaAtual: 10), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(StatusAtivo.Inativo, conta.Status);   // inativa = 0, como o front grava (DB-P1-03)
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

        await CriarService().Update(Edicao(novoStatus), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(3, conta.ParcelaAtual);
        Assert.Equal(1, conta.Status);
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

        await CriarService().Update(Edicao("PENDENTE", parcelaAtual: 9), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(9, conta.ParcelaAtual);
        Assert.Equal(1, conta.Status);
    }

    [Fact]
    public async Task Delete_da_penultima_paga_com_a_conta_na_ultima_volta_para_a_penultima()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 9);

        await CriarService().Delete(TransactionId, UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(9, conta.ParcelaAtual);
        Assert.Equal(1, conta.Status);
    }

    [Fact]
    public async Task Update_desfazer_pagamento_da_ultima_parcela_reativa_a_conta()
    {
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10, status: 2);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 10);

        await CriarService().Update(Edicao("PENDENTE", parcelaAtual: 10), UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(1, conta.Status);          // 1 = ativa
        Assert.Equal(10, conta.ParcelaAtual);
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
        _repo.Verify(r => r.Update(It.IsAny<Transaction>(), UserId), Times.Once);
    }

    [Fact]
    public async Task Update_transacao_nao_parcelada_nem_consulta_a_conta()
    {
        await CriarService().Update(Edicao("PAGO NO PRAZO", ehParcelado: false), UserId);

        _accountService.Verify(s => s.GetAccountById(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        Assert.Empty(_parcelasAtualizadas);
        _repo.Verify(r => r.Update(It.IsAny<Transaction>(), UserId), Times.Once);
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_de_parcela_paga_volta_a_parcela_atual_e_remove()
    {
        ContaParcelada(parcelaAtual: 4, quantidadeParcelas: 10);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 3);

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
        ContaParcelada(parcelaAtual: 10, quantidadeParcelas: 10, status: 2);
        TransacaoGravada("PAGO NO PRAZO", parcelaAtual: 10);

        await CriarService().Delete(TransactionId, UserId);

        var conta = Assert.Single(_parcelasAtualizadas);
        Assert.Equal(1, conta.Status);
        Assert.Equal(10, conta.ParcelaAtual);
    }

    [Fact]
    public async Task Delete_de_transacao_inexistente_lanca_excecao_e_nao_remove()
    {
        _repo.Setup(r => r.GetTransactionById(TransactionId, UserId)).ReturnsAsync((Transaction)null!);

        var ex = await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().Delete(TransactionId, UserId));

        Assert.Contains("não encontrada", ex.Message);
        _repo.Verify(r => r.Remove(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ---------- Transação (BE-P1-05): as gravações de uma operação valem juntas ----------

    private void RegistrarGravacoesDaTransacao()
    {
        _repo.Setup(r => r.Update(It.IsAny<Transaction>(), UserId))
            .Callback(() => _uow.Registrar("Transaction.Update"))
            .ReturnsAsync(new Transaction());
        _repo.Setup(r => r.Remove(TransactionId, UserId))
            .Callback(() => _uow.Registrar("Transaction.Remove"))
            .ReturnsAsync(new Transaction());
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback(() => _uow.Registrar("Transaction.Create"))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Update_de_pagamento_grava_conta_e_transacao_na_mesma_operacao()
    {
        ContaParcelada(parcelaAtual: 3, quantidadeParcelas: 10);
        TransacaoGravada("PENDENTE", parcelaAtual: 3);
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
        RegistrarGravacoesDaTransacao();

        await CriarService().Delete(TransactionId, UserId);

        Assert.Single(_parcelasAtualizadas);
        _repo.Verify(r => r.Remove(TransactionId, UserId), Times.Once);
        Assert.Empty(_uow.Fora);
    }

    [Fact]
    public async Task Add_com_conta_nova_cria_conta_e_transacoes_na_mesma_operacao()
    {
        _accountService.Setup(s => s.GetAccountById(It.IsAny<int>(), UserId)).ReturnsAsync((AccountDTO)null!);
        _accountService.Setup(s => s.Add(It.IsAny<AccountDTO>(), UserId))
            .Callback(() => _uow.Registrar("Account.Add"))
            .ReturnsAsync(new AccountDTO
            {
                Id = AccountId,
                Value = 80m,
                CategoryId = 4,
                ContaVencimentos = new List<ContaVencimento> { new() { ContaId = AccountId, Dia = 15 } },
            });
        RegistrarGravacoesDaTransacao();

        await CriarService().Add(new TransactionDTO
        {
            Name = "Farmácia",
            Value = 80m,
            CategoryId = 4,
            Status = "PENDENTE",
            Date = new DateOnly(2026, 10, 15),
        }, UserId);

        _repo.Verify(r => r.Create(It.IsAny<List<Transaction>>()), Times.Once);
        Assert.Empty(_uow.Fora);
    }

    // ---------- Add: conta nova com vários dias (modal "Adicionar nova conta", 17/10/2026) ----------

    private AccountDTO? _contaNovaPedida;

    /// <summary>Sem conta: devolve a conta "gravada" com os vencimentos pedidos, como o AccountService faz.</summary>
    private void ContaNovaDevolveOsDiasPedidos()
    {
        _accountService.Setup(s => s.GetAccountById(It.IsAny<int>(), UserId)).ReturnsAsync((AccountDTO)null!);
        _accountService.Setup(s => s.Add(It.IsAny<AccountDTO>(), UserId))
            .ReturnsAsync((AccountDTO pedida, int _) =>
            {
                _contaNovaPedida = pedida;
                return new AccountDTO
                {
                    Id = AccountId,
                    Value = pedida.Value,
                    CategoryId = pedida.CategoryId,
                    ContaVencimentos = pedida.DataOperacao.Select(d => new ContaVencimento { ContaId = AccountId, Dia = d }).ToList(),
                };
            });
        _repo.Setup(r => r.Create(It.IsAny<List<Transaction>>()))
            .Callback<List<Transaction>>(t => _criadas = t)
            .Returns(Task.CompletedTask);
    }

    private static TransactionDTO ContaAvulsa(List<int>? dias) => new()
    {
        Name = "Escola",
        Value = 300m,
        CategoryId = 4,
        Status = "PENDENTE",
        Date = new DateOnly(2026, 10, 1),
        DiasVencimento = dias,
    };

    [Fact]
    public async Task Add_com_conta_nova_e_varios_dias_cria_a_conta_com_os_dias_e_uma_transacao_por_dia()
    {
        ContaNovaDevolveOsDiasPedidos();

        await CriarService().Add(ContaAvulsa([20, 5]), UserId);

        Assert.Equal([5, 20], _contaNovaPedida!.DataOperacao);
        Assert.Equal([new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 20)], _criadas!.Select(t => t.Date));
        Assert.All(_criadas!, t => Assert.Equal(150m, t.Value)); // valor da conta dividido entre os dias
    }

    [Fact]
    public async Task Add_com_conta_nova_sem_dias_usa_o_dia_da_data()
    {
        ContaNovaDevolveOsDiasPedidos();

        var avulsa = ContaAvulsa(null);
        avulsa.Date = new DateOnly(2026, 10, 15);

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

        _accountService.Verify(s => s.Add(It.IsAny<AccountDTO>(), It.IsAny<int>()), Times.Never);
    }
}
