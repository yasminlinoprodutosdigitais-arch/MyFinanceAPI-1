using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(e => e.Id).HasName("PK_Categories");

        builder.Property(e => e.DataAlteracao).HasDefaultValueSql("now()");

        builder.HasIndex(e => new { e.UserId, e.Name, e.SubCategory })
            .IsUnique()
            .HasDatabaseName("UX_Categories_UserId_Name_SubCategory");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Categories_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
