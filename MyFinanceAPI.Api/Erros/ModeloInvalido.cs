using Microsoft.AspNetCore.Mvc;

namespace MyFinanceAPI.Api.Erros;

/// <summary>
/// Resposta do [ApiController] para corpo ausente, JSON malformado ou campo obrigatório faltando
/// (BE-P3-05). O padrão do ASP.NET é um ProblemDetails; aqui sai <c>{ message }</c>, como todo erro.
/// </summary>
public static class ModeloInvalido
{
    public const string Prefixo = "Dados inválidos.";

    public static IActionResult Resposta(ActionContext contexto)
    {
        var detalhes = contexto.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();

        var mensagem = detalhes.Count == 0 ? Prefixo : $"{Prefixo} {string.Join(" ", detalhes)}";
        return new BadRequestObjectResult(new MensagemErro(mensagem));
    }
}
