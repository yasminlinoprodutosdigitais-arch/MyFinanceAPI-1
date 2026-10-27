using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    public void Configure(EntityTypeBuilder<Conta> builder)
    {
        builder.ToTable(t =>
        {
            // Conta não parcelada usa 0/0; parcelada começa na parcela 0 (nenhuma paga) — DB-P1-05.
            t.HasCheckConstraint("contas_parcelas_check", "NOT eh_parcelado OR (" + CheckSql.Parcelas(parcelaMinima: 0) + ")");
        });
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome).HasMaxLength(120);
        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.Observacao).HasMaxLength(500);
        // Contas que já existem ficam como valor não fixo (decisão do card #47).
        builder.Property(e => e.ValorFixo).HasDefaultValue(false);

        // Coluna que só o banco tinha; nenhum código lê, então fica como shadow property.
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // DB-P0-03: categoria com contas não se apaga (a guarda do CategoryService passa a valer no banco).
        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
