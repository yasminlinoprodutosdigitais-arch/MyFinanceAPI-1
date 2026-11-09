using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>Card #76, fase 6: nota fiscal do lançamento (uma por lançamento; apaga junto com ele).</summary>
public class NotaFiscalConfiguration : IEntityTypeConfiguration<NotaFiscal>
{
    public void Configure(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("notas_fiscais");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Estabelecimento).HasMaxLength(200);
        builder.Property(e => e.Total).HasPrecision(10, 2);
        builder.Property(e => e.Link).HasMaxLength(300);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");
        builder.HasIndex(e => e.ExtratoBancarioItemId).IsUnique();
        builder.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.ExtratoBancarioItem).WithOne(i => i.NotaFiscal).HasForeignKey<NotaFiscal>(e => e.ExtratoBancarioItemId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotaFiscalItemConfiguration : IEntityTypeConfiguration<NotaFiscalItem>
{
    public void Configure(EntityTypeBuilder<NotaFiscalItem> builder)
    {
        builder.ToTable("notas_fiscais_itens");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).HasMaxLength(200);
        builder.Property(e => e.Quantidade).HasPrecision(12, 4);
        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");
        builder.HasIndex(e => new { e.NotaFiscalId, e.Ordem });
        builder.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.NotaFiscal).WithMany(n => n.Itens).HasForeignKey(e => e.NotaFiscalId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Categoria).WithMany().HasForeignKey(e => e.CategoriaId).OnDelete(DeleteBehavior.SetNull);
    }
}
