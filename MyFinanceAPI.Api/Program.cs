using Microsoft.Extensions.DependencyInjection;
using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Ioc;  // Certifique-se de que este namespace esteja correto

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Registra os serviços personalizados de DI
builder.Services.AddControllers(); // Para as controllers

// Registra todos os serviços definidos no DependencyInjection
builder.Services.RegisterService(builder.Configuration); // Aqui estamos chamando o método RegisterService

// Registra o Swagger para a API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Gera a documentação Swagger
    app.UseSwaggerUI();  // Exibe a interface Swagger
}

app.UseHttpsRedirection();  // Redireciona para HTTPS
app.MapControllers();  // Mapeia as rotas dos controladores

app.Run();
