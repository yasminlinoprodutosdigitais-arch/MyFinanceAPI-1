using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ListaConfiguration : IEntityTypeConfiguration<Lista>
{
    public void Configure(EntityTypeBuilder<Lista> builder)
    {
        builder.ToTable("Lista");
        builder.HasKey(e => e.Id).HasName("lista_pkey");
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Lista_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
