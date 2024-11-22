using System;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TransactionService : ITransactionService
{
    private ITransactionRepository _transactionRepository;
    private IMapper _mapper;


    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task Add(TransactionDTO transactionDTO)
    {
        var transaction = _mapper.Map<Transaction>(transactionDTO);
        await _transactionRepository.Create(transaction);
    }

    public async Task Update(TransactionDTO transactionDTO)
    {
        var transaction = _mapper.Map<Transaction>(transactionDTO);
        await _transactionRepository.Update(transaction);
    }

    public async Task Remove(TransactionDTO transactionDTO)
    {
        var transaction = _mapper.Map<Transaction>(transactionDTO);
        await _transactionRepository.Remove(transaction);

    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactions()
    {
        var transactions = await _transactionRepository.GetTransactions();
        return _mapper.Map<IEnumerable<TransactionDTO>>(transactions);
    }

    public async Task<TransactionDTO> GetTransactionById(ObjectId id)
    {
        var transaction = await _transactionRepository.GetTransactionById(id);
        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionByCategory(int categoryId)
    {
        var transactions = await _transactionRepository.GetTransactionByCategory(categoryId);
        return _mapper.Map<IEnumerable<TransactionDTO>>(transactions);
    }

}
