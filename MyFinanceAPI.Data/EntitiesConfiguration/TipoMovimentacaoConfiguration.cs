using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TipoMovimentacaoConfiguration : IEntityTypeConfiguration<TipoMovimentacao>
{
    public void Configure(EntityTypeBuilder<TipoMovimentacao> builder)
    {
        builder.ToTable("TipoMovimentacao", t =>
            t.HasCheckConstraint("tipomovimentacao_valormeta_check", "\"ValorMeta\" >= 0"));
        builder.HasKey(e => e.Id).HasName("tipomovimentacao_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.ValorMeta).HasPrecision(10, 2).HasDefaultValue(0m);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("fk_tipomovimentacao_aspnetusers")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
