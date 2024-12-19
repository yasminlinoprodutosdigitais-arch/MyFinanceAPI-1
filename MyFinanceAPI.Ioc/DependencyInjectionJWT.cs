using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MyFinanceAPI.Ioc
{
    public static class DependencyInjectionJWT
    {
        public static IServiceCollection AddInfrastructureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration.GetValue<string>("AppSettings:SecretKey");

            // Verifique se a chave secreta está configurada corretamente
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentException("A chave secreta do JWT não está configurada.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "MyFinanceApi",  // O issuer que você configurou ao gerar o token
            ValidAudience = "MyFinanceApiFrontend",  // A audience que você configurou ao gerar o token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero // Sem tolerância para expiração
        };
    });


            return services;
        }
    }
    
}
