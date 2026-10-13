using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MyFinanceAPI.Data.Context
{
    /// <summary>
    /// Auditoria mínima (DB-P1-12), para qualquer entidade que tenha as colunas — shadow ou propriedade:
    /// <list type="bullet">
    /// <item><c>DataAlteracao</c>: agora, na criação e em toda edição (antes só Banco e Category, à mão).</item>
    /// <item><c>DataCriacao</c> (Transactions, ExtratoBancarioItem): agora, só na criação; nunca é regravada,
    /// nem quando o update chega com a entidade desanexada (shadow com default).</item>
    /// </list>
    /// Updates fora do change tracker (ExecuteUpdate, SQL cru) não passam por aqui — hoje não existem.
    /// </summary>
    public sealed class DataAlteracaoInterceptor(TimeProvider relogio) : SaveChangesInterceptor
    {
        private const string DataAlteracao = nameof(DataAlteracao);
        private const string DataCriacao = nameof(DataCriacao);

        public DataAlteracaoInterceptor() : this(TimeProvider.System) { }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Carimbar(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Carimbar(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void Carimbar(DbContext? contexto)
        {
            if (contexto is null)
                return;

            // timestamptz no Npgsql exige DateTime UTC.
            var agora = relogio.GetUtcNow().UtcDateTime;

            foreach (var entrada in contexto.ChangeTracker.Entries())
            {
                switch (entrada.State)
                {
                    case EntityState.Added:
                        Definir(entrada, DataAlteracao, agora);
                        Definir(entrada, DataCriacao, agora);
                        break;

                    case EntityState.Modified:
                        Definir(entrada, DataAlteracao, agora);
                        if (entrada.Metadata.FindProperty(DataCriacao) is not null)
                            entrada.Property(DataCriacao).IsModified = false;
                        break;
                }
            }
        }

        private static void Definir(EntityEntry entrada, string coluna, DateTime valor)
        {
            if (entrada.Metadata.FindProperty(coluna) is not null)
                entrada.Property(coluna).CurrentValue = valor;
        }
    }
}
