using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ItemListaConfiguration : IEntityTypeConfiguration<ItemLista>
{
    public void Configure(EntityTypeBuilder<ItemLista> builder)
    {
        builder.ToTable("ItemLista");
        builder.HasKey(e => e.Id).HasName("itemlista_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Descricao).HasMaxLength(200);
        builder.Property(e => e.Valor).HasPrecision(10, 2);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_ItemLista_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Lista)
            .WithMany()
            .HasForeignKey(e => e.ListaId)
            .HasConstraintName("itemlista_listaid_fkey")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
