namespace MyFinanceAPI.Api.Erros;

/// <summary>
/// Requisição sem usuário identificável no token. É a única situação que responde 401 —
/// o front faz logout em 401, então "registro de outro usuário" usa UnauthorizedAccessException (403).
/// </summary>
public sealed class UsuarioNaoAutenticadoException() : Exception("Usuário não autorizado!");
