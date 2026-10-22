using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ListaConfiguration : IEntityTypeConfiguration<Lista>
{
    public void Configure(EntityTypeBuilder<Lista> builder)
    {
        builder.ToTable(t =>
            t.HasCheckConstraint("listas_tipo_lista_check", CheckSql.In("tipo_lista", Enum.GetValues<TipoLista>().Select(v => (int)v))));
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Nome).HasMaxLength(120);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
