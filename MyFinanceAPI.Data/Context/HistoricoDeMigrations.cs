using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace MyFinanceAPI.Data.Context;

/// <summary>
/// O UseSnakeCaseNamingConvention também reescreveria as colunas de "__EFMigrationsHistory"
/// (migration_id, product_version), e os bancos existentes têm "MigrationId"/"ProductVersion":
/// o histórico fica fora da convenção (DB-P3).
/// </summary>
#pragma warning disable EF1001 // NpgsqlHistoryRepository é API interna do provider; é o ponto de extensão documentado.
public class HistoricoDeMigrations(HistoryRepositoryDependencies dependencies) : NpgsqlHistoryRepository(dependencies)
{
    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);
        history.Property(h => h.MigrationId).HasColumnName("MigrationId");
        history.Property(h => h.ProductVersion).HasColumnName("ProductVersion");
    }
}
#pragma warning restore EF1001
