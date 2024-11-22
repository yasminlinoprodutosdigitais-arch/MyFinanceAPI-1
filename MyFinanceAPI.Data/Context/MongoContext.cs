using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace MyFinanceAPI.Data.Context;
public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDbSettings");

        var connectionString = configuration.GetSection("MongoDbSettings:ConnectionString").Value;
        var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;

        if (string.IsNullOrEmpty(databaseName) || databaseName.Contains("."))
        {
            throw new ArgumentException("Database name cannot be empty or contain '.'", nameof(databaseName));
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is missing.");
        }

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    // Implementando o método GetCollection<T>
    public IMongoCollection<T> GetCollection<T>(string nomeCollection)
    {
        if (string.IsNullOrEmpty(nomeCollection))
        {
            throw new ArgumentException("Collection name cannot be null or empty.", nameof(nomeCollection));
        }
        return _database.GetCollection<T>(nomeCollection);
    }
}
