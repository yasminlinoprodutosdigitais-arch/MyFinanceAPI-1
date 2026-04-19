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

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IAccountService accountService)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _accountService = accountService;
    }

    public async Task Add(TransactionDTO transactionDTO, int userId)
    {
        var conta = await _accountService.GetAccountById(transactionDTO.IdAccount ?? 0, userId);
        var mesAno = transactionDTO.Date;
        var transacoes = new List<Domain.Entities.Transaction>();

        if (conta == null)
        {
            var novaConta = new AccountDTO
            {
                Name = transactionDTO.Name,
                Value = (decimal)transactionDTO.Value,
                Categoryid = (int)transactionDTO.CategoryId,
                Status = 1,
                DataOperacao = [mesAno.Day],
            };
            conta = await _accountService.Add(novaConta, userId);
        }
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
                EhParcelado = transactionDTO.EhParcelado,
                ParcelaAtual = transactionDTO.ParcelaAtual,
                QuantidadeParcelas = transactionDTO.QuantidadeParcelas,
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

    public async Task Delete(int id, int userId)
    {
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

    public async Task<IEnumerable<TransactionDTO>> GetTransactionByDate(DateTime date, int userId)
    {
        var Transaction = await _transactionRepository.GetTransactionByDate(date, userId);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionGroupingByDate(DateTime date, int userId)
    {
        var Transaction = await _transactionRepository.GetTransactionGroupingByDate(date, userId);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public async Task Update(TransactionDTO TransactionDTO, int userId)
    {
        if (TransactionDTO.EhParcelado)
        {
            await UpdateParcelaAtual(TransactionDTO, userId);
        }

        var update = _mapper.Map<Domain.Entities.Transaction>(TransactionDTO);
        await _transactionRepository.Update(update, userId);
    }

    private async Task UpdateParcelaAtual(TransactionDTO TransactionDTO, int userId)
    {
        var account = await _accountService.GetAccountById(TransactionDTO.IdAccount.Value, userId);
        var transaction = await _transactionRepository.GetTransactionById(TransactionDTO.Id, userId);
        var statusAnterior = transaction.Status;

        var mudouStatus = statusAnterior != TransactionDTO.Status;

        if (mudouStatus)
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
                await _accountService.Update(account, userId);
            }
            else if ((statusAnterior == "PAGO NO PRAZO" || statusAnterior == "PAGO ATRASADO") && (TransactionDTO.Status == "PENDENTE" || TransactionDTO.Status == "AGUARDANDO"))
            {
                var proximaParcela = account.ParcelaAtual.GetValueOrDefault() - 1;
                if (account.ParcelaAtual.GetValueOrDefault() == account.QuantidadeParcelas)
                {
                    account.Status = 1; // Ativa
                }
                else
                {
                    account.ParcelaAtual = proximaParcela;
                }

                await _accountService.Update(account, userId);
            }
        }
    }
}
