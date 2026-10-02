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
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

// Refresh token removido (BE-P0-04): vivia numa List estática em memória e nunca funcionou.
// Se voltar a ser necessário, precisa de persistência (tabela + expiração + rotação).
public class TokenService : ITokenService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly TokenSettings _tokenSettings;

    public TokenService(IUsuarioRepository usuarioRepository, IOptions<TokenSettings> tokenSettings)
    {
        _usuarioRepository = usuarioRepository;
        _tokenSettings = tokenSettings.Value;
    }
    public async Task<TokenDto> CreateToken(UsuarioDto usuarioDto)
    {
        try
        {
            Usuario usuario = await _usuarioRepository.BuscarUsuario(usuarioDto.Login, usuarioDto.Senha);

            if (usuario == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            string str = _tokenSettings.SecretKey;
            var key = Encoding.ASCII.GetBytes(str);
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

            TokenDto tokenDto = new TokenDto(token, DateTime.Now, expiresBrasil);

            return tokenDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar o token: {ex.Message}");
            Console.WriteLine($"Detalhes: {ex.StackTrace}");
            return null;
        }

    }

    private ClaimsIdentity GenerateClaims(Usuario user)
    {
        var ci = new ClaimsIdentity();

        // Adicionando claims essenciais
        ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));  // Adiciona o ID do usuário
        ci.AddClaim(new Claim(ClaimTypes.Role, user.Role));  // Exemplo de uma role (permissão) que o usuário possui

        // Adicionar mais claims conforme necessárioc
        // ci.AddClaim(new Claim("Email", user.Email));
        // ci.AddClaim(new Claim("IsActive", user.IsActive.ToString()));

        return ci;
    }
}
