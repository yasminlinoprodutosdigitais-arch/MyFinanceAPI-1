using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ContaVencimentoConfiguration : IEntityTypeConfiguration<ContaVencimento>
{
    public void Configure(EntityTypeBuilder<ContaVencimento> builder)
    {
        builder.ToTable(t =>
            t.HasCheckConstraint("contas_vencimento_dia_check", "dia BETWEEN 1 AND 31"));
        builder.HasKey(e => e.Id);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vencimento é parte da conta: some com ela (D-9).
        builder.HasOne(e => e.Conta)
            .WithMany(a => a.Vencimentos)
            .HasForeignKey(e => e.ContaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
