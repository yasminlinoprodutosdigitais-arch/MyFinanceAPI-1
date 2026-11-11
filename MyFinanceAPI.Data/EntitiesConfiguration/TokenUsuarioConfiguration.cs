using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>Card #50: hash único (SHA-256 em hex); apaga junto com o usuário.</summary>
public class TokenUsuarioConfiguration : IEntityTypeConfiguration<TokenUsuario>
{
    public void Configure(EntityTypeBuilder<TokenUsuario> builder)
    {
        builder.Property(e => e.Hash).HasMaxLength(64);
        builder.HasIndex(e => e.Hash).IsUnique();
        builder.HasIndex(e => new { e.UsuarioId, e.Tipo });
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");
        builder.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
    }
}
