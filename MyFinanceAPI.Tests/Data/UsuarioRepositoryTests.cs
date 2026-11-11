using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Data.Repositories;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Card #70 (Tela de Login): entra pelo usuário ou pelo e-mail. O usuário tem prioridade; o e-mail
/// compara sem diferenciar maiúsculas (o NormalizedEmail não é preenchido pelo cadastro atual).
/// Só a tabela asp_net_users é criada no SQLite, a partir do script que o próprio EF gera.
/// </summary>
public sealed class UsuarioRepositoryTests : IDisposable
{
    private readonly SqliteConnection _conexao = new("DataSource=:memory:");
    private readonly ContextDB _contexto;

    public UsuarioRepositoryTests()
    {
        _conexao.Open();
        _contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseSqlite(_conexao).Options);

        var script = _contexto.Database.GenerateCreateScript();
        var tabela = Regex.Match(script, @"CREATE TABLE ""asp_net_users"" \(.*?\n\);", RegexOptions.Singleline).Value;
        _contexto.Database.ExecuteSqlRaw(tabela);
    }

    public void Dispose()
    {
        _contexto.Dispose();
        _conexao.Dispose();
    }

    private Usuario Gravar(string userName, string email)
    {
        var usuario = new Usuario(userName, "senha", userName, "Admin", true, DateTime.UtcNow, DateTime.UtcNow) { Email = email };
        new UsuarioRepository(_contexto).Cadastrar(usuario);
        return usuario;
    }

    [Fact]
    public async Task Encontra_pelo_usuario()
    {
        var ana = Gravar("ana", "ana@exemplo.com");

        Assert.Equal(ana.Id, (await new UsuarioRepository(_contexto).BuscarUsuario("ana"))?.Id);
    }

    [Fact]
    public async Task Encontra_pelo_email_sem_diferenciar_maiusculas()
    {
        var ana = Gravar("ana", "ana@exemplo.com");

        Assert.Equal(ana.Id, (await new UsuarioRepository(_contexto).BuscarUsuario("Ana@Exemplo.COM"))?.Id);
    }

    [Fact]
    public async Task Usuario_tem_prioridade_sobre_o_email_de_outra_pessoa()
    {
        Gravar("bia", "ana");          // e-mail de uma igual ao usuário da outra
        var ana = Gravar("ana", "ana@exemplo.com");

        Assert.Equal(ana.Id, (await new UsuarioRepository(_contexto).BuscarUsuario("ana"))?.Id);
    }

    [Fact]
    public async Task Inexistente_devolve_null()
    {
        Gravar("ana", "ana@exemplo.com");

        Assert.Null(await new UsuarioRepository(_contexto).BuscarUsuario("ninguem@exemplo.com"));
    }
}
