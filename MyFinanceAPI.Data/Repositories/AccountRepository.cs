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
        var existingCategory = await _context.Categories
         .Where(c => c.UserId == account.UserId && c.Id == account.CategoryId).FirstOrDefaultAsync();

        if (existingCategory == null)
            throw new KeyNotFoundException("Categoria não encontrada");

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<List<Account>>? GetAccountByCategory(int categoryId, int userId)
    {
        var accounts = await _context.Accounts
            .Where(c => c.UserId == userId && c.Category.Id == categoryId)
            .Include(a => a.Category)
            .OrderBy(c => c.Name)
            .ToListAsync();

        if (accounts == null)
            throw new KeyNotFoundException("Categoria não encontrada");

        return accounts;
    }

    // Local: MyFinanceAPI/MyFinanceAPI.Data/Repositories/AccountRepository.cs

    public async Task Remove(int id, int userId) // O método recebe id e userId
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (account != null)
        {
            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync();
        }
    }


    public async Task<Account> Update(Account incomingAccount, int userId)
    {
        var existingAccount = await _context.Accounts
            .Include(a => a.Category) 
            .FirstOrDefaultAsync(a => a.Id == incomingAccount.Id && a.UserId == userId);

        if (existingAccount == null)
        {
            throw new Exception("Conta não encontrada ou não pertence ao usuário.");
        }

        existingAccount.Name = incomingAccount.Name;
        existingAccount.Value = incomingAccount.Value;
        existingAccount.DataOperacao = incomingAccount.DataOperacao; 
        existingAccount.CategoryId = incomingAccount.CategoryId;

        await _context.SaveChangesAsync();

        return existingAccount;
    }
    public async Task<IEnumerable<Account>> GetAccountsByCategory(int categoryId, int userId)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId && a.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Account?> GetAccountById(int id, int userId)
    {
        var account = await _context.Accounts
            .Where(c => c.UserId == userId && c.Id == id)
            .FirstOrDefaultAsync();
        return account;
    }

    public async Task<List<Account>>? GetAccounts(int userId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Include(a => a.Category)  // Incluindo a categoria nas contas
            .OrderBy(c => c.Name)
            .ToListAsync();

        return accounts;
    }

}
