using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class OcorrenciaItemListaConfiguration : IEntityTypeConfiguration<OcorrenciaItemLista>
{
    public void Configure(EntityTypeBuilder<OcorrenciaItemLista> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        // Uma linha por ocorrência (a data original), só quando ela difere da regra do item.
        builder.HasIndex(e => new { e.ItemId, e.Data }).IsUnique();

        builder.Property(e => e.Feito).HasDefaultValue(false);
        builder.Property(e => e.Pulada).HasDefaultValue(false);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // A ocorrência é parte do item: some com ele.
        builder.HasOne<ItemLista>()
            .WithMany()
            .HasForeignKey(e => e.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
