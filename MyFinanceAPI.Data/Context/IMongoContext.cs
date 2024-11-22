using System;
using MongoDB.Driver;

namespace MyFinanceAPI.Data.Context;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>(string nomeCollection);
}
