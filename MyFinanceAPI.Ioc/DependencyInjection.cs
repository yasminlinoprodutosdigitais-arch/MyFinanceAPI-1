using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinanceAPI.Application.Email;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Interfaces;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Application.Mapping;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Ioc
{
    public static class DependencyInjection
    {
        public const string CorsPolicy = "Front";

        /// <summary>
        /// Envio de e-mail (card #50) pela seção "Email": Provedor "Brevo" em produção, arquivo .html no resto.
        /// Brevo sem remetente, chave ou endereço do front não sobe: o erro apareceria só no primeiro cadastro.
        /// </summary>
        public static void RegistrarEmail(IServiceCollection services, IConfiguration configuration)
        {
            var secao = configuration.GetSection("Email");
            services.Configure<EmailSettings>(secao);
            var email = secao.Get<EmailSettings>() ?? new EmailSettings();

            if (!string.Equals(email.Provedor, "Brevo", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<IEnvioEmail, EnvioEmailArquivo>();
                return;
            }

            var faltando = new[]
            {
                (Valor: email.Remetente, Nome: "Email__Remetente"),
                (Valor: email.BrevoApiKey, Nome: "Email__BrevoApiKey"),
                (Valor: email.UrlFront, Nome: "Email__UrlFront"),
            }.Where(c => string.IsNullOrWhiteSpace(c.Valor)).Select(c => c.Nome).ToList();
            if (faltando.Count > 0)
                throw new InvalidOperationException($"Email:Provedor é Brevo, mas falta configurar: {string.Join(", ", faltando)}.");

            services.AddHttpClient<IEnvioEmail, EnvioEmailBrevo>(c => c.Timeout = TimeSpan.FromSeconds(15));
        }

        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration, bool ambienteLocal)
        {
            // Configuração do Entity Framework Core com PostgreSQL
            // DataAlteracaoInterceptor: DataAlteracao/DataCriacao em toda gravação (DB-P1-12). Sem estado: uma instância basta.
            var auditoria = new DataAlteracaoInterceptor();
            services.AddDbContext<ContextDB>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
               .AddInterceptors(auditoria));

            // Transação sobre o mesmo ContextDB que os repositórios usam na requisição (BE-P1-05).
            services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<ContextDB>()));
            services.AddSingleton(TimeProvider.System);

            // CORS só para as origens do front (Cors__AllowedOrigins); sem configuração fora do local a API não sobe (BE-P1-10).
            var origens = CorsOrigins.Ler(configuration, ambienteLocal);
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, builder =>
                    builder.WithOrigins(origens)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           // Total das listas paginadas (BE-P4-16); sem isso o navegador esconde o cabeçalho do front.
                           .WithExposedHeaders("X-Total-Count"));
            });

            RegistrarEmail(services, configuration);

            // Registrar os serviços
            services.AddScoped<IValidadorToken, ValidadorToken>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<ITransacaoService, TransacaoService>();
            services.AddScoped<IContaService, ContaService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();

            services.AddScoped<IBancoService, BancoService>();
            services.AddScoped<ITipoCartaoService, TipoCartaoService>();
            services.AddScoped<IMovimentacaoDiariaService, MovimentacaoDiariaService>();
            services.AddScoped<IExtratoBancarioService, ExtratoBancarioService>();
            services.AddScoped<IExtratoBancarioItemService, ExtratoBancarioItemService>();
            services.AddScoped<IAcertosService, AcertosService>();
            services.AddScoped<ITipoMovimentacaoService, TipoMovimentacaoService>();
            services.AddScoped<IItemListaService, ItemListaService>();
            services.AddScoped<IListaService, ListaService>();  
            services.AddScoped<IPessoaMovimentacaoService, PessoaMovimentacaoService>();
            services.AddScoped<IRegraClassificacaoService, RegraClassificacaoService>();
            services.AddScoped<IClassificacaoService, ClassificacaoService>();
            services.AddScoped<INomesExtratoService, NomesExtratoService>();
            services.AddScoped<IResumoCategoriasService, ResumoCategoriasService>();
            services.AddScoped<IResumoContasService, ResumoContasService>();
            services.AddScoped<IBuscaService, BuscaService>();
            services.AddScoped<INotaFiscalService, NotaFiscalService>();
            services.AddScoped<IPreferenciaNotificacaoService, PreferenciaNotificacaoService>();
            services.AddScoped<INotificacaoService, NotificacaoService>();
            services.AddScoped<IImportacoesService, ImportacoesService>();
            services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
            services.AddScoped<IPreferenciaNotificacaoRepository, PreferenciaNotificacaoRepository>();
            services.AddScoped<ITokenUsuarioRepository, TokenUsuarioRepository>();
            services.AddScoped<ITokensUsuarioService, TokensUsuarioService>();
            services.AddScoped<IAcessoService, AcessoService>();


            // Registro do repositório IUsuarioRepository
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Repositórios adicionais
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();
            services.AddScoped<IContaRepository, ContaRepository>();
            
            services.AddScoped<IBancoRepository, BancoRepository>();
            services.AddScoped<IBuscaRepository, BuscaRepository>();
            services.AddScoped<ITipoCartaoRepository, TipoCartaoRepository>();
            services.AddScoped<IMovimentacaoDiariaRepository, MovimentacaoDiariaRepository>();
            services.AddScoped<IExtratoBancarioRepository, ExtratoBancarioRepository>();
            services.AddScoped<IExtratoBancarioItemRepository, ExtratoBancarioItemRepository>();
            services.AddScoped<ITipoMovimentacaoRepository, TipoMovimentacaoRepository>();
            services.AddScoped<IItemListaRepository, ItemListaRepository>();
            services.AddScoped<IListaRepository, ListaRepository>();
            services.AddScoped<IPessoaMovimentacaoRepository, PessoaMovimentacaoRepository>();
            services.AddScoped<IRegraClassificacaoRepository, RegraClassificacaoRepository>();

            // Configuração do AutoMapper
            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            return services;
        }
    }

}
