using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Interfaces;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Application.Mapping;
using Microsoft.EntityFrameworkCore;

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

            // Registrar os serviços
            services.AddScoped<IValidadorToken, ValidadorToken>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IAccountService, AccountService>();

            // Registro do repositório IUsuarioRepository
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Repositórios adicionais
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            // Configuração do AutoMapper
            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            return services;
        }
    }

}
