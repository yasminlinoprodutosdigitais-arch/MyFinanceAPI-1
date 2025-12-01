using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Context
{
    public class ContextDB : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Banco> Banco { get; set; }
        public DbSet<Lista> Lista { get; set; }
        public DbSet<ItemLista> ItemLista { get; set; }
        public DbSet<MovimentacaoDiaria> MovimentacaoDiaria { get; set; }
        public DbSet<TipoCartao> TipoCartao { get; set; }
        public DbSet<TipoMovimentacao> TipoMovimentacao { get; set; }

        // 👇 DbSets do extrato
        public DbSet<ExtratoBancario> ExtratoBancario { get; set; }
        public DbSet<ExtratoBancarioItem> ExtratoBancarioItens { get; set; }

        public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ------------------------------
            // Transaction (que você já tinha)
            // ------------------------------
            builder.Entity<Transaction>()
                .HasOne(m => m.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(m => m.IdAccount)
                .OnDelete(DeleteBehavior.SetNull);

            // ------------------------------
            // ExtratoBancario
            // ------------------------------
            builder.Entity<ExtratoBancario>(entity =>
            {
                // força usar exatamente essa tabela
                entity.ToTable("ExtratoBancario");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.DataImportacao)
                      .IsRequired();

                entity.Property(e => e.Situacao)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.NomeArquivoOrigem)
                      .HasMaxLength(255);

                // se suas propriedades forem nullable, o EF já entende,
                // aqui é só para garantir nomes e relação.
                entity.Property(e => e.LoteImportacaoId);
                entity.Property(e => e.DataInicioPeriodo);
                entity.Property(e => e.DataFimPeriodo);
                entity.Property(e => e.QuantidadeLancamentos);
                entity.Property(e => e.ValorTotal);
                entity.Property(e => e.BancoId);
                entity.Property(e => e.TipoCartaoId);

                // relação 1:N com ExtratoBancarioItem
                // (ajuste o nome da navigation se na entidade for diferente: Itens/Items)
                entity.HasMany(e => e.Itens)
                      .WithOne(i => i.ExtratoBancario)
                      .HasForeignKey(i => i.ExtratoBancarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------
            // ExtratoBancarioItem
            // ------------------------------
            builder.Entity<ExtratoBancarioItem>(entity =>
            {
                entity.ToTable("ExtratoBancarioItem");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoLancamento)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Descricao)
                      .HasMaxLength(500);

                entity.Property(e => e.NomePessoaTransacao)
                      .HasMaxLength(200);

                entity.Property(e => e.Identificador)
                      .HasMaxLength(200);

                // FKs
                entity.Property(e => e.ExtratoBancarioId);
                entity.Property(e => e.BancoId);
                entity.Property(e => e.TipoCartaoId);
                entity.Property(e => e.TipoMovimentacaoId);
            });
        }
    }
}
