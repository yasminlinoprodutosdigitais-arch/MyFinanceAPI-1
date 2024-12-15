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

    public async  Task<Account> GetAccountByCategory(int categoryId)
    {
        return await _context.Accounts.FirstOrDefaultAsync(c => c.Category.Id == categoryId);
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
        // Carrega a categoria correspondente, se o categoryId for fornecido
        if (account.Category != null && account.Category.Id != 0)
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

    public async Task<Account?> GetAccountById(int id)
    {
        return await _context.Accounts.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Account>>? GetAccounts()
    {
        var accounts = await _context.Accounts
            .Include(a => a.Category)  // Incluindo a categoria nas contas
            .OrderBy(c => c.Name)
            .ToListAsync();

        return accounts;
    }

}
