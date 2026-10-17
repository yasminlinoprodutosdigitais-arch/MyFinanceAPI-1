using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Ioc; // << importante

var builder = WebApplication.CreateBuilder(args);


// ----- Hospedagem (BE-P1-10): o Cloud Run informa a porta em PORT; no local vale o launchSettings.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// O TLS termina no proxy do Cloud Run: o esquema e o IP originais chegam nos X-Forwarded-*.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownNetworks.Clear(); // o proxy do Cloud Run não tem IP fixo conhecido
    o.KnownProxies.Clear();
});

// ----- IoC: registra DbContext (DefaultConnection), CORS (Cors__AllowedOrigins),
// AutoMapper, Services (ITokenService etc.), Repositories (IUsuarioRepository etc.)
builder.Services.RegisterService(builder.Configuration, builder.Environment.IsDevelopment());

// ----- App settings / secrets
// Segredos NÃO ficam no appsettings.json versionado (BE-P0-01):
//   dev  → dotnet user-secrets (carregado automaticamente em Development)
//   prod → variáveis de ambiente AppSettings__SecretKey e ConnectionStrings__DefaultConnection
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("AppSettings"));
var secretKey = builder.Configuration.GetSection("AppSettings")["SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException(
        "AppSettings:SecretKey não configurada. Use 'dotnet user-secrets set' em dev " +
        "ou a variável de ambiente AppSettings__SecretKey em produção.");
if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection não configurada. Use 'dotnet user-secrets set' em dev " +
        "ou a variável de ambiente ConnectionStrings__DefaultConnection em produção.");

// ----- Identity / Auth
builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<MyFinanceAPI.Data.Context.ContextDB>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(o => o.AddPolicy("Admin", p => p.RequireRole("Admin", "Empresa")));

builder.Services.AddHealthChecks();

// Erros num lugar só, sempre { message } e sem stack trace (BE-P2-01).
builder.Services.AddExceptionHandler<ErroHandler>();
builder.Services.AddProblemDetails();

// Corpo inválido também sai como { message }, não como ProblemDetails (BE-P3-05).
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ModeloInvalido.Resposta);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyFinance API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
});

var app = builder.Build();

// Primeiro do pipeline: envolve tudo. Os cabeçalhos de CORS sobrevivem (CorsMiddleware usa OnStarting).
app.UseExceptionHandler();

app.UseForwardedHeaders();

// ----- Swagger em Dev OU via flag
var enableSwagger = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled")
    || builder.Configuration.GetValue<bool>("Swagger__Enabled");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyFinance API v1"));
}

// ----- CORS: policy do IoC, só com as origens de Cors__AllowedOrigins. O middleware já
// responde o preflight (OPTIONS). Sem UseHttpsRedirection: o Cloud Run força HTTPS na borda.
app.UseCors(DependencyInjection.CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 2) expor SEM auth
app.UseHealthChecks("/healthz");

// opcional para teste rápido:
app.MapGet("/", () => Results.Ok("MyFinance API live v3")).AllowAnonymous();

app.Run();
