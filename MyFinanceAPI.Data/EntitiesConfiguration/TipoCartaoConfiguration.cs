using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TipoCartaoConfiguration : IEntityTypeConfiguration<TipoCartao>
{
    public void Configure(EntityTypeBuilder<TipoCartao> builder)
    {
        builder.ToTable("TipoCartao");
        builder.HasKey(e => e.Id).HasName("tipocartao_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.HasIndex(e => new { e.UserId, e.NomeTipoCartao })
            .IsUnique()
            .HasDatabaseName("UX_TipoCartao_UserId_NomeTipoCartao");

        // Tipos padrão do sistema (UserId nulo): o índice acima não pega duplicatas, porque NULLs são distintos.
        builder.HasIndex(e => e.NomeTipoCartao)
            .IsUnique()
            .HasFilter("\"UserId\" IS NULL")
            .HasDatabaseName("UX_TipoCartao_Sistema_NomeTipoCartao");

        builder.Ignore(e => e.EhSistema);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("fk_tipocartao_aspnetusers")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
