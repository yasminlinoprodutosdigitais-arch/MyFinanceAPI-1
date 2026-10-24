using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Falha ao gravar tem que chegar ao ErroHandler (BE-P4-07). Antes o repositório engolia a exceção e devolvia null,
/// e o controller quebrava em created.Id: o lançamento manual voltava 500 "Erro inesperado" e o erro real sumia.
/// SQLite sem esquema: qualquer gravação falha, que é o que se quer aqui.
/// </summary>
public sealed class ExtratoBancarioItemRepositoryTests : IDisposable
{
    private readonly SqliteConnection _conexao = new("DataSource=:memory:");
    private readonly ContextDB _contexto;

    public ExtratoBancarioItemRepositoryTests()
    {
        _conexao.Open();
        _contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseSqlite(_conexao).Options);
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    private static ExtratoBancarioItem Item() => new()
    {
        UsuarioId = 1,
        DataMovimentacao = new DateOnly(2026, 10, 1),
        Valor = 1,
        TipoLancamento = TipoLancamentoExtrato.Saida,
    };

    [Fact]
    public async Task CreateAsync_repassa_a_falha_da_gravacao()
    {
        var repo = new ExtratoBancarioItemRepository(_contexto);

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(Item()));
    }

    [Fact]
    public async Task CreateRangeAsync_repassa_a_falha_da_gravacao()
    {
        var repo = new ExtratoBancarioItemRepository(_contexto);

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateRangeAsync([Item()]));
    }
}
