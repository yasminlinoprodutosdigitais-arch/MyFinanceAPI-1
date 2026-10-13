using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", t =>
        {
            t.HasCheckConstraint("categories_status_check", CheckSql.In("Status", StatusAtivo.Todos));
            // 0..5 = enum NaturezaOperacao do front (Rendimento/Entrada … Empresa).
            t.HasCheckConstraint("categories_naturezaoperacao_check", "\"NaturezaOperacao\" BETWEEN 0 AND 5");
        });
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
