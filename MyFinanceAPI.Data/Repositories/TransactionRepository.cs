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

    // public async Task<Transaction> Create(Transaction transaction)
    // {
    //     if (transaction.IdAccount != 0)
    //     {
    //         var existingAccount = _context.Accounts.Any(a => a.Id == transaction.IdAccount && a.UserId == transaction.UserId);

    //         if (!existingAccount)
    //         {
    //             throw new ArgumentException("Account not existing", nameof(transaction));
    //         }
    //     }
    //     else if (transaction.CategoryId != 0)
    //     {
    //         var existingCategory = _context.Categories.Any(c => c.Id == transaction.CategoryId && c.UserId == transaction.UserId);
    //         transaction.IdAccount = null;
    //         if (!existingCategory)
    //         {
    //             throw new ArgumentException("Category not existing", nameof(transaction));
    //         }
    //     }


    //     DateTime localDate = transaction.Date;
    //     if (localDate.Kind == DateTimeKind.Unspecified)
    //     {
    //         localDate = DateTime.SpecifyKind(localDate, DateTimeKind.Local);  // Ou UTC, se for o caso
    //     }

    //     DateTime utcDate = localDate.ToUniversalTime();
    //     transaction.Date = utcDate;


    //     await _context.Transactions.AddAsync(transaction);
    //     await _context.SaveChangesAsync();
    //     return transaction;
    // }

    public async Task Create(List<Transaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            DateTime localDate = transaction.Date;
            if (localDate.Kind == DateTimeKind.Unspecified)
                localDate = DateTime.SpecifyKind(localDate, DateTimeKind.Local);

            transaction.Date = localDate.ToUniversalTime();
        }

        try
        {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();

        }
        catch (Exception e)
        {

        }
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var a = await _context.Transactions
            .Where(c => c.UserId == userId && (pesquisaDataCompleta ? c.Date.Date == dateTime.Date : c.Date.Month == dateTime.Month && c.Date.Year == dateTime.Year))
            .ToListAsync();
        return a;
    }

    public async Task<IEnumerable<Transaction>> GetContaVencida(DateTime hoje, int userId)
    {
        var a = await _context.Transactions
            .Where(c => c.UserId == userId
                && c.Date.Date < hoje.Date
                && c.Status != "PAGO NO PRAZO"
                && c.Status != "PAGO ATRASADO")
            .ToListAsync();

        return a;
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
                group.Key.NaturezaOperacao, // Subcategoria da categoria
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


    public async Task<List<Transaction>> GetTransactionGroupingByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var month = dateTime.Month;
        var year = dateTime.Year;

        // 1) Carrega transações do mês (com ou sem conta)
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && (pesquisaDataCompleta ? t.Date == dateTime : t.Date.Month == month && t.Date.Year == year))
            .Select(t => new Transaction
            {
                Id = t.Id,
                Date = t.Date,
                Name = t.Name,
                Value = t.Value,
                Status = t.Status,
                CategoryId = t.CategoryId,
                IdAccount = t.IdAccount,
                EhParcelado = t.EhParcelado,
                ParcelaAtual = t.ParcelaAtual,
                QuantidadeParcelas = t.QuantidadeParcelas,
                Observacao = t.Observacao,


                Account = t.IdAccount == null ? null : new Account
                {
                    Id = t.Account.Id,
                    Name = t.Account.Name,
                    Value = t.Account.Value,
                    CategoryId = t.Account.CategoryId,
                    Transactions = t.Account.Transactions.Select(m => new Transaction
                    {
                        Id = m.Id,
                        Date = m.Date,
                        Name = m.Name,
                        Value = m.Value,
                        Status = m.Status
                    }).ToList()
                },
                Category = t.CategoryId == null ? null : new Category
                {
                    Id = t.Account.CategoryId,
                    SubCategory = t.Account.Category.SubCategory,
                    NaturezaOperacao = t.Account.Category.NaturezaOperacao,
                    Status = t.Account.Category.Status
                }

            })
            .ToListAsync();

        return transactions;
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

        if (transaction == null)
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
        else
            throw new Exception("Nenhuma transação encontrada");

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
        existingTransaction.EhParcelado = transaction.EhParcelado;
        existingTransaction.ParcelaAtual = transaction.ParcelaAtual;
        existingTransaction.CategoryId = transaction.CategoryId;
        existingTransaction.QuantidadeParcelas = transaction.QuantidadeParcelas;
        existingTransaction.Observacao = transaction.Observacao;


        await _context.SaveChangesAsync();

        return existingTransaction;
    }

}
