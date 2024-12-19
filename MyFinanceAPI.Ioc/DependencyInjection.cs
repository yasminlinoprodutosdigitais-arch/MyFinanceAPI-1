using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do Entity Framework Core com PostgreSQL
            services.AddDbContext<ContextDB>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            // Configuração da política CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            // Registro de repositórios e serviços
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            // services.AddScoped<IAuthenticate, AuthenticateService>();

            // Configuração do AutoMapper
            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            return services;
        }
    }
}
