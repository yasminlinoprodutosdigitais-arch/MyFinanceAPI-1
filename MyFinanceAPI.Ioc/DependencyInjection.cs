using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do MongoDB
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            // Registra o MongoClient como Singleton
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);  // Cria o MongoClient com a string de conexão
            });

            // Registra o IMongoDatabase como Scoped
            services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = serviceProvider.GetRequiredService<IMongoClient>();  // Resolve o MongoClient
                return client.GetDatabase(settings.DatabaseName);  // Retorna o banco de dados desejado
            });

            // Registra o MongoContext
            services.AddScoped<MongoContext>();

            // Outros serviços, como repositórios, serviços, etc.
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            return services;
        }
    }
}
