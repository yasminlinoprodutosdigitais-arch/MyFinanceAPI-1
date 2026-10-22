using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class BancoConfiguration : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.NomeBanco).HasMaxLength(120);
        builder.Property(e => e.Agencia).HasMaxLength(10);
        builder.Property(e => e.NumeroConta).HasMaxLength(20);

        builder.Property(e => e.SaldoInicial).HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(e => e.Ativo).HasDefaultValue(true);
        builder.Property(e => e.DataAlteracao).HasDefaultValueSql("now()");

        builder.HasIndex(e => new { e.UsuarioId, e.NomeBanco })
            .IsUnique();

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TipoCartao)
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
