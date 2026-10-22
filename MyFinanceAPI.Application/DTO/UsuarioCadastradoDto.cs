
namespace MyFinanceAPI.Application.DTO;

/// <summary>Resposta do 201 de CadastrarUsuario: o criado, sem senha nem hash (BE-P3-10).</summary>
public sealed record UsuarioCadastradoDto(int Id, string Login, string NomeUsuario, string Perfil);
