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
            services.AddDbContext<ContextDB>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


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
            services.AddScoped<IUserContextService, UserContextService>();

            services.AddScoped<IBancoService, BancoService>();
            services.AddScoped<ITipoCartaoService, TipoCartaoService>();
            services.AddScoped<IMovimentacaoDiariaService, MovimentacaoDiariaService>();
            services.AddScoped<ITipoMovimentacaoService, TipoMovimentacaoService>();
            services.AddScoped<IItemListaService, ItemListaService>();
            services.AddScoped<IListaService, ListaService>();  

            // Registro do repositório IUsuarioRepository
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Repositórios adicionais
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            
            services.AddScoped<IBancoRepository, BancoRepository>();
            services.AddScoped<ITipoCartaoRepository, TipoCartaoRepository>();
            services.AddScoped<IMovimentacaoDiariaRepository, MovimentacaoDiariaRepository>();
            services.AddScoped<ITipoMovimentacaoRepository, TipoMovimentacaoRepository>();
            services.AddScoped<IItemListaRepository, ItemListaRepository>();
            services.AddScoped<IListaRepository, ListaRepository>();

            // Configuração do AutoMapper
            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            return services;
        }
    }

}
