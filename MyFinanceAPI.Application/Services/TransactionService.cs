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
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task Add(TransactionDTO TransactionDTO)
    {
        var update = _mapper.Map<Domain.Entities.Transaction>(TransactionDTO);
        await _transactionRepository.Create(update);
    }

    public async Task Delete(int id)
    {
        await _transactionRepository.Remove(id);
    }

    public async Task<IEnumerable<AccountGroupingDTO>> GetTransactions()
    {
        var monthlyUpdate = await _transactionRepository.GetTransactions();
        return _mapper.Map<IEnumerable<AccountGroupingDTO>>(monthlyUpdate);
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId)
    {
        var Transaction = await _transactionRepository.GetTransactionByCategory(categoryId);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public async Task<TransactionDTO> GetTransactionById(int id)
    {
        var update = await _transactionRepository.GetTransactionById(id);
        return _mapper.Map<TransactionDTO>(update);
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionByDate(DateTime date)
    {
        var Transaction = await _transactionRepository.GetTransactionByDate(date);
        return _mapper.Map<IEnumerable<TransactionDTO>>(Transaction);
    }

    public async Task<IEnumerable<AccountGroupingDTO>> GetTransactionGroupingByDate(DateTime date)
    {
        var Transaction = await _transactionRepository.GetTransactionGroupingByDate(date);
        return _mapper.Map<IEnumerable<AccountGroupingDTO>>(Transaction);
    }

    public async Task Update(TransactionDTO TransactionDTO)
    {
        var update = _mapper.Map<Domain.Entities.Transaction>(TransactionDTO);
        await _transactionRepository.Update(update);
    }
}
