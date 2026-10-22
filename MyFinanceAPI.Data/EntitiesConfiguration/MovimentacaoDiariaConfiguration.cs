using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class MovimentacaoDiariaConfiguration : IEntityTypeConfiguration<MovimentacaoDiaria>
{
    public void Configure(EntityTypeBuilder<MovimentacaoDiaria> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.TipoLancamento).HasMaxLength(20);
        builder.Property(e => e.Descricao).HasMaxLength(200);
        builder.Property(e => e.NomePessoaTransacao).HasMaxLength(120);
        builder.Property(e => e.Identificador).HasMaxLength(100);

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Banco com lançamentos não se apaga; inativa-se (D-2).
        builder.HasOne(e => e.Banco)
            .WithMany()
            .HasForeignKey(e => e.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoCartao)
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
