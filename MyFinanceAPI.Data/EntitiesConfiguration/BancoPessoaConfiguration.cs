using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>Quem usa cada cartão compartilhado (card #77). Apagar o banco ou a pessoa apaga a ligação.</summary>
public class BancoPessoaConfiguration : IEntityTypeConfiguration<BancoPessoa>
{
    public void Configure(EntityTypeBuilder<BancoPessoa> builder)
    {
        builder.HasKey(e => new { e.BancoId, e.PessoaMovimentacaoId });

        builder.HasOne(e => e.Banco)
            .WithMany(b => b.Pessoas)
            .HasForeignKey(e => e.BancoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.PessoaMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.PessoaMovimentacaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
