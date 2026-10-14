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
        var a = await _context.Transactions.AsNoTracking()
            .Where(TransactionFiltros.DoDiaOuDoMes(userId, dia, pesquisaDataCompleta))
            .ToListAsync();
        return a;
    }

    public async Task<IEnumerable<Transaction>> GetContaVencida(DateTime hoje, int userId)
    {
        var dataHoje = DateOnly.FromDateTime(hoje);
        var a = await _context.Transactions.AsNoTracking()
            .Where(c => c.UserId == userId
                && c.Date < dataHoje
                && c.Status != StatusTransacao.PagoNoPrazo
                && c.Status != StatusTransacao.PagoAtrasado)
            .ToListAsync();

        return a;
    }

    public async Task<List<Transaction>> BuscarContasMensais(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var dia = DateOnly.FromDateTime(dateTime);

        // 1) Carrega transações do mês (com ou sem conta)
        var transactions = await _context.Transactions
            .Where(TransactionFiltros.DoDiaOuDoMes(userId, dia, pesquisaDataCompleta))
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
