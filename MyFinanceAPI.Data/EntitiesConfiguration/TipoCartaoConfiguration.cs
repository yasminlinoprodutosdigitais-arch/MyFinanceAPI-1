using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class TipoCartaoConfiguration : IEntityTypeConfiguration<TipoCartao>
{
    public void Configure(EntityTypeBuilder<TipoCartao> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.NomeTipoCartao).HasMaxLength(60);
        builder.Property(e => e.Descricao).HasMaxLength(200);

        builder.HasIndex(e => new { e.UsuarioId, e.NomeTipoCartao })
            .IsUnique();

        // Tipos padrão do sistema (UsuarioId nulo): o índice acima não pega duplicatas, porque NULLs são distintos.
        builder.HasIndex(e => e.NomeTipoCartao)
            .IsUnique()
            .HasFilter("\"usuario_id\" IS NULL")
            .HasDatabaseName("ux_tipos_cartao_sistema_nome_tipo_cartao");

        builder.Ignore(e => e.EhSistema);

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
