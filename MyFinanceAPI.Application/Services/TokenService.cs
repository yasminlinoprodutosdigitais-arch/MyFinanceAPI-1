using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TokenService : ITokenService
{
    private static readonly List<(string, string)> _refreshTokens = new();
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

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(usuario),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials

            };


            // Gerar o token JWT
            var objectToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(objectToken);

            // Gerar o refresh token
            string refreshToken = GenerateRefreshToken();

            // Salvar o refresh token
            SaveRefreshToken(usuario.Id.ToString(), refreshToken);

            // Retornar o DTO com o JWT e refresh token
            TokenDto tokenDto = new TokenDto(token, refreshToken, DateTime.Now, DateTime.UtcNow);

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


    public TokenDto RefreshToken(string oldToken, string refreshToken)
    {
        try
        {
            ClaimsPrincipal claims = GetPrincipalFromExpiredToken(oldToken);

            ObjectId objectId = ObjectId.Parse(claims.Identity!.Name);

            string savedRefreshToken = GetRefreshToken(objectId.ToString());

            if (refreshToken != savedRefreshToken)
                return null;

            JwtSecurityTokenHandler tokenHandler = new();
            string str = _tokenSettings.SecretKey;
            var key = Encoding.ASCII.GetBytes(str);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, claims.Identity.Name!),
                    new(ClaimTypes.Sid, Guid.NewGuid().ToString())
                }),

                Expires = new TokenSettings().Expires,
                NotBefore = new TokenSettings().NotBefore,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            SecurityToken objectToken = tokenHandler.CreateToken(tokenDescriptor);
            string newToken = tokenHandler.WriteToken(objectToken);
            string newRefreshToken = GenerateRefreshToken();
            DeleteRefreshToken(objectId.ToString());
            SaveRefreshToken(objectId.ToString(), newRefreshToken);

            TokenDto tokenDto = new(newToken, refreshToken, DateTime.Now, new TokenSettings().Expires);

            return tokenDto;
        }
        catch (Exception ex)
        {
            return null!;
        }
    }

    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];

        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public void SaveRefreshToken(string userLogin, string refreshToken)
    {
        _refreshTokens.Add((userLogin, refreshToken));
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)),
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token Inválido.");

        return principal;
    }

    public string GetRefreshToken(string userId)
    {
        return _refreshTokens.FirstOrDefault(x => x.Item1 == userId).Item2;
    }

    public void DeleteRefreshToken(string userLogin)
    {
        var item = _refreshTokens.FirstOrDefault(x => x.Item1 == userLogin);
        _refreshTokens.Remove(item);
    }
}

