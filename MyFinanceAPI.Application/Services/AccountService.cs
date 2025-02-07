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

    public async Task Add(AccountDTO accountDTO, int userId)
    {
        var account = _mapper.Map<Account>(accountDTO);
        account.UserId = userId;
        await _accountRepository.Create(account);
    }

    public async Task Update(AccountDTO accountDTO, int userId)
    {
        var account = _mapper.Map<Account>(accountDTO);
        await _accountRepository.Update(account, userId);
    }

    public async Task Remove(int id, int userId)
    {
        await _accountRepository.Remove(id, userId);
    }

    public async Task<IEnumerable<AccountDTO>> GetAccounts(int userId)
    {
        var accounts = await _accountRepository.GetAccounts(userId);
        return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
    }

    public async Task<AccountDTO> GetAccountById(int id, int userId)
    {
        var account = await _accountRepository.GetAccountById(id, userId);
        return _mapper.Map<AccountDTO>(account);
    }

    public async Task<IEnumerable<AccountDTO>> GetAccountByCategory(int categoryId, int userId)
    {
        var accounts = await _accountRepository.GetAccountByCategory(categoryId, userId);
        return _mapper.Map<IEnumerable<AccountDTO>>(accounts);
    }

}
