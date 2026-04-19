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

    public async Task<AccountDTO> Add(AccountDTO accountDTO, int userId)
    {
        var account = _mapper.Map<Account>(accountDTO);
        account.UserId = userId;

        var conta = await _accountRepository.Create(account);

        var vencimentos = accountDTO.DataOperacao.Select(d => new ContaVencimento
        {
            ContaId = conta.Id,
            Dia = d,
            UserId = userId 
        }).ToList();

        await _accountRepository.CreateContaVencimento(vencimentos);
        return _mapper.Map<AccountDTO>(conta);
    }

    public async Task Update(AccountDTO accountDTO, int userId)
    {
        var account = _mapper.Map<Account>(accountDTO);

        await _accountRepository.Update(account, userId);

        await _accountRepository.RemoveContaVencimento(account.Id, userId);

        // 🔥 adicionar novos
        var vencimentos = accountDTO.DataOperacao.Select(d => new ContaVencimento
        {
            ContaId = account.Id,
            Dia = d,
            UserId = userId
        }).ToList();

        await _accountRepository.CreateContaVencimento(vencimentos);
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

    public async Task<IEnumerable<AccountDTO>> GetContasAtivas(int userId)
    {
        var accounts = await _accountRepository.GetContasAtivas(userId);
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
