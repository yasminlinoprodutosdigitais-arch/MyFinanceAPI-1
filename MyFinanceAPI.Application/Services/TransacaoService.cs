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
        var conta = await _accountService.GetAccountById(transactionDTO.ContaId ?? 0, userId);
        var mesAno = transactionDTO.Data;
        var transacoes = new List<Domain.Entities.Transacao>();

        if (conta == null)
        {
            var novaConta = new ContaDTO
            {
                Nome = transactionDTO.Nome,
                Valor = transactionDTO.Valor,
                CategoriaId = (int)transactionDTO.CategoriaId,
                Ativo = StatusAtivo.Ativo,
                DataOperacao = DiasDaContaNova(transactionDTO),
            };
            conta = await _accountService.Add(novaConta, userId);
        }
        int? parcela = conta.EhParcelado ? await ProximaParcela(conta, userId) : null;

        var ultimoDiaMes = DateTime.DaysInMonth(mesAno.Year, mesAno.Month);
        var valorParcelaMensal = conta.Valor / conta.Vencimentos.Count();
        foreach (var vencimento in conta.Vencimentos)
        {
            var dia = Math.Min(vencimento.Dia, ultimoDiaMes);
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
    /// Vencimentos da conta criada no lançamento: os dias pedidos (mesmas regras do Cadastro de Contas —
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
        var transaction = _mapper.Map<Domain.Entities.Transacao, TransacaoDTO>(await _transactionRepository.GetTransactionById(id, userId));
        if (transaction == null)        {
            throw new NaoEncontradoException("Transação não encontrada.");
        }

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
            await UpdateParcelaAtual(transactionDTO, userId);
        }

        var update = _mapper.Map<Domain.Entities.Transacao>(transactionDTO);
        await _transactionRepository.Update(update, userId);
    }

    private async Task UpdateParcelaAtual(TransacaoDTO TransactionDTO, int userId, bool deleted = false)
    {
        var account = await _accountService.GetAccountById(TransactionDTO.ContaId.Value, userId);
        var transaction = await _transactionRepository.GetTransactionById(TransactionDTO.Id, userId);
        var statusAnterior = transaction.Situacao;

        var mudouStatus = statusAnterior != TransactionDTO.Situacao;

        if (mudouStatus || deleted)
        {
            if (StatusTransacao.EhEmAberto(statusAnterior) && StatusTransacao.EhPago(TransactionDTO.Situacao))
            {
                var proximaParcela = account.ParcelaAtual.GetValueOrDefault() + 1;
                if (account.ParcelaAtual.GetValueOrDefault() == account.QuantidadeParcelas)
                {
                    account.Ativo = StatusAtivo.Inativo;
                }
                else
                {
                    account.ParcelaAtual = proximaParcela;
                }
                await _accountService.UpdateParcela(account, userId);
            }
            // Excluir uma parcela não paga não mexe na conta: ParcelaAtual só anda quando se paga.
            else if (StatusTransacao.EhPago(statusAnterior) && (StatusTransacao.EhEmAberto(TransactionDTO.Situacao) || deleted))
            {
                // A parcela estornada volta a ser a próxima a pagar e a conta fica ativa. Não dá para
                // olhar só a conta: ela estar na última parcela não quer dizer que a estornada seja a última.
                account.ParcelaAtual = transaction.ParcelaAtual;
                account.Ativo = StatusAtivo.Ativo;

                await _accountService.UpdateParcela(account, userId);
            }
        }
    }
}
