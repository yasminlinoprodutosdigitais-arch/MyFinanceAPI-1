using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ExtratoBancarioItemConfiguration : IEntityTypeConfiguration<ExtratoBancarioItem>
{
    public void Configure(EntityTypeBuilder<ExtratoBancarioItem> builder)
    {
        builder.ToTable("ExtratoBancarioItem");
        builder.HasKey(e => e.Id).HasName("ExtratoBancarioItem_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.ChaveDescricao).HasMaxLength(200);
        builder.Property(e => e.NumeroFatura).HasMaxLength(20);
        builder.Property(e => e.EhParcelado).HasDefaultValue(false);

        // Colunas que só o banco tinha; nenhum código lê.
        builder.Property<string?>("GrupoParcelamento").HasMaxLength(60);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasIndex(e => e.ExtratoBancarioId).HasDatabaseName("idx_extratobancarioitem_extrato");
        builder.HasIndex(e => e.DataMovimentacao).HasDatabaseName("idx_extratobancarioitem_datamov");

        // Idempotência da importação (DB-P0-04): a mesma linha do arquivo não entra duas vezes.
        builder.Property(e => e.ChaveImportacao).HasMaxLength(64);
        builder.HasIndex(e => new { e.UserId, e.BancoId, e.ChaveImportacao })
            .IsUnique()
            .HasFilter("\"ChaveImportacao\" IS NOT NULL")
            .HasDatabaseName("UX_ExtratoBancarioItem_ChaveImportacao");
        // A unique acima é parcial (fora os itens manuais) e não serve às consultas por UserId:
        // sem esta declaração o EF a trataria como cobertura da FK e removeria o índice simples.
        builder.HasIndex(e => e.UserId).HasDatabaseName("IX_ExtratoBancarioItem_UserId");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_ExtratoBancarioItem_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Banco)
            .WithMany()
            .HasForeignKey(e => e.BancoId)
            .HasConstraintName("extratobancarioitem_bancoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoCartao)
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .HasConstraintName("extratobancarioitem_tipocartaoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany(t => t.ExtratoBancarioItem)
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .HasConstraintName("extratobancarioitem_tipomovimentacaoid_fkey")
            .OnDelete(DeleteBehavior.Restrict);

        // Nome legado: a constraint de CategoriaId se chama "...bancoId_fkey" no banco.
        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .HasConstraintName("extratoBancarioItem_bancoId_fkey")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.PessoaMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.PessoaMovimentacaoId)
            .HasConstraintName("extratobancarioitem_pessoamovimentacaoid_fkey")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
