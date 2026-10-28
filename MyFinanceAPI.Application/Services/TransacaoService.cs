using System;
using System.Transactions;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TransacaoService : ITransacaoService
{
    private readonly ITransacaoRepository _transactionRepository;
    private readonly IContaService _accountService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TransacaoService(ITransacaoRepository transactionRepository, IMapper mapper, IContaService accountService, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    // Conta nova, parcela da conta e transações valem juntas (BE-P1-05).
    public Task<IReadOnlyList<TransacaoDTO>> Add(TransacaoDTO transactionDTO, int userId) =>
        _unitOfWork.ExecuteAsync(() => AddNaOperacao(transactionDTO, userId));

    /// <summary>Uma transação por vencimento da conta; devolve as criadas, já com id.</summary>
    private async Task<IReadOnlyList<TransacaoDTO>> AddNaOperacao(TransacaoDTO transactionDTO, int userId)
    {
        if (transactionDTO.ContaId is null && transactionDTO.CadastrarConta == false)
            return await AddAvulso(transactionDTO, userId);

        var conta = await _accountService.GetAccountById(transactionDTO.ContaId ?? 0, userId);
        var mesAno = transactionDTO.Data;
        var transacoes = new List<Domain.Entities.Transacao>();

        if (conta == null)
        {
            // Antes o (int) de um CategoriaId nulo virava 500 (BE-P4-01).
            if (transactionDTO.CategoriaId is not int categoriaId)
                throw new ArgumentException("Informe a categoria para criar a conta do lançamento.");

            var novaConta = new ContaDTO
            {
                Nome = transactionDTO.Nome,
                Valor = transactionDTO.Valor,
                CategoriaId = categoriaId,
                Ativo = true,
                DataOperacao = DiasDaContaNova(transactionDTO),
            };
            conta = await _accountService.Add(novaConta, userId);
        }
        else if (transactionDTO.ValorDoMes is <= 0)
            throw new ArgumentException("O valor do mês deve ser maior que zero.");

        // Lote (card #48): dias e valor pedidos valem só para este lançamento; o cadastro da conta não muda.
        // Conta nova: o EF liga os vencimentos gravados à conta rastreada, então Vencimentos já vem preenchido.
        var diasDoMes = transactionDTO.ContaId is not null && transactionDTO.DiasVencimento is { Count: > 0 }
            ? DiasDaContaNova(transactionDTO)
            : conta.Vencimentos?.Select(v => v.Dia).ToList();
        // Conta antiga sem vencimento dividia por zero (BE-P4-01).
        if (diasDoMes is not { Count: > 0 })
            throw new ArgumentException("A conta não tem dia de vencimento cadastrado. Edite a conta e informe ao menos um dia.");

        int? parcela = conta.EhParcelado ? await ProximaParcela(conta, userId) : null;

        var ultimoDiaMes = DateTime.DaysInMonth(mesAno.Year, mesAno.Month);
        var valorParcelaMensal = (transactionDTO.ValorDoMes ?? conta.Valor) / diasDoMes.Count;
        foreach (var diaDoVencimento in diasDoMes)
        {
            var dia = Math.Min(diaDoVencimento, ultimoDiaMes);
            var novaTransacao = new Domain.Entities.Transacao
            {
                ContaId = conta.Id,
                Nome = transactionDTO.Nome,
                UsuarioId = userId,
                Valor = (decimal)valorParcelaMensal,
                Situacao = transactionDTO.Situacao,
                EhParcelado = conta.EhParcelado,
                ParcelaAtual = parcela,
                QuantidadeParcelas = conta.EhParcelado ? conta.QuantidadeParcelas : null,
                CategoriaId = conta.CategoriaId,
                Data = new DateOnly(
                    mesAno.Year,
                    mesAno.Month,
                    dia
                )
            };

            transacoes.Add(novaTransacao);
        }

        await _transactionRepository.Create(transacoes);

        // Na gravação o EF liga a conta rastreada às transações (Account.Transactions ↔ Transaction.Account):
        // devolvida assim, a resposta entra em ciclo no JSON. A conta já vai em ContaId ("idAccount" no JSON).
        var criadas = _mapper.Map<List<TransacaoDTO>>(transacoes);
        criadas.ForEach(t => t.Conta = null);
        return criadas;
    }

    /// <summary>
    /// Lançamento avulso (card #47, "Não, só este mês"): nenhuma conta no cadastro. Um lançamento por dia pedido,
    /// com o valor dividido entre eles, como a conta faria; dia além do fim do mês cai no último dia.
    /// </summary>
    private async Task<IReadOnlyList<TransacaoDTO>> AddAvulso(TransacaoDTO transactionDTO, int userId)
    {
        if (transactionDTO.CategoriaId is not int categoriaId)
            throw new ArgumentException("Informe a categoria do lançamento.");

        var dias = DiasDaContaNova(transactionDTO);
        var mesAno = transactionDTO.Data;
        var ultimoDiaMes = DateTime.DaysInMonth(mesAno.Year, mesAno.Month);
        var valor = transactionDTO.Valor / dias.Count;

        var transacoes = dias.Select(dia => new Domain.Entities.Transacao
        {
            Nome = transactionDTO.Nome,
            UsuarioId = userId,
            Valor = valor,
            Situacao = transactionDTO.Situacao,
            EhParcelado = false,
            CategoriaId = categoriaId,
            Observacao = transactionDTO.Observacao,
            Data = new DateOnly(mesAno.Year, mesAno.Month, Math.Min(dia, ultimoDiaMes))
        }).ToList();

        await _transactionRepository.Create(transacoes);
        return _mapper.Map<List<TransacaoDTO>>(transacoes);
    }

    /// <summary>
    /// Dias pedidos no lançamento — da conta criada junto, ou só deste mês no lote (card #48) — com as mesmas regras do Cadastro de Contas (
    /// 1 a 31, sem repetir), em ordem; sem dias, o dia da data.
    /// </summary>
    private static List<int> DiasDaContaNova(TransacaoDTO transactionDTO)
    {
        var dias = transactionDTO.DiasVencimento;
        if (dias is not { Count: > 0 })
            return [transactionDTO.Data.Day];

        if (dias.Any(d => d is < 1 or > 31))
            throw new ArgumentException("Dia de vencimento deve estar entre 1 e 31.");
        if (dias.Distinct().Count() != dias.Count)
            throw new ArgumentException("Dia de vencimento repetido.");

        return dias.Order().ToList();
    }

    // A ParcelaAtual da conta só anda quando se paga; por isso o número vem do que já foi lançado:
    // a menor parcela a partir dela que ainda não existe (lançar dois meses sem pagar dá 4 e 5).
    // O mês não entra na conta, de propósito (BE-P4-21, decisão de 23/10/2026): relançar uma parcela excluída
    // num mês posterior é legítimo (pagar atrasado), mesmo que ela fique depois da seguinte no calendário.
    private async Task<int> ProximaParcela(ContaDTO conta, int userId)
    {
        var lancadas = (await _transactionRepository.GetParcelasLancadas(conta.Id, userId)).ToHashSet();

        var parcela = Math.Max(1, conta.ParcelaAtual ?? 1);
        while (lancadas.Contains(parcela))
            parcela++;

        if (parcela > conta.QuantidadeParcelas)
            throw new ArgumentException("Todas as parcelas desta conta já foram lançadas.");

        return parcela;
    }

    public Task Delete(int id, int userId) =>
        _unitOfWork.ExecuteAsync(() => DeleteNaOperacao(id, userId));

    private async Task DeleteNaOperacao(int id, int userId)
    {
        // O repositório já lança NaoEncontradoException quando não acha (BE-P4-14).
        var transaction = _mapper.Map<Domain.Entities.Transacao, TransacaoDTO>(await _transactionRepository.GetTransactionById(id, userId));

        if(transaction.EhParcelado)
            await UpdateParcelaAtual(transaction, userId, deleted: true);
        await _transactionRepository.Remove(id, userId);
    }

    public async Task<TransacaoDTO> GetTransactionById(int id, int userId)
    {
        var update = await _transactionRepository.GetTransactionById(id, userId);
        return _mapper.Map<TransacaoDTO>(update);
    }

    public async Task<IEnumerable<TransacaoDTO>> GetTransactionByDate(DateTime date, int userId,  bool pesquisaDataCompleta = false)
    {
        var Transaction = await _transactionRepository.GetTransactionByDate(date, userId, pesquisaDataCompleta);
        return _mapper.Map<IEnumerable<TransacaoDTO>>(Transaction);
    }
    public async Task<IEnumerable<TransacaoDTO>> GetContaVencida(DateTime hoje, int userId)
    {
        var Transaction = await _transactionRepository.GetContaVencida(hoje, userId);
        return _mapper.Map<IEnumerable<TransacaoDTO>>(Transaction);
    }

    public async Task<IReadOnlyList<TransacaoDTO>> GetTransacoesDaConta(int contaId, int userId)
    {
        var lancamentos = await _transactionRepository.GetTransacoesDaConta(contaId, userId);
        return _mapper.Map<List<TransacaoDTO>>(lancamentos
            .OrderByDescending(t => t.Data)
            .ThenByDescending(t => t.Id));
    }

    public async Task<IEnumerable<TransacaoDTO>> BuscarContasMensais(DateTime date, int userId, bool pesquisaDataCompleta = false)
    {
        var Transaction = await _transactionRepository.BuscarContasMensais(date, userId, pesquisaDataCompleta);
        return _mapper.Map<IEnumerable<TransacaoDTO>>(Transaction);
    }

    public Task Update(TransacaoDTO transactionDTO, int userId) =>
        _unitOfWork.ExecuteAsync(() => UpdateNaOperacao(transactionDTO, userId));

    private async Task UpdateNaOperacao(TransacaoDTO transactionDTO, int userId)
    {
        if (transactionDTO.EhParcelado)
        {
            await RecusarParcelaRepetida(transactionDTO, userId);
            await UpdateParcelaAtual(transactionDTO, userId);
        }

        var update = _mapper.Map<Domain.Entities.Transacao>(transactionDTO);
        await _transactionRepository.Update(update, userId);
    }

    /// <summary>
    /// O número da parcela é editável na tela; trocar para um já lançado na mesma conta é recusado.
    /// Só vale quando o número muda: as duplicatas antigas ficam como estão e continuam editáveis
    /// (decisão de 24/10/2026). A conta vem da transação gravada, não do corpo.
    /// </summary>
    private async Task RecusarParcelaRepetida(TransacaoDTO transactionDTO, int userId)
    {
        var gravada = await _transactionRepository.GetTransactionById(transactionDTO.Id, userId);
        if (transactionDTO.ParcelaAtual is not int nova || nova == gravada.ParcelaAtual || gravada.ContaId is not int contaId)
            return;

        var lancadas = await _transactionRepository.GetParcelasLancadas(contaId, userId);
        if (lancadas.Contains(nova))
            throw new ArgumentException($"A parcela {nova} já está lançada nesta conta.");
    }

    /// <summary>
    /// A conta só muda quando a transação entra ou sai de "pago" (pagar, estornar ou excluir uma paga).
    /// Aí ela é recalculada pelo que está pago (BE-P4-19): ParcelaAtual é a menor parcela de 1 até o total
    /// que ainda não foi paga, e a conta fica inativa só com todas pagas. Antes ela andava pelo ponteiro
    /// (ponteiro + 1, inativa se o ponteiro estivesse na última), e pagar fora de ordem a deixava errada.
    /// </summary>
    private async Task UpdateParcelaAtual(TransacaoDTO TransactionDTO, int userId, bool deleted = false)
    {
        var transaction = await _transactionRepository.GetTransactionById(TransactionDTO.Id, userId);
        var estavaPaga = StatusTransacao.EhPago(transaction.Situacao);
        var ficaPaga = !deleted && StatusTransacao.EhPago(TransactionDTO.Situacao);
        if (estavaPaga == ficaPaga)
            return;

        // A conta vem da transação gravada, não do corpo da requisição.
        if (transaction.ContaId is not int contaId)
            return;
        var account = await _accountService.GetAccountById(contaId, userId);
        if (account?.QuantidadeParcelas is not int total)
            return;

        var pagas = (await _transactionRepository.GetParcelasPagas(account.Id, userId, transaction.Id)).ToHashSet();
        if (ficaPaga && transaction.ParcelaAtual is int parcela)
            pagas.Add(parcela);

        var emAberto = Enumerable.Range(1, total).Where(p => !pagas.Contains(p)).ToList();
        account.ParcelaAtual = emAberto.Count > 0 ? emAberto[0] : total;
        account.Ativo = emAberto.Count > 0;

        await _accountService.UpdateParcela(account, userId);
    }
}
