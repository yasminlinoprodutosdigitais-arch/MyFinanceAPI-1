namespace MyFinanceAPI.Api.Erros;

/// <summary>Corpo de toda resposta de erro da API: <c>{ "message": "..." }</c> (BE-P3-05).</summary>
public sealed record MensagemErro(string Message);
