using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Ioc;

var builder = WebApplication.CreateBuilder(args);

// Registra os serviços personalizados definidos no DependencyInjection
builder.Services.RegisterService(builder.Configuration); // Este método deve registrar outros serviços

builder.Services.AddInfrastructureJWT(builder.Configuration); // Adiciona a configuração do JWT

builder.Services.AddInfrastructureSwagger(); // Adiciona a configuração do Swagger

builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

// Registra outros serviços
builder.Services.AddDbContext<ContextDB>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ContextDB>()
    .AddDefaultTokenProviders();

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

// Habilita Swagger se estiver em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configurações de CORS
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Ordem correta dos middlewares
app.UseRouting();         // Roteia as requisições
app.UseAuthentication();  // Ativa o middleware de autenticação (verifica o token)
app.UseAuthorization();   // Ativa o middleware de autorização (verifica se o usuário tem permissão)
app.MapControllers();

app.Run();
