using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ContaVencimentoConfiguration : IEntityTypeConfiguration<ContaVencimento>
{
    public void Configure(EntityTypeBuilder<ContaVencimento> builder)
    {
        builder.ToTable("ContaVencimento");
        builder.HasKey(e => e.Id).HasName("ContaVencimento_pkey");

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("contavencimento_user_fkey")
            .OnDelete(DeleteBehavior.Cascade);

        // Vencimento é parte da conta: some com ela (D-9).
        builder.HasOne(e => e.Account)
            .WithMany(a => a.ContaVencimentos)
            .HasForeignKey(e => e.ContaId)
            .HasConstraintName("ContaVencimento_ContaId_fkey")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
