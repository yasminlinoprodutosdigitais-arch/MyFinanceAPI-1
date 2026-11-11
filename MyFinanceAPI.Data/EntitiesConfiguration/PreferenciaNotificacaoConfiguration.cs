using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>Card #73: uma linha por usuário; apaga junto com ele.</summary>
public class PreferenciaNotificacaoConfiguration : IEntityTypeConfiguration<PreferenciaNotificacao>
{
    public void Configure(EntityTypeBuilder<PreferenciaNotificacao> builder)
    {
        builder.HasKey(e => e.UsuarioId);
        builder.Property(e => e.UsuarioId).ValueGeneratedNever();
        builder.Property(e => e.Config).HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");
        builder.HasOne(e => e.Usuario).WithOne().HasForeignKey<PreferenciaNotificacao>(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}
