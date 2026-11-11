using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Card #50: gravar um token novo invalida os ainda válidos do mesmo usuário e tipo, e só eles.
/// Só as tabelas asp_net_users e tokens_usuario são criadas, a partir do script que o próprio EF gera.
/// </summary>
public sealed class TokenUsuarioRepositoryTests : IDisposable
{
    private static readonly DateTime Agora = new(2026, 11, 11, 12, 0, 0, DateTimeKind.Utc);
    private readonly SqliteConnection _conexao = new("DataSource=:memory:");
    private readonly ContextDB _contexto;
    private readonly int _ana;
    private readonly int _bia;

    public TokenUsuarioRepositoryTests()
    {
        _conexao.Open();
        _contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseSqlite(_conexao).Options);

        var script = _contexto.Database.GenerateCreateScript();
        foreach (var tabela in new[] { "asp_net_users", "tokens_usuario" })
            _contexto.Database.ExecuteSqlRaw(Regex.Match(script, $@"CREATE TABLE ""{tabela}"" \(.*?\n\);", RegexOptions.Singleline).Value
                .Replace("now()", "CURRENT_TIMESTAMP"));   // o padrão do Postgres não existe no SQLite

        _ana = Usuario("ana");
        _bia = Usuario("bia");
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    private int Usuario(string login)
    {
        var usuario = new Usuario(login, "senha", login, Perfis.Pessoa, true, Agora, Agora);
        new UsuarioRepository(_contexto).Cadastrar(usuario);
        return usuario.Id;
    }

    private static TokenUsuario Token(int usuarioId, TipoTokenUsuario tipo, string hash) =>
        new() { UsuarioId = usuarioId, Tipo = tipo, Hash = hash, ExpiraEm = Agora.AddHours(1) };

    private TokenUsuario Gravado(string hash) => _contexto.Set<TokenUsuario>().AsNoTracking().Single(t => t.Hash == hash);

    [Fact]
    public async Task Token_novo_invalida_os_anteriores_do_mesmo_usuario_e_tipo()
    {
        var repo = new TokenUsuarioRepository(_contexto);
        await repo.SubstituirAsync(Token(_ana, TipoTokenUsuario.RedefinirSenha, "a1"), Agora);
        await repo.SubstituirAsync(Token(_ana, TipoTokenUsuario.ConfirmarEmail, "c1"), Agora);
        await repo.SubstituirAsync(Token(_bia, TipoTokenUsuario.RedefinirSenha, "b1"), Agora);

        await repo.SubstituirAsync(Token(_ana, TipoTokenUsuario.RedefinirSenha, "a2"), Agora.AddMinutes(5));

        Assert.Equal(Agora.AddMinutes(5), Gravado("a1").UsadoEm);
        Assert.Null(Gravado("a2").UsadoEm);
        Assert.Null(Gravado("c1").UsadoEm);
        Assert.Null(Gravado("b1").UsadoEm);
    }

    [Fact]
    public async Task Busca_pelo_hash_e_tipo_e_marca_usado()
    {
        var repo = new TokenUsuarioRepository(_contexto);
        await repo.SubstituirAsync(Token(_ana, TipoTokenUsuario.ConfirmarEmail, "c1"), Agora);

        Assert.Null(await repo.ObterPorHashAsync("c1", TipoTokenUsuario.RedefinirSenha));
        var token = await repo.ObterPorHashAsync("c1", TipoTokenUsuario.ConfirmarEmail);
        await repo.MarcarUsadoAsync(token!, Agora);

        Assert.Equal(Agora, Gravado("c1").UsadoEm);
    }
}
