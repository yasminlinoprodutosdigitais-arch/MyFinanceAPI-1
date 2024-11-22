using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Ioc;

public static class DependencyInjection
{
    public static void RegisterService(IServiceCollection services, IConfiguration configuration)
    {
    
        // Registra o contexto do MongoDB, que vai utilizar o banco de dados configurado
        services.AddSingleton<IMongoContext, MongoContext>();

        //services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();

        //repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

    }
}
