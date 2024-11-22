using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _database;

    public TransactionRepository(IMongoContext mongoContext)
    {
        _database = mongoContext.GetCollection<Transaction>("Transactions");
    }
    public async Task<Transaction> Create(Transaction transaction)
    {
        var existingTransaction = await _database.Find(c => c.Id == transaction.Id).FirstOrDefaultAsync();
        if(existingTransaction is not null)
            await _database.InsertOneAsync(transaction);
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByCategory(int categoryId)
    {
        var transactions = await _database.Find(c => c.CategoryId == categoryId).ToListAsync();
        return transactions;
    }

    public async Task<Transaction> GetTransactionById(ObjectId id)
    {
        var transaction = await _database.Find(c => c.Id == id).FirstOrDefaultAsync();
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactions()
    {
        var transactions = await _database.Find(c => true).ToListAsync();
        return transactions;
    }

    public async Task<Transaction> Remove(Transaction transaction)
    {
        await _database.DeleteOneAsync(c => c.Id == transaction.Id);
        return transaction;
    }

    public async Task<Transaction> Update(Transaction transaction)
    {
        await _database.ReplaceOneAsync(c => c.Id == transaction.Id, transaction);
        return transaction;
    }
}
