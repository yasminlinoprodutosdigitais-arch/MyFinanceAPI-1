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
        DateTime localDate = transaction.Date; // Supondo que esta data seja local ou "Unspecified"
        DateTime utcDate = localDate.ToUniversalTime(); // Converte para UTC
        transaction.Date = utcDate;

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }


    public async Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime)
    {
        return await _context.Transactions
            .Where(c => c.Date.Month == dateTime.Month && c.Date.Year == dateTime.Year)
            .ToListAsync();
    }

    public async Task<List<AccountGrouping>> GetTransactions()
    {
        var accounts = await _context.Accounts
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


    public async Task<List<AccountGrouping>> GetTransactionGroupingByDate(DateTime dateTime)
    {
        var accounts = await _context.Accounts
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
                    .Where(m => m.Date.Month == dateTime.Month && m.Date.Year == dateTime.Year)
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


    public Task<IEnumerable<Transaction>> GetTransactionByCategory(int categoryid)
    {
        throw new NotImplementedException();
    }

    public async Task<Transaction> GetTransactionById(int id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Transaction> Remove(int id)
    {
        var update = await GetTransactionById(id);
        if (update != null)
        {
            _context.Transactions.Remove(update);
            _context.SaveChanges();
        }

        return update;
    }

    public async Task<Transaction> Update(Transaction transaction)
    {
        // Primeiro, obtenha o Transaction existente pelo ID
        var existingTransaction = await _context.Transactions
            .FirstOrDefaultAsync(m => m.Id == transaction.Id);

        // Se não encontrar, lança uma exceção
        if (existingTransaction == null)
        {
            throw new Exception("Update not found");
        }

        // Atualize as propriedades desejadas
        existingTransaction.Date = transaction.Date;
        existingTransaction.Name = transaction.Name;
        existingTransaction.Value = transaction.Value;
        existingTransaction.Status = transaction.Status;  // Certifique-se de atualizar todas as propriedades necessárias

        // Salve as mudanças no banco de dados
        await _context.SaveChangesAsync();

        return existingTransaction;
    }


    // public async Task<IEnumerable<Transaction>> GetAccountByDate(DateTime dateTime)
    // {
    //     var dateMounth = dateTime.Month;
    //     var dateYear = dateTime.Year;

    //     var history = await _context
    //                     .Find(t => t.Date.Month == dateMounth
    //                         && t.Date.Year == dateYear)
    //                     .ToListAsync();

    //     return history;

    // }

    // public async Task<IEnumerable<Transaction>> GetTransaction()
    // {
    //     return await _context.Find(h => true).ToListAsync();
    // }

    // public async Task<IEnumerable<Transaction>> GetTransactionByCategory(int idCategory)
    // {
    //     return await _context.Find(c => c.IdCategory == idCategory).ToListAsync();

    // }

    // public async Task<Transaction> GetTransactionById(int id)
    // {
    //     return await _context.Find(t => t.Id == id).FirstOrDefaultAsync();
    // }

    // public async Task<Transaction> Remove(int id)
    // {
    //     var account = await _context.FindOneAndDeleteAsync(c => c.Id == id);
    //     return account;
    // }

    // public async Task<Transaction> Update(Transaction Transaction)
    // {
    //     await _context.ReplaceOneAsync(t => t.Id == Transaction.Id, Transaction);
    //     return Transaction;
    // }
}
