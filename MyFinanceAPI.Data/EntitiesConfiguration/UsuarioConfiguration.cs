using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // As colunas que o projeto acrescentou ao IdentityUser e o telefone; as demais nativas (hash,
        // stamps, claims) continuam como o Identity as declara — DB-P3-04.
        builder.Property(e => e.NomeUsuario).HasMaxLength(120);
        builder.Property(e => e.Perfil).HasMaxLength(30);
        builder.Property(e => e.PhoneNumber).HasMaxLength(20);
    }
}
