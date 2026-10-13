namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Registro inexistente ou de outro usuário — a API responde 404 com a mensagem (BE-P2-01).
/// Substitui os <c>throw new Exception("… não encontrada")</c> genéricos, que viravam 500.
/// </summary>
public sealed class NaoEncontradoException(string mensagem) : Exception(mensagem);
