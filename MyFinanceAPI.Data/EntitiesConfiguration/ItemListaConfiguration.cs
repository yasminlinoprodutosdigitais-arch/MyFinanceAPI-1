using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ItemListaConfiguration : IEntityTypeConfiguration<ItemLista>
{
    public void Configure(EntityTypeBuilder<ItemLista> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("itens_lista_situacao_check", CheckSql.In("situacao", StatusItemLista.Todos));
            t.HasCheckConstraint("itens_lista_quantidade_check", "quantidade >= 0");
        });
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Descricao).HasMaxLength(200);
        builder.Property(e => e.Observacao).HasMaxLength(500);
        builder.Property(e => e.Situacao).HasMaxLength(20);
        builder.Property(e => e.Valor).HasPrecision(10, 2);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Lista)
            .WithMany()
            .HasForeignKey(e => e.ListaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
