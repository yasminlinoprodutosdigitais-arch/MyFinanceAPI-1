using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Ioc;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o AutoMapper ao DI e escaneia os assemblies em busca de perfis de mapeamento
builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

// Registra outros serviços
builder.Services.AddControllers();

builder.Services.AddDbContext<ContextDB>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")?? throw new Exception("Erro ao buscar string de conexão com o banco")));

// Registra os serviços personalizados definidos no DependencyInjection
builder.Services.RegisterService(builder.Configuration); // Este método deve registrar outros serviços

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
