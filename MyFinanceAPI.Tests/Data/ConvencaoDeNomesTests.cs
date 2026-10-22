using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyFinanceAPI.Data.Context;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// DB-P3: todo nome físico do modelo real (Npgsql; montar o modelo não abre conexão) é snake_case
/// e cabe nos 63 caracteres do Postgres — tabela, coluna, PK, FK, índice e CHECK.
/// </summary>
public class ConvencaoDeNomesTests
{
    private static readonly Regex SnakeCase = new("^[a-z][a-z0-9_]*$");

    private static IModel Modelo()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        return contexto.GetService<IDesignTimeModel>().Model;
    }

    private static IEnumerable<string> NomesFisicos()
    {
        foreach (var entidade in Modelo().GetEntityTypes())
        {
            var objeto = StoreObjectIdentifier.Table(entidade.GetTableName()!, entidade.GetSchema());
            yield return objeto.Name;
            foreach (var p in entidade.GetProperties()) yield return p.GetColumnName(objeto)!;
            foreach (var k in entidade.GetKeys().Where(k => k.IsPrimaryKey())) yield return k.GetName()!;
            foreach (var fk in entidade.GetForeignKeys()) yield return fk.GetConstraintName()!;
            foreach (var i in entidade.GetIndexes()) yield return i.GetDatabaseName()!;
            foreach (var c in entidade.GetCheckConstraints()) yield return c.Name!;
        }
    }

    [Fact]
    public void Todo_nome_fisico_e_snake_case()
    {
        Assert.DoesNotContain(NomesFisicos(), n => !SnakeCase.IsMatch(n));
    }

    [Fact]
    public void Nenhum_nome_passa_do_limite_do_Postgres()
    {
        // O Postgres corta identificadores maiores sem erro.
        Assert.DoesNotContain(NomesFisicos(), n => n.Length > 63);
    }

    /// <summary>Os exemplos da spec do card: pk_, fk_ e ix_ da convenção; CHECK no estilo nativo do Postgres.</summary>
    [Theory]
    [InlineData("pk_contas")]
    [InlineData("fk_transacoes_contas_conta_id")]
    [InlineData("ix_transacoes_usuario_id_data")]
    [InlineData("transacoes_situacao_check")]
    [InlineData("contas_ativo_check")]
    [InlineData("itens_extrato_bancario_tipo_lancamento_check")]
    [InlineData("pk_asp_net_users")]
    [InlineData("fk_transacoes_asp_net_users_usuario_id")]
    [InlineData("fk_itens_extrato_bancario_extrato_bancario_id")]
    public void Nome_da_spec_existe_no_modelo(string nome)
    {
        Assert.Contains(nome, NomesFisicos());
    }

    /// <summary>
    /// FK é "fk_{tabela}_{principal}_{colunas}"; se passar de 63, "fk_{tabela}_{colunas}" — e não o
    /// corte silencioso do EF no meio da palavra, nem o nome antigo da tabela principal.
    /// </summary>
    [Fact]
    public void Fk_nomeada_pelas_tabelas_atuais_sem_corte()
    {
        var divergentes =
            from entidade in Modelo().GetEntityTypes()
            let tabela = entidade.GetTableName()!
            let objeto = StoreObjectIdentifier.Table(tabela, entidade.GetSchema())
            from fk in entidade.GetForeignKeys()
            let colunas = string.Join("_", fk.Properties.Select(p => p.GetColumnName(objeto)))
            let completo = $"fk_{tabela}_{fk.PrincipalEntityType.GetTableName()}_{colunas}"
            let esperado = completo.Length <= 63 ? completo : $"fk_{tabela}_{colunas}"
            where fk.GetConstraintName() != esperado
            select $"{fk.GetConstraintName()} (esperado {esperado})";

        Assert.Empty(divergentes);
    }

    /// <summary>
    /// A convenção também reescreveria o histórico de migrations; o banco tem "MigrationId" e
    /// "ProductVersion", e o dotnet ef database update quebraria antes de aplicar qualquer migration.
    /// </summary>
    [Fact]
    public void Historico_de_migrations_mantem_as_colunas_do_banco()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);

        var script = contexto.GetService<IHistoryRepository>().GetCreateScript();

        Assert.Contains("\"__EFMigrationsHistory\"", script);
        Assert.Contains("\"MigrationId\"", script);
        Assert.Contains("\"ProductVersion\"", script);
    }

    [Fact]
    public void Tabelas_de_dominio_no_plural_em_portugues()
    {
        var tabelas = Modelo().GetEntityTypes().Select(e => e.GetTableName()).Where(t => !t!.StartsWith("asp_net_"));

        Assert.Equal(
            new[] { "bancos", "categorias", "contas", "contas_vencimento", "extratos_bancarios", "itens_extrato_bancario",
                    "itens_lista", "listas", "movimentacoes_diarias", "pessoas_movimentacao", "tipos_cartao",
                    "tipos_movimentacao", "transacoes" },
            tabelas.Order(StringComparer.Ordinal));
    }
}
