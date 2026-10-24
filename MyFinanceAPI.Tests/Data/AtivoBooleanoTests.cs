using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using MyFinanceAPI.Data.Context;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Card [DB] contas.ativo e categorias.ativo (24/10/2026): "ativo" é boolean em todos os cadastros,
/// como já era em bancos e listas, e o CHECK 0/1 deixou de existir.
/// </summary>
public class AtivoBooleanoTests
{
    private static IModel Modelo()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        return contexto.GetService<IDesignTimeModel>().Model;
    }

    private static IEntityType Tabela(string nome) => Modelo().GetEntityTypes().Single(e => e.GetTableName() == nome);

    [Theory]
    [InlineData("contas")]
    [InlineData("categorias")]
    public void Ativo_e_booleano_e_obrigatorio(string tabela)
    {
        var ativo = Tabela(tabela).GetProperties().Single(p => p.GetColumnName() == "ativo");

        Assert.Equal(typeof(bool), ativo.ClrType);
        Assert.False(ativo.IsNullable);
    }

    [Theory]
    [InlineData("contas")]
    [InlineData("categorias")]
    public void Nao_ha_check_de_ativo(string tabela)
    {
        Assert.DoesNotContain(Tabela(tabela).GetCheckConstraints(), c => c.Name == $"{tabela}_ativo_check");
    }
}
