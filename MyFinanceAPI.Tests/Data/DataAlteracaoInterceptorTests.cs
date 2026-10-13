using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// DB-P1-12: DataAlteracao muda em toda edição; DataCriacao (só nas transacionais) é gravada uma
/// vez e nunca mais. Contexto mínimo do teste, com as duas formas do modelo real: shadow property
/// (8 tabelas) e propriedade na entidade (Banco, Category).
/// </summary>
public sealed class DataAlteracaoInterceptorTests : IDisposable
{
    private sealed class RelogioFalso(DateTimeOffset agora) : TimeProvider
    {
        public DateTimeOffset Agora { get; set; } = agora;
        public override DateTimeOffset GetUtcNow() => Agora;
    }

    private sealed class Lancamento   // como Transaction: DataAlteracao e DataCriacao shadow
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
    }

    private sealed class Cadastro     // como Banco: DataAlteracao é propriedade
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public DateTime DataAlteracao { get; set; }
    }

    private sealed class SemAuditoria // como MovimentacaoDiaria: sem as colunas
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
    }

    private sealed class ContextoTeste(DbContextOptions<ContextoTeste> options) : DbContext(options)
    {
        public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
        public DbSet<Cadastro> Cadastros => Set<Cadastro>();
        public DbSet<SemAuditoria> SemAuditorias => Set<SemAuditoria>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lancamento>().Property<DateTime>("DataAlteracao");
            modelBuilder.Entity<Lancamento>().Property<DateTime>("DataCriacao");
        }
    }

    private static readonly DateTimeOffset Criacao = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Edicao = new(2026, 10, 13, 18, 30, 0, TimeSpan.Zero);

    private readonly SqliteConnection _conexao = new("DataSource=:memory:");
    private readonly RelogioFalso _relogio = new(Criacao);

    public DataAlteracaoInterceptorTests()
    {
        _conexao.Open();
        using var ctx = NovoContexto();
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _conexao.Dispose();

    private ContextoTeste NovoContexto() =>
        new(new DbContextOptionsBuilder<ContextoTeste>()
            .UseSqlite(_conexao)
            .AddInterceptors(new DataAlteracaoInterceptor(_relogio))
            .Options);

    private static DateTime Shadow(ContextoTeste ctx, object entidade, string coluna) =>
        (DateTime)ctx.Entry(entidade).Property(coluna).CurrentValue!;

    [Fact]
    public void Criacao_preenche_as_duas_datas()
    {
        using var ctx = NovoContexto();
        var lancamento = new Lancamento { Nome = "Luz" };
        ctx.Add(lancamento);
        ctx.SaveChanges();

        Assert.Equal(Criacao.UtcDateTime, Shadow(ctx, lancamento, "DataAlteracao"));
        Assert.Equal(Criacao.UtcDateTime, Shadow(ctx, lancamento, "DataCriacao"));
    }

    [Fact]
    public async Task Edicao_atualiza_DataAlteracao_e_mantem_DataCriacao()
    {
        int id;
        using (var ctx = NovoContexto())
        {
            var novo = new Lancamento { Nome = "Luz" };
            ctx.Add(novo);
            await ctx.SaveChangesAsync();
            id = novo.Id;
        }

        _relogio.Agora = Edicao;
        using (var ctx = NovoContexto())
        {
            var existente = await ctx.Lancamentos.SingleAsync(l => l.Id == id);
            existente.Nome = "Luz (paga)";
            await ctx.SaveChangesAsync();
        }

        using var leitura = NovoContexto();
        var gravado = await leitura.Lancamentos.SingleAsync(l => l.Id == id);
        Assert.Equal(Edicao.UtcDateTime, Shadow(leitura, gravado, "DataAlteracao"));
        Assert.Equal(Criacao.UtcDateTime, Shadow(leitura, gravado, "DataCriacao"));
    }

    [Fact]
    public void Update_com_entidade_desanexada_nao_zera_DataCriacao()
    {
        int id;
        using (var ctx = NovoContexto())
        {
            var novo = new Lancamento { Nome = "Água" };
            ctx.Add(novo);
            ctx.SaveChanges();
            id = novo.Id;
        }

        _relogio.Agora = Edicao;
        using (var ctx = NovoContexto())
        {
            // Como um mapper que monta a entidade do DTO: shadow properties chegam com default(DateTime).
            ctx.Update(new Lancamento { Id = id, Nome = "Água (editada)" });
            ctx.SaveChanges();
        }

        using var leitura = NovoContexto();
        var gravado = leitura.Lancamentos.Single(l => l.Id == id);
        Assert.Equal("Água (editada)", gravado.Nome);
        Assert.Equal(Criacao.UtcDateTime, Shadow(leitura, gravado, "DataCriacao"));
        Assert.Equal(Edicao.UtcDateTime, Shadow(leitura, gravado, "DataAlteracao"));
    }

    [Fact]
    public void Propriedade_DataAlteracao_na_entidade_tambem_e_atualizada()
    {
        using var ctx = NovoContexto();
        var cadastro = new Cadastro { Nome = "Bradesco" };
        ctx.Add(cadastro);
        ctx.SaveChanges();
        Assert.Equal(Criacao.UtcDateTime, cadastro.DataAlteracao);

        _relogio.Agora = Edicao;
        cadastro.Nome = "Bradesco Cartão";
        ctx.SaveChanges();

        Assert.Equal(Edicao.UtcDateTime, cadastro.DataAlteracao);
    }

    [Fact]
    public void Entidade_sem_as_colunas_salva_normalmente()
    {
        using var ctx = NovoContexto();
        ctx.Add(new SemAuditoria { Nome = "x" });

        Assert.Equal(1, ctx.SaveChanges());
    }
}
