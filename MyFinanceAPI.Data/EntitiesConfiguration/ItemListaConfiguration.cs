using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ItemListaConfiguration : IEntityTypeConfiguration<ItemLista>
{
    public void Configure(EntityTypeBuilder<ItemLista> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("itens_lista_quantidade_check", "quantidade >= 1");
            t.HasCheckConstraint("itens_lista_valor_check", "valor >= 0");
            // Repetição (card #67, parte 3): a data é a primeira ocorrência; a semanal precisa de dias de 1 (seg) a 7 (dom).
            t.HasCheckConstraint("itens_lista_repeticao_check", CheckSql.In("repeticao", RepeticaoItem.Todos));
            t.HasCheckConstraint("itens_lista_repeticao_com_data_check", "repeticao = 'NENHUMA' OR data IS NOT NULL");
            t.HasCheckConstraint("itens_lista_semanal_com_dias_check", "repeticao <> 'SEMANAL' OR cardinality(dias_semana) > 0");
            t.HasCheckConstraint("itens_lista_dias_semana_check", "dias_semana <@ ARRAY[1, 2, 3, 4, 5, 6, 7]");
            t.HasCheckConstraint("itens_lista_repetir_ate_check", "repetir_ate >= data");
        });
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Descricao).HasMaxLength(200);
        builder.Property(e => e.Observacao).HasMaxLength(500);
        builder.Property(e => e.Feito).HasDefaultValue(false);
        builder.Property(e => e.Valor).HasPrecision(10, 2);
        builder.Property(e => e.Repeticao).HasMaxLength(10).HasDefaultValue(RepeticaoItem.Nenhuma);

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ItemLista>()
            .WithMany()
            .HasForeignKey(e => e.ItemOrigemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Lista)
            .WithMany()
            .HasForeignKey(e => e.ListaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
