using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyFinanceAPI.Application.Json;

/// <summary>
/// Data de calendário (vencimento de transação) no JSON — DB-P1-02.
/// Escreve sempre "YYYY-MM-DD". Lê "YYYY-MM-DD" e, por tolerância a front antigo/em cache,
/// também um instante ISO ("2026-10-05T03:00:00.000Z"), convertido para a data em São Paulo:
/// é o dia que o usuário escolheu na tela. Nada aqui depende do fuso do servidor.
/// </summary>
public sealed class DataCalendarioJsonConverter : JsonConverter<DateOnly>
{
    private const string Formato = "yyyy-MM-dd";

    private static readonly TimeZoneInfo SaoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Data esperada como texto \"YYYY-MM-DD\".");

        var texto = reader.GetString();

        if (DateOnly.TryParseExact(texto, Formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            return data;

        if (DateTimeOffset.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var instante))
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(instante, SaoPaulo).DateTime);

        throw new JsonException($"Data inválida: \"{texto}\". Esperado \"YYYY-MM-DD\".");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(Formato, CultureInfo.InvariantCulture));
}
