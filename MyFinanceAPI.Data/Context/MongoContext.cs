using MongoDB.Driver;

namespace MyFinanceAPI.Data.Context;
public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database), "Mongo database cannot be null");
    }

    public IMongoDatabase Database => _database;

}
