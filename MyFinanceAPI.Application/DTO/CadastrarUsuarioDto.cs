using System;
using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

public sealed record CadastrarUsuarioDto(string Login, string Senha, [TextoAte(120)] string NomeUsuario, [TextoAte(30)] string Perfil);