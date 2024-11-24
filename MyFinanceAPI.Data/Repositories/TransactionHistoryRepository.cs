using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TransactionHistoryRepository : ITransactionHistoryRepository
{
    private readonly IMongoCollection<TransactionHistory> _historyCollection;

    public TransactionHistoryRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _historyCollection = database.GetCollection<TransactionHistory>("TransactionHistory");
    }
    public async Task<TransactionHistory> Create(TransactionHistory transactionHistory)
    {
        var existingCategory = await _historyCollection.Find(c => c.Id == 0).FirstOrDefaultAsync();
        if (existingCategory != null)
        {
            var maxCategory = await _historyCollection.Find(Builders<TransactionHistory>.Filter.Empty)
            .SortByDescending(c => c.Id)
            .FirstOrDefaultAsync();
            transactionHistory.Id = maxCategory?.Id + 1 ?? 1;
        }

        await _historyCollection.InsertOneAsync(transactionHistory);
        return transactionHistory;
    }

    public async Task<IEnumerable<TransactionHistory>> GetTransactionByDate(DateTime dateTime)
    {
        var dateMounth = dateTime.Month;
        var dateYear = dateTime.Year;
    
        var history = await _historyCollection
                        .Find(t => t.Date.Month == dateMounth 
                            && t.Date.Year == dateYear)
                        .ToListAsync();

        return history;

    }

    public async Task<IEnumerable<TransactionHistory>> GetTransactionHistory()
    {
        return await _historyCollection.Find(h => true).ToListAsync();
    }

    public async Task<IEnumerable<TransactionHistory>> GetTransactionHistoryByCategory(int idCategory)
    {
        return await _historyCollection.Find(c => c.IdCategory == idCategory).ToListAsync();
        
    }

    public async Task<TransactionHistory> GetTransactionHistoryById(int id)
    {
        return await _historyCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<TransactionHistory> Remove(TransactionHistory transactionHistory)
    {
        await _historyCollection.DeleteOneAsync(t => t.Id == transactionHistory.Id);
        return transactionHistory;
    }

    public async Task<TransactionHistory> Update(TransactionHistory transactionHistory)
    {
        await _historyCollection.ReplaceOneAsync(t => t.Id == transactionHistory.Id, transactionHistory);
        return transactionHistory;
    }
}
