using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(e => e.Id).HasName("PK_Accounts");

        builder.Property(e => e.Value).HasPrecision(10, 2);

        // Colunas que só o banco tinha; nenhum código lê, então ficam como shadow properties.
        builder.Property<string?>("DataOperacao");
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Accounts_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        // DB-P0-03: categoria com contas não se apaga (a guarda do CategoryService passa a valer no banco).
        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .HasConstraintName("FK_Accounts_Categories_CategoryId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
