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

        // constraint_1 / constraint_2: nomes genéricos herdados do banco (renomear no subprojeto 3).
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("constraint_1")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .HasConstraintName("constraint_2")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .HasConstraintName("FK_PessoaMovimentacao_Categories_CategoriaId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
