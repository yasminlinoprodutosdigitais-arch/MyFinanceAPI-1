using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TipoMovimentacaoConfiguration : IEntityTypeConfiguration<TipoMovimentacao>
{
    public void Configure(EntityTypeBuilder<TipoMovimentacao> builder)
    {
        builder.ToTable(t =>
            t.HasCheckConstraint("tipos_movimentacao_valor_meta_check", "valor_meta >= 0"));
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.NomeTipoMovimentacao).HasMaxLength(60);
        builder.Property(e => e.Descricao).HasMaxLength(200);

        builder.Property(e => e.ValorMeta).HasPrecision(10, 2).HasDefaultValue(0m);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
