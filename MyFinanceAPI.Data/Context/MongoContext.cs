using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace MyFinanceAPI.Data.Context;

public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IConfiguration configuration)
    {
        MongoClient mongoClient = new(configuration.GetSection("MongoSettings:ConnectionString").Value);
        _database = mongoClient.GetDatabase(configuration.GetSection("MongoSettings:Database").Value);
    }
    public virtual IMongoCollection<T> GetCollection<T>(string nomeCollection)
    {
        return _database.GetCollection<T>(nomeCollection);
    }
}
