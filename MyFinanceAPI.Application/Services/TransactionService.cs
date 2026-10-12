using System;
using System.Transactions;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IAccountService accountService, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    // Conta nova, parcela da conta e transações valem juntas (BE-P1-05).
    public Task Add(TransactionDTO transactionDTO, int userId) =>
        _unitOfWork.ExecuteAsync(() => AddNaOperacao(transactionDTO, userId));

    private async Task AddNaOperacao(TransactionDTO transactionDTO, int userId)
    {
        var conta = await _accountService.GetAccountById(transactionDTO.IdAccount ?? 0, userId);
        var mesAno = transactionDTO.Date;
        var transacoes = new List<Domain.Entities.Transaction>();

        if (conta == null)
        {
            var novaConta = new AccountDTO
            {
                Name = transactionDTO.Name,
                Value = transactionDTO.Value,
                Categoryid = (int)transactionDTO.CategoryId,
                Status = 1,
                DataOperacao = [mesAno.Day],
            };
            conta = await _accountService.Add(novaConta, userId);
        }
        int? parcela = conta.EhParcelado ? await ProximaParcela(conta, userId) : null;

        var ultimoDiaMes = DateTime.DaysInMonth(mesAno.Year, mesAno.Month);
        var valorParcelaMensal = conta.Value / conta.ContaVencimentos.Count();
        foreach (var vencimento in conta.ContaVencimentos)
        {
            var dia = Math.Min(vencimento.Dia, ultimoDiaMes);
            var novaTransacao = new Domain.Entities.Transaction
            {
                IdAccount = conta.Id,
                Name = transactionDTO.Name,
                UserId = userId,
                Value = (decimal)valorParcelaMensal,
                Status = transactionDTO.Status,
                EhParcelado = conta.EhParcelado,
                ParcelaAtual = parcela,
                QuantidadeParcelas = conta.EhParcelado ? conta.QuantidadeParcelas : null,
                CategoryId = conta.Categoryid,
                Date = new DateTime(
                    mesAno.Year,
                    mesAno.Month,
                    dia
                )
            };

            transacoes.Add(novaTransacao);
        }

        await _transactionRepository.Create(transacoes);
    }

    // A ParcelaAtual da conta só anda quando se paga; por isso o número vem do que já foi lançado:
    // a menor parcela a partir dela que ainda não existe (lançar dois meses sem pagar dá 4 e 5).
    private async Task<int> ProximaParcela(AccountDTO conta, int userId)
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
        var transaction = _mapper.Map<Domain.Entities.Transaction, TransactionDTO>(await _transactionRepository.GetTransactionById(id, userId));
        if (transaction == null)        {
            throw new Exception("Transação não encontrada ou não pertence ao usuário.");
        }

        if(transaction.EhParcelado)
            await UpdateParcelaAtual(transaction, userId, deleted: true);
        await _transactionRepository.Remove(id, userId);
    }

    public async Task<IEnumerable<AccountGroupingDTO>> GetTransactions(int userId)
    {
        var monthlyUpdate = await _transactionRepository.GetTransactions(userId);
        return _mapper.Map<IEnumerable<AccountGroupingDTO>>(monthlyUpdate);
    }

    // public async Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId, int userId)
    // {
    //     var Transaction = await _transactionRepository.GetTransactionByCategory(categoryId, userId);
    //     return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    // }

    public async Task<TransactionDTO> GetTransactionById(int id, int userId)
    {
        var update = await _transactionRepository.GetTransactionById(id, userId);
        return _mapper.Map<TransactionDTO>(update);
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionByDate(DateTime date, int userId,  bool pesquisaDataCompleta = false)
    {
        var Transaction = await _transactionRepository.GetTransactionByDate(date, userId, pesquisaDataCompleta);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }
    public async Task<IEnumerable<TransactionDTO>> GetContaVencida(DateTime hoje, int userId)
    {
        var Transaction = await _transactionRepository.GetContaVencida(hoje, userId);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionGroupingByDate(DateTime date, int userId, bool pesquisaDataCompleta = false)
    {
        var Transaction = await _transactionRepository.GetTransactionGroupingByDate(date, userId, pesquisaDataCompleta);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public async Task<IEnumerable<TransactionDTO>> BuscarContasMensais(DateTime date, int userId, bool pesquisaDataCompleta = false)
    {
        var Transaction = await _transactionRepository.BuscarContasMensais(date, userId, pesquisaDataCompleta);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public Task Update(TransactionDTO transactionDTO, int userId) =>
        _unitOfWork.ExecuteAsync(() => UpdateNaOperacao(transactionDTO, userId));

    private async Task UpdateNaOperacao(TransactionDTO transactionDTO, int userId)
    {
        if (transactionDTO.EhParcelado)
        {
            await UpdateParcelaAtual(transactionDTO, userId);
        }

        var update = _mapper.Map<Domain.Entities.Transaction>(transactionDTO);
        await _transactionRepository.Update(update, userId);
    }

    private async Task UpdateParcelaAtual(TransactionDTO TransactionDTO, int userId, bool deleted = false)
    {
        var account = await _accountService.GetAccountById(TransactionDTO.IdAccount.Value, userId);
        var transaction = await _transactionRepository.GetTransactionById(TransactionDTO.Id, userId);
        var statusAnterior = transaction.Status;

        var mudouStatus = statusAnterior != TransactionDTO.Status;

        if (mudouStatus || deleted)
        {
            if ((statusAnterior == "PENDENTE" || statusAnterior == "AGUARDANDO") && (TransactionDTO.Status == "PAGO NO PRAZO" || TransactionDTO.Status == "PAGO ATRASADO"))
            {
                var proximaParcela = account.ParcelaAtual.GetValueOrDefault() + 1;
                if (account.ParcelaAtual.GetValueOrDefault() == account.QuantidadeParcelas)
                {
                    account.Status = 2; // Inativa
                }
                else
                {
                    account.ParcelaAtual = proximaParcela;
                }
                await _accountService.UpdateParcela(account, userId);
            }
            // Excluir uma parcela não paga não mexe na conta: ParcelaAtual só anda quando se paga.
            else if ((statusAnterior == "PAGO NO PRAZO" || statusAnterior == "PAGO ATRASADO") && (TransactionDTO.Status == "PENDENTE" || TransactionDTO.Status == "AGUARDANDO" || deleted))
            {
                // A parcela estornada volta a ser a próxima a pagar e a conta fica ativa. Não dá para
                // olhar só a conta: ela estar na última parcela não quer dizer que a estornada seja a última.
                account.ParcelaAtual = transaction.ParcelaAtual;
                account.Status = 1; // Ativa

                await _accountService.UpdateParcela(account, userId);
            }
        }
    }
}
