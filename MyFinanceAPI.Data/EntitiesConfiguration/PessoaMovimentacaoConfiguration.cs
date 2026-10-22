using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class PessoaMovimentacaoConfiguration : IEntityTypeConfiguration<PessoaMovimentacao>
{
    public void Configure(EntityTypeBuilder<PessoaMovimentacao> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.NomePessoa).HasMaxLength(120);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        // DB-P0-04: o nome é gravado normalizado (trim + maiúsculas) pelo repositório.
        builder.HasIndex(e => new { e.UsuarioId, e.NomePessoa })
            .IsUnique();

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
