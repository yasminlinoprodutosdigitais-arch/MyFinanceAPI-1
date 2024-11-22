var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço de controladores
builder.Services.AddControllers();

// Se estiver usando o Swagger para documentar a API
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Gera a documentação Swagger
    app.UseSwaggerUI(); // Exibe a interface Swagger
}

app.UseHttpsRedirection(); // Redireciona para HTTPS

app.MapControllers(); // Mapeia os controladores para as rotas

app.Run();
