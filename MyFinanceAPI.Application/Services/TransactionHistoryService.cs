using System;
using System.Transactions;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TransactionHistoryService : ITransactionHistoryService
{
    private readonly ITransactionHistoryRepository _transactionHistoryRepository;
    private readonly IMapper _mapper;

    public TransactionHistoryService(ITransactionHistoryRepository transactionHistoryRepository, IMapper mapper)
    {
        _transactionHistoryRepository = transactionHistoryRepository;
        _mapper = mapper;
    }

    public async Task Add(TransactionHistoryDTO transactionHistoryDTO)
    {
        var transaction = _mapper.Map<TransactionHistory>(transactionHistoryDTO);
        await _transactionHistoryRepository.Create(transaction);
    }

    public async Task Delete(TransactionHistoryDTO transactionHistoryDTO)
    {
        var transaction = _mapper.Map<TransactionHistory>(transactionHistoryDTO);
        await _transactionHistoryRepository.Remove(transaction);
    }

    public async Task<IEnumerable<TransactionHistoryDTO>> GetTransactionHistory()
    {
        var transactionHistory = await _transactionHistoryRepository.GetTransactionHistory();
        return _mapper.Map<IEnumerable<TransactionHistoryDTO>>(transactionHistory);
    }

    public async Task<IEnumerable<TransactionHistoryDTO>> GetTransactionHistoryByCategory(int categoryId)
    {
        var transactionHistory = await _transactionHistoryRepository.GetTransactionHistoryByCategory(categoryId);
        return _mapper.Map<IEnumerable<TransactionHistoryDTO>>(transactionHistory);
    }

    public async Task<TransactionHistoryDTO> GetTransactionHistoryById(int id)
    {
        var transaction = await _transactionHistoryRepository.GetTransactionHistoryById(id);
        return _mapper.Map<TransactionHistoryDTO>(transaction);
    }

    public async Task<IEnumerable<TransactionHistoryDTO>> GetTransactionHistoryByDate(DateTime date)
    {
        var transactionHistory = await _transactionHistoryRepository.GetTransactionByDate(date);
        return _mapper.Map<IEnumerable<TransactionHistoryDTO>>(transactionHistory);
    }

    public async Task Update(TransactionHistoryDTO transactionHistoryDTO)
    {
        var transaction = _mapper.Map<TransactionHistory>(transactionHistoryDTO);
        await _transactionHistoryRepository.Update(transaction);
    }
}
