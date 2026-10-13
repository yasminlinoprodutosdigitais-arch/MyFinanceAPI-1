using System.Text.Json;
using System.Text.Json.Serialization;
using MyFinanceAPI.Application.Json;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// DB-P1-02: a data da transação é data de calendário ("YYYY-MM-DD"). O conversor também aceita
/// instante ISO, que o front antigo manda, e o lê na data de São Paulo.
/// </summary>
public class DataCalendarioJsonConverterTests
{
    private sealed class Payload
    {
        [JsonConverter(typeof(DataCalendarioJsonConverter))]
        public DateOnly Date { get; set; }
    }

    private static DateOnly Ler(string valorJson) =>
        JsonSerializer.Deserialize<Payload>($"{{\"Date\":{valorJson}}}")!.Date;

    [Fact]
    public void Le_data_de_calendario()
    {
        Assert.Equal(new DateOnly(2026, 10, 5), Ler("\"2026-10-05\""));
    }

    [Theory]
    [InlineData("\"2026-10-05T03:00:00.000Z\"")]   // meia-noite em BRT (o que o front manda hoje)
    [InlineData("\"2026-10-05T15:30:00Z\"")]
    [InlineData("\"2026-10-05T23:59:00-03:00\"")]
    [InlineData("\"2026-10-06T02:59:00Z\"")]       // ainda dia 05 em São Paulo
    public void Instante_ISO_vira_a_data_em_Sao_Paulo(string valorJson)
    {
        Assert.Equal(new DateOnly(2026, 10, 5), Ler(valorJson));
    }

    [Fact]
    public void Meia_noite_UTC_e_o_dia_anterior_em_Sao_Paulo()
    {
        // Instante explícito é respeitado: 00:00Z do dia 06 ainda é dia 05 em BRT.
        Assert.Equal(new DateOnly(2026, 10, 5), Ler("\"2026-10-06T00:00:00Z\""));
    }

    [Fact]
    public void Escreve_so_a_data()
    {
        var json = JsonSerializer.Serialize(new Payload { Date = new DateOnly(2026, 1, 31) });

        Assert.Equal("{\"Date\":\"2026-01-31\"}", json);
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("\"amanhã\"")]
    [InlineData("123")]
    public void Valor_invalido_lanca_JsonException(string valorJson)
    {
        Assert.Throws<JsonException>(() => Ler(valorJson));
    }
}
