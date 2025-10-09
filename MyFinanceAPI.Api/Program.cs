using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Ioc;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// ================================
//  Connection String (Neon)
//  Cloud Run: defina ConnectionStrings__Default
// ================================
var connString = builder.Configuration.GetConnectionString("Default")
                 ?? throw new Exception("ConnectionStrings:Default não configurada.");

// Npgsql ajustes (timestamp legacy evita warning em migrações antigas)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Se seu RegisterService JÁ registra o DbContext, remova o bloco AddDbContextPool abaixo.
builder.Services.AddDbContextPool<ContextDB>(opt =>
{
    opt.UseNpgsql(connString, npg => npg.EnableRetryOnFailure());
});

// ================================
//  Serviços da aplicação / IoC
// ================================
builder.Services.RegisterService(builder.Configuration);

// ================================
//  Identity (usuários/roles)
// ================================
builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<ContextDB>()
    .AddDefaultTokenProviders();

// ================================
//  JWT (chave via ENV em produção)
//  Cloud Run: defina AppSettings__SecretKey
// ================================
var secretKey = builder.Configuration["AppSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    if (env.IsDevelopment())
        secretKey = "dev-secret-apenas-local";
    else
        throw new Exception("AppSettings:SecretKey ausente. Defina a env AppSettings__SecretKey.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;   // em Cloud Run tem HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// ================================
//  AutoMapper / Controllers
// ================================
builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));
builder.Services.AddControllers();

// ================================
//  CORS (origens via env)
//  Cloud Run: defina Cors__AllowedOrigins="https://seu-front.com,https://sua-api-xxxx-uc.a.run.app"
// ================================
var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // libere geral no início, se preferir
    });
});

// ================================
//  Swagger (habilitável por env)
//  Cloud Run: defina Swagger__Enabled=true se quiser usar em prod
// ================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyFinance API", Version = "v1" });

    // Suporte a JWT no Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token no formato: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ================================
//  Pipeline HTTP
// ================================
var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:Enabled");

// Swagger opcional (em dev, ou em prod se Swagger__Enabled=true)
if (env.IsDevelopment() || enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyFinance API v1"));
}

app.UseCors("Default");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// endpoint simples para health-check
app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();
