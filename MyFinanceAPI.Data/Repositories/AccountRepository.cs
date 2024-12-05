using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class AccountRepository(ContextDB context) : IAccountRepository
{
    private readonly ContextDB _context = context;

    public async Task<Account> Create(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public Task<IEnumerable<Account>> GetAccountByCategory(int categoryid)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> Remove(int id)
    {
        var account = await GetAccountById(id);
        if (account != null)
        { 
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        return account;
    }

    public async Task<Account> Update(Account account)
    {
        _context.Accounts.Update(account);
        _context.SaveChanges();
        return account;
    }


    // public async Task<IEnumerable<Account>> GetAccountByCategory(int categoryId)
    // {
    //     var accounts = await _accountCollection.Find(c => c.CategoryId == categoryId).ToListAsync();
    //     return accounts;
    // }

    public async Task<Account?> GetAccountById(int id)
    {
        return await _context.Accounts.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Account>>? GetAccounts()
    {
        var accounts = await _context.Accounts.OrderBy(c => c.Name).ToListAsync();
        return accounts;
    }

}
