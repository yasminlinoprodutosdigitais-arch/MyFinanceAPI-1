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
                public DbSet<ContaVencimento> ContaVencimento { get; set; }
                public DbSet<Category> Categories { get; set; }
                public DbSet<Transaction> Transactions { get; set; }
                public DbSet<Banco> Banco { get; set; }
                public DbSet<Lista> Lista { get; set; }
                public DbSet<ItemLista> ItemLista { get; set; }
                public DbSet<MovimentacaoDiaria> MovimentacaoDiaria { get; set; }
                public DbSet<TipoCartao> TipoCartao { get; set; }
                public DbSet<TipoMovimentacao> TipoMovimentacao { get; set; }
                public DbSet<PessoaMovimentacao> PessoaMovimentacao { get; set; }

                // 👇 DbSets do extrato
                public DbSet<ExtratoBancario> ExtratoBancario { get; set; }
                public DbSet<ExtratoBancarioItem> ExtratoBancarioItens { get; set; }

                public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }
                protected override void OnModelCreating(ModelBuilder builder)
                {
                        base.OnModelCreating(builder);

                        // Uma classe IEntityTypeConfiguration<T> por entidade de domínio,
                        // em MyFinanceAPI.Data/EntitiesConfiguration. Nomes de PK/FK/índice
                        // reproduzem os do banco de produção (drift-report.md, 03/10/2026).
                        builder.ApplyConfigurationsFromAssembly(typeof(ContextDB).Assembly);
                }

        }
}
