using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// UnitOfWork real sobre SQLite em memória (BE-P1-05). Um contexto mínimo, só do teste,
/// basta: o que se verifica é a transação, não o modelo do MyFinance.
/// </summary>
public sealed class UnitOfWorkTests : IDisposable
{
    private sealed class Registro
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
    }

    private sealed class ContextoTeste(DbContextOptions<ContextoTeste> options) : DbContext(options)
    {
        public DbSet<Registro> Registros => Set<Registro>();
    }

    private readonly SqliteConnection _conexao = new("DataSource=:memory:");
    private readonly ContextoTeste _contexto;
    private readonly UnitOfWork _uow;

    public UnitOfWorkTests()
    {
        _conexao.Open();
        _contexto = NovoContexto();
        _contexto.Database.EnsureCreated();
        _uow = new UnitOfWork(_contexto);
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    private ContextoTeste NovoContexto() =>
        new(new DbContextOptionsBuilder<ContextoTeste>().UseSqlite(_conexao).Options);

    /// <summary>Lê por outro contexto, para não enxergar o que só está no change tracker.</summary>
    private List<string> Gravados()
    {
        using var leitura = NovoContexto();
        return leitura.Registros.OrderBy(r => r.Id).Select(r => r.Nome).ToList();
    }

    // Cada passo grava sozinho, como os repositórios fazem hoje.
    private async Task Gravar(string nome)
    {
        _contexto.Registros.Add(new Registro { Nome = nome });
        await _contexto.SaveChangesAsync();
    }

    [Fact]
    public async Task Varias_gravacoes_valem_juntas_quando_a_operacao_termina()
    {
        await _uow.ExecuteAsync(async () =>
        {
            await Gravar("conta");
            await Gravar("vencimento");
        });

        Assert.Equal(new[] { "conta", "vencimento" }, Gravados());
        Assert.Null(_contexto.Database.CurrentTransaction);
    }

    [Fact]
    public async Task Excecao_no_meio_desfaz_o_que_ja_foi_gravado_e_e_repassada()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _uow.ExecuteAsync(async () =>
        {
            await Gravar("conta");
            throw new InvalidOperationException("falhou no vencimento");
        }));

        Assert.Equal("falhou no vencimento", ex.Message);
        Assert.Empty(Gravados());
        Assert.Null(_contexto.Database.CurrentTransaction);
    }

    [Fact]
    public async Task Operacao_aninhada_participa_da_transacao_de_fora()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _uow.ExecuteAsync(async () =>
        {
            await _uow.ExecuteAsync(() => Gravar("conta nova"));   // não pode confirmar sozinha
            await Gravar("transação");
            throw new InvalidOperationException("falhou depois");
        }));

        Assert.Empty(Gravados());
    }

    [Fact]
    public async Task Versao_com_retorno_devolve_o_valor_e_confirma()
    {
        var id = await _uow.ExecuteAsync(async () =>
        {
            var registro = new Registro { Nome = "extrato" };
            _contexto.Registros.Add(registro);
            await _contexto.SaveChangesAsync();
            return registro.Id;
        });

        Assert.True(id > 0);
        Assert.Equal(new[] { "extrato" }, Gravados());
    }
}
