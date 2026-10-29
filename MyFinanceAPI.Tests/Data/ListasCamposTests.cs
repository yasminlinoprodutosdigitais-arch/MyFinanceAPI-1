using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using MyFinanceAPI.Data.Context;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Card #67 Listas com campos configuráveis (28/10/2026): o tipo fixo virou um conjunto de campos por
/// lista, "inativa" virou arquivada_em, e o item tem só feito/pendente.
/// </summary>
public class ListasCamposTests
{
    private static IModel Modelo()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        return contexto.GetService<IDesignTimeModel>().Model;
    }

    private static IEntityType Tabela(string nome) => Modelo().GetEntityTypes().Single(e => e.GetTableName() == nome);

    private static IEnumerable<string> Colunas(string tabela) => Tabela(tabela).GetProperties().Select(p => p.GetColumnName());

    [Theory]
    [InlineData("icone")]
    [InlineData("usa_quantidade")]
    [InlineData("usa_valor")]
    [InlineData("modo_valor")]
    [InlineData("usa_limite")]
    [InlineData("limite")]
    [InlineData("usa_data")]
    [InlineData("usa_observacao")]
    [InlineData("modelo")]
    [InlineData("arquivada_em")]
    public void Listas_tem_a_configuracao_dos_campos(string coluna)
    {
        Assert.Contains(coluna, Colunas("listas"));
    }

    [Theory]
    [InlineData("tipo_lista")]
    [InlineData("ativo")]
    public void Listas_nao_tem_mais_tipo_nem_ativo(string coluna)
    {
        Assert.DoesNotContain(coluna, Colunas("listas"));
    }

    [Theory]
    [InlineData("feito")]
    [InlineData("feito_em")]
    [InlineData("ordem")]
    [InlineData("data")]
    public void Itens_tem_feito_ordem_e_data(string coluna)
    {
        Assert.Contains(coluna, Colunas("itens_lista"));
    }

    [Theory]
    [InlineData("situacao")]
    [InlineData("data_tarefa")]
    public void Itens_nao_tem_mais_situacao_nem_data_tarefa(string coluna)
    {
        Assert.DoesNotContain(coluna, Colunas("itens_lista"));
    }

    [Theory]
    [InlineData("listas", "listas_limite_com_valor_check", "NOT usa_limite OR usa_valor")]
    [InlineData("listas", "listas_limite_check", "limite >= 0")]
    [InlineData("listas", "listas_modo_valor_check", "modo_valor IN ('UNITARIO', 'TOTAL')")]
    [InlineData("listas", "listas_modelo_check", "modelo IN ('COMPRAS', 'ORCAMENTO', 'TAREFAS', 'AGENDA', 'BRANCO')")]
    [InlineData("itens_lista", "itens_lista_quantidade_check", "quantidade >= 1")]
    [InlineData("itens_lista", "itens_lista_valor_check", "valor >= 0")]
    public void Regras_dos_campos_estao_no_banco(string tabela, string nome, string sql)
    {
        var check = Assert.Single(Tabela(tabela).GetCheckConstraints(), c => c.Name == nome);
        Assert.Equal(sql, check.Sql);
    }

    [Theory]
    [InlineData("listas", "listas_tipo_lista_check")]
    [InlineData("itens_lista", "itens_lista_situacao_check")]
    public void Checks_antigos_sairam(string tabela, string nome)
    {
        Assert.DoesNotContain(Tabela(tabela).GetCheckConstraints(), c => c.Name == nome);
    }

    [Fact]
    public void Lista_tem_serie_obrigatoria_gerada_pelo_banco_e_origem_opcional()
    {
        var listas = Tabela("listas");
        var serie = listas.GetProperties().Single(p => p.GetColumnName() == "serie_id");
        var origem = listas.GetProperties().Single(p => p.GetColumnName() == "lista_origem_id");

        Assert.Equal(typeof(Guid), serie.ClrType);
        Assert.False(serie.IsNullable);
        Assert.Equal("gen_random_uuid()", serie.GetDefaultValueSql());
        Assert.True(origem.IsNullable);
        Assert.Contains(listas.GetIndexes(), i => i.Properties.Select(p => p.GetColumnName()).SequenceEqual(["usuario_id", "serie_id"]));
    }

    [Theory]
    [InlineData("listas", "lista_origem_id")]
    [InlineData("itens_lista", "item_origem_id")]
    public void Apagar_a_origem_so_solta_a_copia(string tabela, string coluna)
    {
        var fk = Assert.Single(Tabela(tabela).GetForeignKeys(), f => f.Properties.Single().GetColumnName() == coluna);

        Assert.Equal(DeleteBehavior.SetNull, fk.DeleteBehavior);
        Assert.Equal(tabela, fk.PrincipalEntityType.GetTableName());
    }

    [Theory]
    [InlineData("hora")]
    [InlineData("repeticao")]
    [InlineData("dias_semana")]
    [InlineData("repetir_ate")]
    public void Itens_tem_hora_e_regra_de_repeticao(string coluna)
    {
        Assert.Contains(coluna, Colunas("itens_lista"));
        Assert.DoesNotContain("horario_tarefa", Colunas("itens_lista"));
        Assert.DoesNotContain("dia_todo", Colunas("itens_lista"));
    }

    [Theory]
    [InlineData("itens_lista_repeticao_check", "repeticao IN ('NENHUMA', 'DIARIA', 'SEMANAL', 'MENSAL')")]
    [InlineData("itens_lista_repeticao_com_data_check", "repeticao = 'NENHUMA' OR data IS NOT NULL")]
    [InlineData("itens_lista_semanal_com_dias_check", "repeticao <> 'SEMANAL' OR cardinality(dias_semana) > 0")]
    [InlineData("itens_lista_dias_semana_check", "dias_semana <@ ARRAY[1, 2, 3, 4, 5, 6, 7]")]
    [InlineData("itens_lista_repetir_ate_check", "repetir_ate >= data")]
    public void Regras_da_repeticao_estao_no_banco(string nome, string sql)
    {
        var check = Assert.Single(Tabela("itens_lista").GetCheckConstraints(), c => c.Name == nome);
        Assert.Equal(sql, check.Sql);
    }

    [Fact]
    public void Ocorrencias_guardam_so_o_que_difere_da_regra_uma_por_data()
    {
        var ocorrencias = Tabela("itens_lista_ocorrencias");

        Assert.Equal(
            new[] { "data", "data_alteracao", "feito", "feito_em", "id", "item_id", "nova_data", "nova_hora", "pulada", "usuario_id" },
            ocorrencias.GetProperties().Select(p => p.GetColumnName()).Order());
        Assert.Contains(ocorrencias.GetIndexes(), i => i.IsUnique && i.Properties.Select(p => p.GetColumnName()).SequenceEqual(["item_id", "data"]));
        var item = Assert.Single(ocorrencias.GetForeignKeys(), f => f.Properties.Single().GetColumnName() == "item_id");
        Assert.Equal(DeleteBehavior.Cascade, item.DeleteBehavior);
    }

    [Fact]
    public void Feito_e_os_switches_sao_obrigatorios_com_padrao_false()
    {
        foreach (var (tabela, coluna) in new[] { ("listas", "usa_valor"), ("listas", "usa_quantidade"), ("itens_lista", "feito") })
        {
            var p = Tabela(tabela).GetProperties().Single(x => x.GetColumnName() == coluna);
            Assert.False(p.IsNullable);
            Assert.Equal(false, p.GetDefaultValue());
        }
    }
}
