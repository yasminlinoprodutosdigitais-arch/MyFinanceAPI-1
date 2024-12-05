using System;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class AccountService : IAccountService
{
    private IAccountRepository _accountRepository;
    private IMapper _mapper;


    public AccountService(IAccountRepository AccountRepository, IMapper mapper)
    {
        _accountRepository = AccountRepository;
        _mapper = mapper;
    }

    public async Task Add(AccountDTO accountDTO)
    {
        var account = _mapper.Map<Account>(accountDTO);
        await _accountRepository.Create(account);
    }

    public async Task Update(AccountDTO accountDTO)
    {
        var account = _mapper.Map<Account>(accountDTO);
        await _accountRepository.Update(account);
    }

    public async Task Remove(int id)
    {
        await _accountRepository.Remove(id);
    }

    public async Task<IEnumerable<AccountDTO>> GetAccounts()
    {
        var accounts = await _accountRepository.GetAccounts();
        return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
    }

    public async Task<AccountDTO> GetAccountById(int id)
    {
        var account = await _accountRepository.GetAccountById(id);
        return _mapper.Map<AccountDTO>(account);
    }

    public async Task<IEnumerable<AccountDTO>> GetAccountByCategory(int categoryId)
    {
        var accounts = await _accountRepository.GetAccountByCategory(categoryId);
        return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
    }

}
