using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(t => t.Id);
        builder.ToTable("Accounts");
        // builder.HasOne(e => e.Category)
        //              .WithMany(c => c.Accounts)
        //              .HasForeignKey(e => e.CategoryId)
        //              .HasConstraintName("fk_category")
        //              .OnDelete(DeleteBehavior.Restrict);
    }
}