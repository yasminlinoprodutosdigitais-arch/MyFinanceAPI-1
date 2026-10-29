using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.Context
{
        public class ContextDB : IdentityDbContext<Usuario, IdentityRole<int>, int>
        {
                public DbSet<Usuario> Usuarios { get; set; }
                public DbSet<Conta> Contas { get; set; }
                public DbSet<ContaVencimento> ContasVencimento { get; set; }
                public DbSet<Categoria> Categorias { get; set; }
                public DbSet<Transacao> Transacoes { get; set; }
                public DbSet<Banco> Bancos { get; set; }
                public DbSet<Lista> Listas { get; set; }
                public DbSet<ItemLista> ItensLista { get; set; }
                public DbSet<OcorrenciaItemLista> ItensListaOcorrencias { get; set; }
                public DbSet<MovimentacaoDiaria> MovimentacoesDiarias { get; set; }
                public DbSet<TipoCartao> TiposCartao { get; set; }
                public DbSet<TipoMovimentacao> TiposMovimentacao { get; set; }
                public DbSet<PessoaMovimentacao> PessoasMovimentacao { get; set; }

                // 👇 DbSets do extrato
                public DbSet<ExtratoBancario> ExtratosBancarios { get; set; }
                public DbSet<ExtratoBancarioItem> ItensExtratoBancario { get; set; }

                public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }

                // Nomes físicos em snake_case (DB-P3): tabela a partir do DbSet, coluna, pk_, fk_ e ix_.
                // Aqui, e não na Ioc, para valer igual no runtime, no dotnet ef e nos testes.
                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
                        optionsBuilder.UseSnakeCaseNamingConvention()
                                .ReplaceService<IHistoryRepository, HistoricoDeMigrations>();

                protected override void OnModelCreating(ModelBuilder builder)
                {
                        base.OnModelCreating(builder);

                        // O Identity fixa "AspNet*" com ToTable, e nome explícito a convenção não reescreve.
                        builder.Entity<Usuario>().ToTable("asp_net_users");
                        builder.Entity<IdentityRole<int>>().ToTable("asp_net_roles");
                        builder.Entity<IdentityUserClaim<int>>().ToTable("asp_net_user_claims");
                        builder.Entity<IdentityUserLogin<int>>().ToTable("asp_net_user_logins");
                        builder.Entity<IdentityUserRole<int>>().ToTable("asp_net_user_roles");
                        builder.Entity<IdentityUserToken<int>>().ToTable("asp_net_user_tokens");
                        builder.Entity<IdentityRoleClaim<int>>().ToTable("asp_net_role_claims");
                        // Pelo mesmo motivo, os índices que o Identity nomeia ("UserNameIndex"…) só passam a snake_case.
                        builder.Entity<Usuario>().HasIndex(u => u.NormalizedUserName).HasDatabaseName("user_name_index");
                        builder.Entity<Usuario>().HasIndex(u => u.NormalizedEmail).HasDatabaseName("email_index");
                        builder.Entity<IdentityRole<int>>().HasIndex(r => r.NormalizedName).HasDatabaseName("role_name_index");

                        // Uma classe IEntityTypeConfiguration<T> por entidade de domínio,
                        // em MyFinanceAPI.Data/EntitiesConfiguration.
                        builder.ApplyConfigurationsFromAssembly(typeof(ContextDB).Assembly);

                        NomearForeignKeys(builder);
                }

                // A convenção nomeia a FK quando ela nasce, com o nome que a tabela principal tinha então
                // ("fk_transacoes_usuario_…" em vez de asp_net_users), e o EF corta acima de 63 caracteres
                // no meio da palavra. Aqui, com todas as tabelas já nomeadas: "fk_{tabela}_{principal}_{colunas}",
                // ou "fk_{tabela}_{colunas}" quando o primeiro não cabe.
                private static void NomearForeignKeys(ModelBuilder builder)
                {
                        foreach (var entidade in builder.Model.GetEntityTypes())
                        {
                                var tabela = entidade.GetTableName();
                                if (tabela is null)
                                        continue;

                                var objeto = StoreObjectIdentifier.Table(tabela, entidade.GetSchema());
                                foreach (var fk in entidade.GetForeignKeys())
                                {
                                        var colunas = string.Join("_", fk.Properties.Select(p => p.GetColumnName(objeto)));
                                        var nome = $"fk_{tabela}_{fk.PrincipalEntityType.GetTableName()}_{colunas}";
                                        fk.SetConstraintName(nome.Length <= 63 ? nome : $"fk_{tabela}_{colunas}");
                                }
                        }
                }

        }
}
