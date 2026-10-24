using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using MyFinanceAPI.Data.Context;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// Card [DB] Apagar a coluna morta contas.data_operacao (24/10/2026): os dias de vencimento vivem só em
/// contas_vencimento. O DataOperacao do ContaDTO (lista de dias de entrada) é outra coisa e fica.
/// </summary>
public class ContasSemDataOperacaoTests
{
    [Fact]
    public void Contas_nao_tem_mais_a_coluna_data_operacao()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        var contas = contexto.GetService<IDesignTimeModel>().Model.GetEntityTypes().Single(e => e.GetTableName() == "contas");

        Assert.DoesNotContain(contas.GetProperties(), p => p.GetColumnName() == "data_operacao");
    }
}
