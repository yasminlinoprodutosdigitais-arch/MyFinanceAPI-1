using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Data.EntitiesConfiguration;

public class ListaConfiguration : IEntityTypeConfiguration<Lista>
{
    public void Configure(EntityTypeBuilder<Lista> builder)
    {
        builder.ToTable(t =>
        {
            // Limite de gasto só faz sentido com o campo Valor ligado (card #67).
            t.HasCheckConstraint("listas_limite_com_valor_check", "NOT usa_limite OR usa_valor");
            t.HasCheckConstraint("listas_limite_check", "limite >= 0");
            t.HasCheckConstraint("listas_modo_valor_check", CheckSql.In("modo_valor", ModoValorLista.Todos));
            t.HasCheckConstraint("listas_modelo_check", CheckSql.In("modelo", ModeloLista.Todos));
            t.HasCheckConstraint("listas_cor_check", CheckSql.In("cor", PaletaCores.Todas));
        });
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseSerialColumn();

        builder.Property(e => e.Nome).HasMaxLength(120);
        builder.Property(e => e.Icone).HasMaxLength(40);
        builder.Property(e => e.Cor).HasMaxLength(20);

        builder.Property(e => e.UsaQuantidade).HasDefaultValue(false);
        builder.Property(e => e.UsaValor).HasDefaultValue(false);
        builder.Property(e => e.ModoValor).HasMaxLength(10).HasDefaultValue(ModoValorLista.Total);
        builder.Property(e => e.UsaLimite).HasDefaultValue(false);
        builder.Property(e => e.Limite).HasPrecision(10, 2);
        builder.Property(e => e.UsaData).HasDefaultValue(false);
        builder.Property(e => e.UsaObservacao).HasDefaultValue(false);
        builder.Property(e => e.Modelo).HasMaxLength(20).HasDefaultValue(ModeloLista.Branco);

        // Série nova a cada lista criada; duplicar e ligar à série gravam a da outra lista (card #67, parte 2).
        builder.Property(e => e.SerieId).HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(e => new { e.UsuarioId, e.SerieId });

        builder.Property<DateTime>("DataAlteracao").HasDefaultValueSql("now()");

        builder.HasOne<Lista>()
            .WithMany()
            .HasForeignKey(e => e.ListaOrigemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
