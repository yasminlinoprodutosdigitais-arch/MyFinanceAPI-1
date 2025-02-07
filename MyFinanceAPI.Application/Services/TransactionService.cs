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
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _accountRepository = _accountRepository;
    }

    public async Task Add(TransactionDTO TransactionDTO, int userId)
    {
        var update = _mapper.Map<Domain.Entities.Transaction>(TransactionDTO);
        update.UserId = userId;
        await _transactionRepository.Create(update);
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

    public async Task<IEnumerable<AccountGroupingDTO>> GetTransactionGroupingByDate(DateTime date, int userId)
    {
        var Transaction = await _transactionRepository.GetTransactionGroupingByDate(date, userId);
        return _mapper.Map<IEnumerable<AccountGroupingDTO>>(Transaction);
    }

    public async Task Update(TransactionDTO TransactionDTO, int userId)
    {
        var update = _mapper.Map<Domain.Entities.Transaction>(TransactionDTO);
        await _transactionRepository.Update(update, userId);
    }
}
