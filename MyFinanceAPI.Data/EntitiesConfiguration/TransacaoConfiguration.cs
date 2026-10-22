using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("transacoes_situacao_check", CheckSql.In("situacao", StatusTransacao.Todos));
            t.HasCheckConstraint("transacoes_parcelas_check", CheckSql.Parcelas(parcelaMinima: 1));
        });
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome).HasMaxLength(120);
        builder.Property(e => e.Situacao).HasMaxLength(20);
        builder.Property(e => e.Observacao).HasMaxLength(500);
        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.EhParcelado).HasDefaultValue(false);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        // Gravada uma vez, na criação; o DataAlteracaoInterceptor nunca a regrava (DB-P1-12).
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");

        // Contas a Pagar, Home e vencidas filtram por usuário + data (DB-P2-02); também cobre a FK de UserId.
        builder.HasIndex(e => new { e.UsuarioId, e.Data });

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Transação é histórico: conta com transação não se apaga (D-9).
        builder.HasOne(e => e.Conta)
            .WithMany(a => a.Transacoes)
            .HasForeignKey(e => e.ContaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
