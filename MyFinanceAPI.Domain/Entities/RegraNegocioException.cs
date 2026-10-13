namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Regra de negócio violada (nome duplicado, registro em uso, tipo padrão do sistema…) — a API
/// responde 409 com a mensagem (BE-P2-01). Herda InvalidOperationException para os catch existentes;
/// uma InvalidOperationException comum (do framework, do EF) vira 500 sem expor a mensagem.
/// </summary>
public sealed class RegraNegocioException(string mensagem) : InvalidOperationException(mensagem);
