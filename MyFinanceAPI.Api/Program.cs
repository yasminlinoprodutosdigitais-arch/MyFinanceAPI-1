using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Ioc;

var builder = WebApplication.CreateBuilder(args);

// Registra os serviços no contêiner de dependências
builder.Services.RegisterService(builder.Configuration);
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("AppSettings"));

// Verifica se a SecretKey está carregada corretamente
var secretKey = builder.Configuration.GetSection("AppSettings")["SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("Erro: SecretKey não foi carregada do appsettings.json!");
}

// Configuração do Identity
builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<ContextDB>()
    .AddDefaultTokenProviders();

// Configuração do Swagger com suporte a JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyFinance API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    };

    c.AddSecurityRequirement(securityRequirement);
});

// Configuração do JWT Authentication
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options => {
        options.AddPolicy("Admin", policy =>{
            policy.RequireRole("Admin");
        });
    });

// Configuração do AutoMapper
builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

// Adiciona os controladores
builder.Services.AddControllers();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Habilita o Swagger se estiver em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyFinance API v1");
    });
}

// Configuração de Middleware na ordem correta
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
