using System.Linq.Expressions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Base dos cadastros simples (BE-P2-03) sobre SQLite em memória. Um contexto mínimo, só do teste:
/// o que se verifica é o CRUD restrito ao dono, não o modelo do MyFinance.
/// </summary>
public sealed class RepositorioDoUsuarioTests : IDisposable
{
    private const int Eu = 1;
    private const int Outro = 2;

    private sealed class Cadastro : IEntidade
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Nome { get; set; } = "";
        public DateTime DataCriacao { get; set; }
    }

    /// <summary>Como o TipoCartao: sem dono é padrão do sistema, visível a todos.</summary>
    private sealed class Tipo : IEntidade
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Nome { get; set; } = "";
    }

    private sealed class ContextoTeste(DbContextOptions<ContextoTeste> options) : DbContext(options)
    {
        public DbSet<Cadastro> Cadastros => Set<Cadastro>();
        public DbSet<Tipo> Tipos => Set<Tipo>();
    }

    private sealed class CadastroRepository(ContextoTeste context) : RepositorioDoUsuario<Cadastro>(context)
    {
        protected override string MensagemNaoEncontrado => "Cadastro não encontrado.";
        protected override Expression<Func<Cadastro, bool>> Proprios(int userId) => c => c.UserId == userId;
        protected override IOrderedQueryable<Cadastro> Ordenar(IQueryable<Cadastro> query) => query.OrderBy(c => c.Nome);
        protected override void CopiarAlteracoes(Cadastro existente, Cadastro entrada) => existente.Nome = entrada.Nome;
    }

    private sealed class TipoRepository(ContextoTeste context) : RepositorioDoUsuario<Tipo>(context)
    {
        protected override string MensagemNaoEncontrado => "Tipo não encontrado.";
        protected override Expression<Func<Tipo, bool>> Proprios(int userId) => t => t.UserId == userId;
        protected override Expression<Func<Tipo, bool>> Visiveis(int userId) => t => t.UserId == userId || t.UserId == null;
        protected override IOrderedQueryable<Tipo> Ordenar(IQueryable<Tipo> query) => query.OrderBy(t => t.Nome);
        protected override void CopiarAlteracoes(Tipo existente, Tipo entrada) => existente.Nome = entrada.Nome;
    }

    private readonly SqliteConnection _conexao = new("DataSource=:memory:");
    private readonly ContextoTeste _contexto;

    public RepositorioDoUsuarioTests()
    {
        _conexao.Open();
        _contexto = NovoContexto();
        _contexto.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    private ContextoTeste NovoContexto() =>
        new(new DbContextOptionsBuilder<ContextoTeste>().UseSqlite(_conexao).Options);

    private async Task<Cadastro> Semear(int userId, string nome)
    {
        using var escrita = NovoContexto();
        var c = new Cadastro { UserId = userId, Nome = nome, DataCriacao = new DateTime(2026, 1, 1) };
        escrita.Cadastros.Add(c);
        await escrita.SaveChangesAsync();
        return c;
    }

    private async Task<Tipo> SemearTipo(int? userId, string nome)
    {
        using var escrita = NovoContexto();
        var t = new Tipo { UserId = userId, Nome = nome };
        escrita.Tipos.Add(t);
        await escrita.SaveChangesAsync();
        return t;
    }

    /// <summary>Lê por outro contexto, para não enxergar o que só está no change tracker.</summary>
    private Cadastro? Lido(int id)
    {
        using var leitura = NovoContexto();
        return leitura.Cadastros.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    [Fact]
    public async Task Listar_traz_so_os_do_usuario_na_ordem_do_repositorio()
    {
        await Semear(Eu, "Nubank");
        await Semear(Outro, "Alheio");
        await Semear(Eu, "Itaú");

        var lista = await new CadastroRepository(_contexto).ListarAsync(Eu);

        Assert.Equal(new[] { "Itaú", "Nubank" }, lista.Select(c => c.Nome));
    }

    [Fact]
    public async Task ObterPorId_de_outro_usuario_e_null()
    {
        var alheio = await Semear(Outro, "Alheio");
        var meu = await Semear(Eu, "Meu");
        var repo = new CadastroRepository(_contexto);

        Assert.Null(await repo.ObterPorIdAsync(alheio.Id, Eu));
        Assert.Equal("Meu", (await repo.ObterPorIdAsync(meu.Id, Eu))!.Nome);
    }

    [Fact]
    public async Task Criar_grava_e_devolve_com_id()
    {
        var criado = await new CadastroRepository(_contexto).CriarAsync(new Cadastro { UserId = Eu, Nome = "Novo" });

        Assert.True(criado.Id > 0);
        Assert.Equal("Novo", Lido(criado.Id)!.Nome);
    }

    [Fact]
    public async Task Atualizar_copia_so_os_campos_declarados_e_mantem_dono_e_criacao()
    {
        var meu = await Semear(Eu, "Antigo");
        var entrada = new Cadastro { Id = meu.Id, UserId = Outro, Nome = "Novo", DataCriacao = new DateTime(2030, 1, 1) };

        await new CadastroRepository(_contexto).AtualizarAsync(entrada, Eu);

        var gravado = Lido(meu.Id)!;
        Assert.Equal("Novo", gravado.Nome);
        Assert.Equal(Eu, gravado.UserId);
        Assert.Equal(new DateTime(2026, 1, 1), gravado.DataCriacao);
    }

    [Fact]
    public async Task Atualizar_de_outro_usuario_e_404_e_nao_altera()
    {
        var alheio = await Semear(Outro, "Alheio");

        var ex = await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            new CadastroRepository(_contexto).AtualizarAsync(new Cadastro { Id = alheio.Id, Nome = "Invadido" }, Eu));

        Assert.Equal("Cadastro não encontrado.", ex.Message);
        Assert.Equal("Alheio", Lido(alheio.Id)!.Nome);
    }

    [Fact]
    public async Task Remover_apaga_o_do_usuario_e_devolve_o_removido()
    {
        var meu = await Semear(Eu, "Meu");

        var removido = await new CadastroRepository(_contexto).RemoverAsync(meu.Id, Eu);

        Assert.Equal("Meu", removido!.Nome);
        Assert.Null(Lido(meu.Id));
    }

    [Fact]
    public async Task Remover_de_outro_usuario_nao_apaga_e_devolve_null()
    {
        var alheio = await Semear(Outro, "Alheio");

        Assert.Null(await new CadastroRepository(_contexto).RemoverAsync(alheio.Id, Eu));
        Assert.NotNull(Lido(alheio.Id));
    }

    [Fact]
    public async Task Visiveis_ampliam_a_leitura_mas_nao_a_escrita()
    {
        var sistema = await SemearTipo(null, "Cartão de crédito");
        await SemearTipo(Eu, "Meu tipo");
        await SemearTipo(Outro, "Alheio");
        var repo = new TipoRepository(_contexto);

        Assert.Equal(new[] { "Cartão de crédito", "Meu tipo" }, (await repo.ListarAsync(Eu)).Select(t => t.Nome));
        Assert.NotNull(await repo.ObterPorIdAsync(sistema.Id, Eu));

        // Alterar e remover só acham os próprios: o padrão do sistema fica intocado.
        await Assert.ThrowsAsync<NaoEncontradoException>(() => repo.AtualizarAsync(new Tipo { Id = sistema.Id, Nome = "X" }, Eu));
        Assert.Null(await repo.RemoverAsync(sistema.Id, Eu));

        using var leitura = NovoContexto();
        Assert.Equal("Cartão de crédito", leitura.Tipos.AsNoTracking().Single(t => t.Id == sistema.Id).Nome);
    }
}
