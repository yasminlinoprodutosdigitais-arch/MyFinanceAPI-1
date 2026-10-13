using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TransactionRepository(ContextDB context) : ITransactionRepository
{
    private readonly ContextDB _context = context;

    // A data é de calendário (date): grava como veio. A conversão pelo fuso do servidor que havia
    // aqui gravava meia-noite UTC com a API hospedada em UTC — o dia aparecia um antes (DB-P1-02).
    public async Task Create(List<Transaction> transactions)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var dia = DateOnly.FromDateTime(dateTime);
        var a = await _context.Transactions
            .Where(c => c.UserId == userId && (pesquisaDataCompleta ? c.Date == dia : c.Date.Month == dia.Month && c.Date.Year == dia.Year))
            .ToListAsync();
        return a;
    }

    public async Task<IEnumerable<Transaction>> GetContaVencida(DateTime hoje, int userId)
    {
        var dataHoje = DateOnly.FromDateTime(hoje);
        var a = await _context.Transactions
            .Where(c => c.UserId == userId
                && c.Date < dataHoje
                && c.Status != StatusTransacao.PagoNoPrazo
                && c.Status != StatusTransacao.PagoAtrasado)
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
        var dia = DateOnly.FromDateTime(dateTime);

        // 1) Carrega transações do mês (com ou sem conta)
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && (pesquisaDataCompleta ? t.Date == dia : t.Date.Month == month && t.Date.Year == year))
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

    public async Task<List<Transaction>> BuscarContasMensais(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var month = dateTime.Month;
        var year = dateTime.Year;
        var dia = DateOnly.FromDateTime(dateTime);

        // 1) Carrega transações do mês (com ou sem conta)
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && (pesquisaDataCompleta ? t.Date == dia : t.Date.Month == month && t.Date.Year == year))
            .Select(t => new Transaction
            {
                Id = t.Id,
                Date = t.Date,
                Name = t.Name,
                Value = t.Value,
                Status = t.Status,

                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                SubCategoryName = t.Category.SubCategory,
                
                IdAccount = t.IdAccount,
                AccountName = t.Account.Name,

                EhParcelado = t.EhParcelado,
                ParcelaAtual = t.ParcelaAtual,
                QuantidadeParcelas = t.QuantidadeParcelas,
                Observacao = t.Observacao,
            })
            .ToListAsync();

        return transactions;
    }

    public async Task<Transaction> GetTransactionById(int id, int userId)
    {
        var transaction = await _context.Transactions
            .Where(t => t.UserId == userId && t.Id == id)
            .FirstOrDefaultAsync();

        if (transaction == null)
            throw new NaoEncontradoException("Transação não encontrada.");

        return transaction;
    }

    public async Task<List<int>> GetParcelasLancadas(int accountId, int userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId && t.IdAccount == accountId && t.ParcelaAtual != null)
            .Select(t => t.ParcelaAtual!.Value)
            .Distinct()
            .ToListAsync();
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
            throw new NaoEncontradoException("Transação não encontrada.");

        return update;
    }

    public async Task<Transaction> Update(Transaction transaction, int userId)
    {
        var existingTransaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transaction.Id) ?? throw new NaoEncontradoException("Transação não encontrada.");

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
