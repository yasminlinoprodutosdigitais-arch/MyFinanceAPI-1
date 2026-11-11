using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

/// <summary>Autocadastro (card #50). O perfil não vem daqui: quem se cadastra é sempre Pessoa.</summary>
public sealed record CadastroDto([TextoAte(120)] string Nome, [TextoAte(256)] string Email, [TextoAte(128)] string Senha);

/// <summary>Pedidos que levam só o e-mail: reenviar a confirmação e "esqueci a senha".</summary>
public sealed record EmailDto([TextoAte(256)] string Email);

/// <summary>O token do link recebido por e-mail.</summary>
public sealed record LinkDto([TextoAte(100)] string Token);

public sealed record RedefinirSenhaDto([TextoAte(100)] string Token, [TextoAte(128)] string Senha);
