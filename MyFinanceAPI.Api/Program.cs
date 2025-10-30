using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Ioc; // << importante

var builder = WebApplication.CreateBuilder(args);


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ----- IoC: registra DbContext (DefaultConnection), CORS "AllowAll",
// AutoMapper, Services (ITokenService etc.), Repositories (IUsuarioRepository etc.)
builder.Services.RegisterService(builder.Configuration);

// ----- App settings / secrets
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("AppSettings"));
var secretKey = builder.Configuration.GetSection("AppSettings")["SecretKey"]
               ?? throw new Exception("Erro: SecretKey não foi carregada do appsettings.json/ENV!");

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

builder.Services.AddAuthorization(o => o.AddPolicy("Admin", p => p.RequireRole("Admin")));

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
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

// ----- Swagger em Dev OU via flag
var enableSwagger = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled")
    || builder.Configuration.GetValue<bool>("Swagger__Enabled");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyFinance API v1"));
}

// ----- CORS: use a policy registrada pelo IoC ("AllowAll") OU crie outra e use só uma.
// Se quiser controlar por origem via env, crie a policy lá; aqui aplique uma única policy:
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// (Opcional) OPTIONS catch-all se quiser redundância
app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.NoContent())
   .RequireCors("AllowAll");

// 2) expor SEM auth
app.UseHealthChecks("/healthz");

// opcional para teste rápido:
app.MapGet("/", () => Results.Ok("MyFinance API live v3")).AllowAnonymous();

app.Run();
