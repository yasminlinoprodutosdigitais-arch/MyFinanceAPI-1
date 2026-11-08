using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>Card #76, fase 3: partes de um lançamento dividido.</summary>
public class ExtratoItemParteConfiguration : IEntityTypeConfiguration<ExtratoItemParte>
{
    public void Configure(EntityTypeBuilder<ExtratoItemParte> builder)
    {
        builder.ToTable("extrato_item_partes", t => t.HasCheckConstraint("extrato_item_partes_valor_check", "valor > 0"));
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.Descricao).HasMaxLength(200);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");

        builder.HasIndex(e => new { e.ExtratoBancarioItemId, e.Ordem });

        builder.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.ExtratoBancarioItem).WithMany(i => i.Partes).HasForeignKey(e => e.ExtratoBancarioItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Categoria).WithMany().HasForeignKey(e => e.CategoriaId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.TipoMovimentacao).WithMany().HasForeignKey(e => e.TipoMovimentacaoId).OnDelete(DeleteBehavior.SetNull);
        // Como no lançamento: a pessoa usada em uma parte não pode ser apagada (o cadastro avisa antes).
        builder.HasOne(e => e.PessoaMovimentacao).WithMany().HasForeignKey(e => e.PessoaMovimentacaoId).OnDelete(DeleteBehavior.NoAction);
    }
}
