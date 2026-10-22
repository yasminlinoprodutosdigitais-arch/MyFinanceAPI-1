using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFinanceAPI.Domain.Entities;
using Npgsql;

namespace MyFinanceAPI.Api.Erros;

/// <summary>
/// Tratamento de erros da API num lugar só (BE-P2-01). Toda resposta de erro é <c>{ message }</c>,
/// sem stack trace nem detalhe interno; só o 500 é registrado em log com a exceção completa.
/// Os controllers não têm mais try/catch.
/// </summary>
public sealed class ErroHandler(ILogger<ErroHandler> logger) : IExceptionHandler
{
    private const string MensagemInesperada = "Erro inesperado. Tente novamente.";

    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excecao, CancellationToken cancellationToken)
    {
        var (status, mensagem) = Classificar(excecao);

        if (status >= StatusCodes.Status500InternalServerError)
            logger.LogError(excecao, "Erro não tratado em {Metodo} {Caminho}", contexto.Request.Method, contexto.Request.Path);

        contexto.Response.StatusCode = status;
        await contexto.Response.WriteAsJsonAsync(new MensagemErro(mensagem), cancellationToken);
        return true;
    }

    public static (int Status, string Mensagem) Classificar(Exception excecao) => excecao switch
    {
        UsuarioNaoAutenticadoException or SecurityTokenException
            => (StatusCodes.Status401Unauthorized, excecao.Message),

        // Registro de outro usuário: 403, porque o front faz logout em 401.
        UnauthorizedAccessException
            => (StatusCodes.Status403Forbidden, excecao.Message),

        NaoEncontradoException or KeyNotFoundException
            => (StatusCodes.Status404NotFound, excecao.Message),

        // Regras de negócio (nome duplicado, tipo do sistema, registro em uso…). Uma
        // InvalidOperationException comum (framework, EF) cai no 500 — a mensagem é interna.
        RegraNegocioException
            => (StatusCodes.Status409Conflict, excecao.Message),

        ArgumentException or JsonException or FormatException
            => (StatusCodes.Status400BadRequest, excecao.Message),

        DbUpdateException { InnerException: PostgresException pg } => Classificar(pg),

        _ => (StatusCodes.Status500InternalServerError, MensagemInesperada)
    };

    private static (int, string) Classificar(PostgresException pg) => pg.SqlState switch
    {
        PostgresErrorCodes.CheckViolation
            => (StatusCodes.Status400BadRequest, $"Valor não permitido (regra {pg.ConstraintName})."),
        PostgresErrorCodes.UniqueViolation
            => (StatusCodes.Status409Conflict, "Já existe um registro com esses dados."),
        PostgresErrorCodes.ForeignKeyViolation
            => (StatusCodes.Status409Conflict, "Operação não permitida: o registro está ligado a outros dados."),
        // As colunas de texto têm limite desde DB-P3-04; passar do tamanho é erro de entrada.
        PostgresErrorCodes.StringDataRightTruncation
            => (StatusCodes.Status400BadRequest, "Texto maior que o limite permitido para o campo."),
        _ => (StatusCodes.Status500InternalServerError, MensagemInesperada)
    };
}
