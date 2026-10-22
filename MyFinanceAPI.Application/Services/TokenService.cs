using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Services;

// Refresh token removido (BE-P0-04): vivia numa List estática em memória e nunca funcionou.
// Se voltar a ser necessário, precisa de persistência (tabela + expiração + rotação).
public class TokenService : ITokenService
{
    private readonly TokenSettings _tokenSettings;

    public TokenService(IOptions<TokenSettings> tokenSettings)
    {
        _tokenSettings = tokenSettings.Value;
    }

    /// <summary>
    /// Recebe o usuário já validado (BE-P3-10): antes buscava de novo pelo login, sem conferir a
    /// senha. Sem catch: uma falha vai ao ErroHandler (500 com log) em vez de virar null e 422.
    /// </summary>
    public TokenDto CreateToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenSettings.SecretKey);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

        var brasilTz = TimeZoneInfo.FindSystemTimeZoneById(
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "E. South America Standard Time"
            : "America/Sao_Paulo"
        );

        var expiresUtc = DateTime.UtcNow.AddHours(3);
        var expiresBrasil = TimeZoneInfo.ConvertTimeFromUtc(expiresUtc, brasilTz);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(usuario),
            Expires = expiresUtc,
            SigningCredentials = credentials
        };

        // Gerar o token JWT
        var objectToken = tokenHandler.CreateToken(tokenDescriptor);
        string token = tokenHandler.WriteToken(objectToken);

        return new TokenDto(token, DateTime.Now, expiresBrasil);
    }

    private ClaimsIdentity GenerateClaims(Usuario user)
    {
        var ci = new ClaimsIdentity();

        // Adicionando claims essenciais
        ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));  // Adiciona o ID do usuário
        ci.AddClaim(new Claim(ClaimTypes.Role, user.Perfil));  // Exemplo de uma role (permissão) que o usuário possui

        // Adicionar mais claims conforme necessárioc
        // ci.AddClaim(new Claim("Email", user.Email));
        // ci.AddClaim(new Claim("IsActive", user.IsActive.ToString()));

        return ci;
    }
}
