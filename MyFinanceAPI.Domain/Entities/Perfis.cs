namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Perfis de usuário (claim role do token). Pessoa é quem se cadastra pela tela (card #50);
/// Admin e Empresa são criados pelo /CadastrarUsuario.
/// </summary>
public static class Perfis
{
    public const string Admin = "Admin";
    public const string Empresa = "Empresa";
    public const string Pessoa = "Pessoa";
}
