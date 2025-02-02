using System;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface ITokenService
{
    Task<TokenDto> CreateToken(UsuarioDto usuario);
    TokenDto RefreshToken(string oldToken, string refreshToken);
}
