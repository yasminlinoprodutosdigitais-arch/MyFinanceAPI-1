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

        if(existingCategory == null)
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

        if(accounts == null)
            throw new KeyNotFoundException("Categoria não encontrada");

        return accounts;
    }

    public async Task<Account> Remove(int id, int userId)
    {
        var account = await GetAccountById(id, userId);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        return account;
    }

    public async Task<Account> Update(Account account, int userId)
    {
        // Carrega a categoria correspondente, se o categoryId for fornecido
        if (account.Category != null && account.Category.Id != 0 && account.UserId == userId)
        {
            var category = await _context.Categories.FindAsync(account.Category.Id);
            if (category == null)
            {
                throw new Exception("Categoria não encontrada.");
            }
            account.Category = category; // Associa a categoria carregada
        }

        _context.Accounts.Update(account);  // Atualiza a conta
        await _context.SaveChangesAsync();  // Salva as mudanças no banco

        return account;
    }



    // public async Task<IEnumerable<Account>> GetAccountByCategory(int categoryId)
    // {
    //     var accounts = await _accountCollection.Find(c => c.CategoryId == categoryId).ToListAsync();
    //     return accounts;
    // }

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
