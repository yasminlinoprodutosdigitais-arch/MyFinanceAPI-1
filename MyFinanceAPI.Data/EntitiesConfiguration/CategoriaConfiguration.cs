using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable(t =>
        {
            // 0..5 = enum NaturezaOperacao do front (Rendimento/Entrada … Empresa).
            t.HasCheckConstraint("categorias_natureza_operacao_check", "natureza_operacao BETWEEN 0 AND 5");
        });
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome).HasMaxLength(120);
        builder.Property(e => e.Subcategoria).HasMaxLength(120);
        builder.Property(e => e.Icone).HasMaxLength(40);
        builder.Property(e => e.Cor).HasMaxLength(20);
        builder.Property(e => e.DataAlteracao).HasDefaultValueSql("now()");

        builder.HasIndex(e => new { e.UsuarioId, e.Nome, e.Subcategoria })
            .IsUnique();

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
