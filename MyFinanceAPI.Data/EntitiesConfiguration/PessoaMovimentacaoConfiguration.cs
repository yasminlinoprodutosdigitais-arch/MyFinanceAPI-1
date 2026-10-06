using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class PessoaMovimentacaoConfiguration : IEntityTypeConfiguration<PessoaMovimentacao>
{
    public void Configure(EntityTypeBuilder<PessoaMovimentacao> builder)
    {
        builder.ToTable("PessoaMovimentacao");
        builder.HasKey(e => e.Id).HasName("PessoaMovimentacao_pkey");

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        // DB-P0-04: o nome é gravado normalizado (trim + maiúsculas) pelo repositório.
        builder.HasIndex(e => new { e.UserId, e.NomePessoa })
            .IsUnique()
            .HasDatabaseName("UX_PessoaMovimentacao_UserId_NomePessoa");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_PessoaMovimentacao_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .HasConstraintName("FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .HasConstraintName("FK_PessoaMovimentacao_Categories_CategoriaId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
