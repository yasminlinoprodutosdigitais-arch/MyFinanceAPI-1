using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _transactionCollection;

    public TransactionRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _transactionCollection = database.GetCollection<Transaction>("Transactions");
    }
    public async Task<Transaction> Create(Transaction transaction)
    {
        var existingCategory = await _transactionCollection.Find(c => c.Id == 0).FirstOrDefaultAsync();
        if (existingCategory != null)
        {
            var maxCategory = await _transactionCollection.Find(Builders<Transaction>.Filter.Empty)
            .SortByDescending(c => c.Id)
            .FirstOrDefaultAsync();
            transaction.Id = maxCategory?.Id + 1 ?? 1;
        }

        // Caso contrário, insira o novo documento
        await _transactionCollection.InsertOneAsync(transaction);
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByCategory(int categoryId)
    {
        var transactions = await _transactionCollection.Find(c => c.CategoryId == categoryId).ToListAsync();
        return transactions;
    }

    public async Task<Transaction> GetTransactionById(int id)
    {
        var transaction = await _transactionCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactions()
    {
        var transactions = await _transactionCollection.Find(c => true).ToListAsync();
        return transactions;
    }

    public async Task<Transaction> Remove(Transaction transaction)
    {
        await _transactionCollection.DeleteOneAsync(c => c.Id == transaction.Id);
        return transaction;
    }

    public async Task<Transaction> Update(Transaction transaction)
    {
        await _transactionCollection.ReplaceOneAsync(c => c.Id == transaction.Id, transaction);
        return transaction;
    }
}
