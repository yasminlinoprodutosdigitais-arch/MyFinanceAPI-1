using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ExtratoBancarioItemConfiguration : IEntityTypeConfiguration<ExtratoBancarioItem>
{
    public void Configure(EntityTypeBuilder<ExtratoBancarioItem> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("itens_extrato_bancario_tipo_lancamento_check", CheckSql.In("tipo_lancamento", TipoLancamentoExtrato.Todos));
            t.HasCheckConstraint("itens_extrato_bancario_parcelas_check", CheckSql.Parcelas(parcelaMinima: 1));
        });
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.TipoLancamento).HasMaxLength(20);
        builder.Property(e => e.Descricao).HasMaxLength(200);
        builder.Property(e => e.Observacao).HasMaxLength(500);
        builder.Property(e => e.NomePessoaTransacao).HasMaxLength(120);
        builder.Property(e => e.Identificador).HasMaxLength(100);
        builder.Property(e => e.ChaveDescricao).HasMaxLength(200);
        builder.Property(e => e.NumeroFatura).HasMaxLength(20);
        builder.Property(e => e.EhParcelado).HasDefaultValue(false);

        // Colunas que só o banco tinha; nenhum código lê.
        builder.Property<string?>("GrupoParcelamento").HasMaxLength(60);
        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");
        // Gravada uma vez, na criação; o DataAlteracaoInterceptor nunca a regrava (DB-P1-12).
        builder.Property<DateTime>("DataCriacao").HasDefaultValueSql("now()");

        builder.HasIndex(e => e.ExtratoBancarioId);

        // Idempotência da importação (DB-P0-04): a mesma linha do arquivo não entra duas vezes.
        builder.Property(e => e.ChaveImportacao).HasMaxLength(64);
        builder.HasIndex(e => new { e.UsuarioId, e.BancoId, e.ChaveImportacao })
            .IsUnique()
            .HasFilter("\"chave_importacao\" IS NOT NULL")
            .HasDatabaseName("ux_itens_extrato_bancario_chave_importacao");
        // Filtros quentes do extrato (DB-P2-02): mês pela data e cartão de crédito pela fatura.
        // Começam por UserId, então também cobrem a FK; substituem os índices simples de UserId e de DataMovimentacao.
        builder.HasIndex(e => new { e.UsuarioId, e.DataMovimentacao });
        builder.HasIndex(e => new { e.UsuarioId, e.NumeroFatura });

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Banco)
            .WithMany()
            .HasForeignKey(e => e.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoCartao)
            .WithMany()
            .HasForeignKey(e => e.TipoCartaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoMovimentacao)
            .WithMany(t => t.ExtratoBancarioItem)
            .HasForeignKey(e => e.TipoMovimentacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Nome legado: a constraint de CategoriaId se chama "...bancoId_fkey" no banco.
        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => e.CategoriaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.PessoaMovimentacao)
            .WithMany()
            .HasForeignKey(e => e.PessoaMovimentacaoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
