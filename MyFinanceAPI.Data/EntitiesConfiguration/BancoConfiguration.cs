using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class BancoConfiguration : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.ToTable("Banco");
        builder.HasKey(e => e.Id).HasName("banco_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.SaldoInicial).HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(e => e.Ativo).HasDefaultValue(true);
        builder.Property(e => e.DataAlteracao).HasDefaultValueSql("now()");

        // "UerId": grafia legada do nome da constraint no banco.
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Banco_AspNetUsers_UerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TipoCartao)
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .HasConstraintName("fk_banco_tipocartao")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
