using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(e => e.Id).HasName("PK_Transactions");

        builder.Property(e => e.Value).HasPrecision(10, 2);
        builder.Property(e => e.EhParcelado).HasDefaultValue(false);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_Transactions_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        // Transação é histórico: conta com transação não se apaga (D-9).
        builder.HasOne(e => e.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(e => e.IdAccount)
            .HasConstraintName("FK_Transactions_Accounts_IdAccount")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .HasConstraintName("FK_Transactions_Categories_CategoryId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
