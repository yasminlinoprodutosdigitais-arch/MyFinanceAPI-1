using MyFinanceAPI.Application.Mapping;
using MyFinanceAPI.Ioc;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o AutoMapper ao DI e escaneia os assemblies em busca de perfis de mapeamento
builder.Services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

// Registra outros serviços
builder.Services.AddControllers();

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
