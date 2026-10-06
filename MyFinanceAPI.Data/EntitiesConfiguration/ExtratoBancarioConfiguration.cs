using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ExtratoBancarioConfiguration : IEntityTypeConfiguration<ExtratoBancario>
{
    public void Configure(EntityTypeBuilder<ExtratoBancario> builder)
    {
        builder.ToTable("ExtratoBancario");
        builder.HasKey(e => e.Id).HasName("ExtratoBancario_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.DataImportacao).HasDefaultValueSql("now()");
        builder.Property(e => e.ValorTotal).HasPrecision(10, 2).HasDefaultValue(0m);
        builder.Property(e => e.QuantidadeLancamentos).HasDefaultValue(0);
        builder.Property(e => e.Situacao).HasMaxLength(30).HasDefaultValue("Concluido");

        // Coluna que só o banco tinha; nenhum código lê.
        builder.Property<string?>("NomeArquivo");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("extratobancario_user_fkey")
            .OnDelete(DeleteBehavior.Cascade);

        // Catálogo em uso não se apaga (D-8).
        builder.HasOne<Banco>()
            .WithMany()
            .HasForeignKey(e => e.BancoId)
            .HasConstraintName("extratobancario_bancoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);

        // DB-P0-06: TipoCartaoId não tinha FK em lugar nenhum.
        builder.HasOne<TipoCartao>()
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .HasConstraintName("FK_ExtratoBancario_TipoCartao_TipoCartaoId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Itens)
            .WithOne(i => i.ExtratoBancario)
            .HasForeignKey(i => i.ExtratoBancarioId)
            .HasConstraintName("extratobancarioitem_extrato_fkey")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
