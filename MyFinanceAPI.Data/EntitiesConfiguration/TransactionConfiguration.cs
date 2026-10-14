using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions", t =>
        {
            t.HasCheckConstraint("transactions_status_check", CheckSql.In("Status", StatusTransacao.Todos));
            t.HasCheckConstraint("transactions_parcelas_check", CheckSql.Parcelas(parcelaMinima: 1));
        });
        builder.HasKey(e => e.Id).HasName("PK_Transactions");

        builder.Property(e => e.Value).HasPrecision(10, 2);
        builder.Property(e => e.EhParcelado).HasDefaultValue(false);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        // Gravada uma vez, na criação; o DataAlteracaoInterceptor nunca a regrava (DB-P1-12).
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");

        // Contas a Pagar, Home e vencidas filtram por usuário + data (DB-P2-02); também cobre a FK de UserId.
        builder.HasIndex(e => new { e.UserId, e.Date }).HasDatabaseName("IX_Transactions_UserId_Date");

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
