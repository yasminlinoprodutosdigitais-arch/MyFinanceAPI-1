using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>
/// Acesso sem login (card #50): criar conta, confirmar o e-mail e recuperar a senha. Os pedidos que levam
/// um e-mail não dizem se ele tem conta: o que muda é o e-mail que o dono recebe.
/// </summary>
public interface IAcessoService
{
    /// <summary>Cria a conta (Pessoa) e manda o link de confirmação. Dados inválidos: ArgumentException.</summary>
    Task CadastrarAsync(CadastroDto dados);
    /// <summary>false se o link venceu ou já foi usado.</summary>
    Task<bool> ConfirmarEmailAsync(string token);
    Task ReenviarConfirmacaoAsync(string email);
    Task EsqueciSenhaAsync(string email);
    /// <summary>false se o link venceu ou já foi usado. Senha fraca: ArgumentException (o link continua valendo).</summary>
    Task<bool> RedefinirSenhaAsync(RedefinirSenhaDto dados);
}
