using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Context;
public class ContextDB : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Transaction>()
            .HasOne(m => m.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(m => m.IdAccount)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
