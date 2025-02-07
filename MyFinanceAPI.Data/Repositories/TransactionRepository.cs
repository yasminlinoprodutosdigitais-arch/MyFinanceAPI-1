using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TransactionRepository(ContextDB context) : ITransactionRepository
{
    private readonly ContextDB _context = context;

    public async Task<Transaction> Create(Transaction transaction)
    {
        var existingAccount = _context.Accounts.Any(a => a.Id == transaction.IdAccount && a.UserId == transaction.UserId);

        if (!existingAccount)
        {
            throw new ArgumentException("Account not existing", nameof(transaction));
        }

        DateTime localDate = transaction.Date;
        if (localDate.Kind == DateTimeKind.Unspecified)
        {
            localDate = DateTime.SpecifyKind(localDate, DateTimeKind.Local);  // Ou UTC, se for o caso
        }

        DateTime utcDate = localDate.ToUniversalTime();
        transaction.Date = utcDate;


        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }


    public async Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime, int userId)
    {
        return await _context.Transactions
            .Where(c => c.UserId == userId && c.Date.Month == dateTime.Month && c.Date.Year == dateTime.Year)
            .ToListAsync();
    }

    public async Task<List<AccountGrouping>> GetTransactions(int userId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Include(a => a.Category)             // Inclui a Categoria
            .Include(a => a.Transactions)       // Inclui os MonthlyUpdates associados
            .OrderBy(a => a.Category.Name)        // Ordena pelas categorias
            .ToListAsync();                       // Carrega todos os dados necessários

        var groupedAccounts = accounts
            .GroupBy(a => a.Category)  // Agrupar as contas por categoria
            .Select(group => new AccountGrouping
            (
                group.Key.Id,          // ID da categoria
                group.Key.Name,        // Nome da categoria
                group.Key.SubCategory, // Subcategoria da categoria
                group.Select(a => new Account
                {
                    Id = a.Id,
                    Name = a.Name,
                    Value = a.Value,
                    Transactions = a.Transactions.Select(m => new Transaction
                    {
                        Id = m.Id,
                        Date = m.Date,
                        Name = m.Name,
                        IdAccount = m.IdAccount,
                        Value = m.Value,
                        Status = m.Status
                    }).ToList() // Para cada conta, adicionar os detalhes de todos os MonthlyUpdates
                }).ToList() // Lista de contas agrupadas por categoria
            ))
            .ToList();  // Realiza a execução e retorna a lista agrupada

        return groupedAccounts;
    }


    public async Task<List<AccountGrouping>> GetTransactionGroupingByDate(DateTime dateTime, int userId)
    {
        var accounts = await _context.Accounts
        .Where(a => a.UserId == userId)
            .Include(a => a.Category)             // Inclui a Categoria
            .Include(a => a.Transactions)       // Inclui os Transactions associados
            .OrderBy(a => a.Category.Name)        // Ordena pelas categorias
            .ToListAsync();                       // Carrega todos os dados necessários

        var groupedAccounts = accounts
            .GroupBy(a => a.Category)  // Agrupar as contas por categoria
            .Select(group => new AccountGrouping
            (
                group.Key.Id,          // ID da categoria
                group.Key.Name,        // Nome da categorias
                group.Key.SubCategory, // Subcategoria da categoria
                group
                .Select(a => new Account
                {
                    Id = a.Id,
                    Name = a.Name,
                    Value = a.Value,
                    Transactions = a.Transactions
                    .Where(m => m.UserId == userId && m.Date.Month == dateTime.Month && m.Date.Year == dateTime.Year)
                    .Select(m => new Transaction
                    {
                        Id = m.Id,
                        Date = m.Date,
                        Name = m.Name,
                        Value = m.Value,
                        Status = m.Status
                    }).ToList() // Para cada conta, adicionar os detalhes de todos os Transactions
                }).ToList() // Lista de contas agrupadas por categoria
            ))
            .ToList();  // Realiza a execução e retorna a lista agrupada

        return groupedAccounts;
    }


    // public Task<IEnumerable<Transaction>> GetTransactionByCategory(int categoryid, int userId)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<Transaction> GetTransactionById(int id, int userId)
    {
        var transaction = await _context.Transactions
            .Where(t => t.UserId == userId && t.Id == id)
            .FirstOrDefaultAsync();

        if(transaction == null)
            throw new Exception("Nenhuma transação encontrada");

        return transaction;
    }

    public async Task<Transaction> Remove(int id, int userId)
    {
        var update = await GetTransactionById(id, userId);
        if (update != null)
        {
            _context.Transactions.Remove(update);
            _context.SaveChanges();
        }

        return update;
    }

    public async Task<Transaction> Update(Transaction transaction, int userId)
    {
        var existingTransaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transaction.Id) ?? throw new Exception("Update not found");

        existingTransaction.Date = transaction.Date;
        existingTransaction.Name = transaction.Name;
        existingTransaction.Value = transaction.Value;
        existingTransaction.Status = transaction.Status;

        await _context.SaveChangesAsync();

        return existingTransaction;
    }
}
