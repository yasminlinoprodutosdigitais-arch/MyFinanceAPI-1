using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>Card #76, fase 1: regras de classificação.</summary>
public class RegraClassificacaoConfiguration : IEntityTypeConfiguration<RegraClassificacao>
{
    public void Configure(EntityTypeBuilder<RegraClassificacao> builder)
    {
        builder.ToTable("regras_classificacao");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Texto).HasMaxLength(120);
        builder.Property(e => e.Campo).HasMaxLength(20).HasDefaultValue(CamposRegra.DescricaoOuPessoa);
        builder.Property(e => e.Operador).HasMaxLength(20).HasDefaultValue(OperadoresRegra.Contem);
        builder.Property(e => e.Ativa).HasDefaultValue(true);
        builder.Property(e => e.Ordem).HasDefaultValue(0);
        builder.Property(e => e.VezesAplicada).HasDefaultValue(0);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");

        // A mesma frase (campo + operador + texto + conta) só uma vez; o texto já vem normalizado ("Ifood" × "IFOOD").
        // NULLS NOT DISTINCT: duas regras "em todas as contas" (banco_id null) também contam como repetidas.
        builder.HasIndex(e => new { e.UsuarioId, e.Campo, e.Operador, e.Texto, e.BancoId }).IsUnique().AreNullsDistinct(false);
        builder.HasIndex(e => new { e.UsuarioId, e.Ordem });

        builder.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        // Apagar a conta apaga a regra só daquela conta.
        builder.HasOne(e => e.Banco).WithMany().HasForeignKey(e => e.BancoId).OnDelete(DeleteBehavior.Cascade);
        // Apagar a categoria, o tipo ou a pessoa não apaga a regra: ela só perde aquele destino.
        builder.HasOne(e => e.Categoria).WithMany().HasForeignKey(e => e.CategoriaId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.TipoMovimentacao).WithMany().HasForeignKey(e => e.TipoMovimentacaoId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.PessoaMovimentacao).WithMany().HasForeignKey(e => e.PessoaMovimentacaoId).OnDelete(DeleteBehavior.SetNull);
    }
}
