using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class MovimentacaoDiariaConfiguration : IEntityTypeConfiguration<MovimentacaoDiaria>
{
    public void Configure(EntityTypeBuilder<MovimentacaoDiaria> builder)
    {
        builder.ToTable("MovimentacaoDiaria");
        builder.HasKey(e => e.Id).HasName("movimentacaodiaria_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Valor).HasPrecision(10, 2);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_MovimentacaoDiaria_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        // Banco com lançamentos não se apaga; inativa-se (D-2).
        builder.HasOne(e => e.Banco)
            .WithMany()
            .HasForeignKey(e => e.BancoId)
            .HasConstraintName("movimentacaodiaria_bancoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoCartao)
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .HasConstraintName("movimentacaodiaria_tipocartaoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .HasConstraintName("movimentacaodiaria_tipomovimentacaoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
