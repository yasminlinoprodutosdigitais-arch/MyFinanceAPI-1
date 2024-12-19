using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Context;
public class ContextDB : IdentityDbContext<User>
{
    public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Transaction>()
        .HasOne(m => m.Account)                // Relaciona MonthlyUpdate com Account
        .WithMany(a => a.Transactions)       // Uma Account pode ter vários MonthlyUpdates
        .HasForeignKey(m => m.IdAccount)       // Especifica que a chave estrangeira é IdAccount, não AccountId
        .OnDelete(DeleteBehavior.SetNull);
    }
}
