using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Domain.Entities;
using Npgsql;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// BE-P2-01: erros tratados num lugar só, sempre como { message }, sem stack trace.
/// O front faz logout em 401 — por isso "item de outro usuário" é 403, não 401.
/// </summary>
public class ErroHandlerTests
{
    private static async Task<(int status, string? message, string corpo)> Tratar(Exception ex)
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();

        var tratou = await new ErroHandler(NullLogger<ErroHandler>.Instance).TryHandleAsync(contexto, ex, CancellationToken.None);
        Assert.True(tratou);

        contexto.Response.Body.Position = 0;
        var corpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
        var message = JsonDocument.Parse(corpo).RootElement.GetProperty("message").GetString();
        return (contexto.Response.StatusCode, message, corpo);
    }

    private static DbUpdateException Postgres(string sqlState, string? constraint = null) =>
        new("falha ao salvar", new PostgresException("detalhe interno do banco", "ERROR", "ERROR", sqlState, constraintName: constraint));

    [Fact]
    public async Task Nao_encontrado_e_404_com_a_mensagem()
    {
        var (status, message, _) = await Tratar(new NaoEncontradoException("Banco não encontrado."));
        Assert.Equal(404, status);
        Assert.Equal("Banco não encontrado.", message);
    }

    [Fact]
    public async Task KeyNotFound_tambem_e_404()
    {
        var (status, _, _) = await Tratar(new KeyNotFoundException("Extrato bancário não encontrado."));
        Assert.Equal(404, status);
    }

    [Fact]
    public async Task Regra_de_negocio_e_409_com_a_mensagem()
    {
        var (status, message, _) = await Tratar(new RegraNegocioException("Os tipos padrão do sistema não podem ser alterados nem excluídos."));
        Assert.Equal(409, status);
        Assert.Equal("Os tipos padrão do sistema não podem ser alterados nem excluídos.", message);
    }

    [Fact]
    public async Task InvalidOperation_interno_do_framework_e_500_sem_expor_a_mensagem()
    {
        // .First() em lista vazia, conflito de tracking do EF etc. não são regra de negócio.
        var (status, message, corpo) = await Tratar(new InvalidOperationException("Sequence contains no elements"));
        Assert.Equal(500, status);
        Assert.Equal("Erro inesperado. Tente novamente.", message);
        Assert.DoesNotContain("Sequence", corpo);
    }

    [Fact]
    public async Task Sem_usuario_autenticado_e_401()
    {
        var (status, _, _) = await Tratar(new UsuarioNaoAutenticadoException());
        Assert.Equal(401, status);
    }

    [Fact]
    public async Task Item_de_outro_usuario_e_403_para_nao_deslogar()
    {
        var (status, message, _) = await Tratar(new UnauthorizedAccessException("Usuário não autorizado para excluir este item."));
        Assert.Equal(403, status);
        Assert.Equal("Usuário não autorizado para excluir este item.", message);
    }

    [Theory]
    [InlineData(typeof(ArgumentException))]
    [InlineData(typeof(ArgumentNullException))]
    [InlineData(typeof(JsonException))]
    [InlineData(typeof(FormatException))]
    public async Task Entrada_invalida_e_400(Type tipo)
    {
        var ex = (Exception)Activator.CreateInstance(tipo, "Dados inválidos.")!;
        var (status, _, _) = await Tratar(ex);
        Assert.Equal(400, status);
    }

    [Fact]
    public async Task Check_do_banco_e_400_citando_a_regra()
    {
        var (status, message, corpo) = await Tratar(Postgres(PostgresErrorCodes.CheckViolation, "itemlista_status_check"));
        Assert.Equal(400, status);
        Assert.Contains("itemlista_status_check", message);
        Assert.DoesNotContain("detalhe interno", corpo);
    }

    [Fact]
    public async Task Unique_do_banco_e_409()
    {
        var (status, _, _) = await Tratar(Postgres(PostgresErrorCodes.UniqueViolation, "UX_Banco_UserId_NomeBanco"));
        Assert.Equal(409, status);
    }

    [Fact]
    public async Task FK_do_banco_e_409()
    {
        var (status, _, _) = await Tratar(Postgres(PostgresErrorCodes.ForeignKeyViolation));
        Assert.Equal(409, status);
    }

    [Fact]
    public async Task Texto_acima_do_limite_da_coluna_e_400()
    {
        // DB-P3-04: com varchar(n) nas colunas, estourar o limite é erro de entrada, não falha interna.
        var (status, message, corpo) = await Tratar(Postgres(PostgresErrorCodes.StringDataRightTruncation));
        Assert.Equal(400, status);
        Assert.Equal("Texto maior que o limite permitido para o campo.", message);
        Assert.DoesNotContain("detalhe interno", corpo);
    }

    [Fact]
    public async Task Erro_inesperado_e_500_sem_detalhes_internos()
    {
        var (status, message, corpo) = await Tratar(new NullReferenceException("Object reference not set to an instance of an object."));
        Assert.Equal(500, status);
        Assert.Equal("Erro inesperado. Tente novamente.", message);
        Assert.DoesNotContain("Object reference", corpo);
        Assert.DoesNotContain("   at ", corpo);
    }

    [Fact]
    public async Task Outro_erro_de_banco_e_500()
    {
        var (status, _, _) = await Tratar(new DbUpdateException("falha", new Exception("interno")));
        Assert.Equal(500, status);
    }
}
