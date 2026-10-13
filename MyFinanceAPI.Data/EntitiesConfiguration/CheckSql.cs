namespace MyFinanceAPI.Data.EntitiesConfiguration;

/// <summary>
/// Monta o SQL dos CHECK constraints a partir das constantes do Domain (DB-P1-03/04),
/// para o domínio aceito pelo banco não divergir do usado no código.
/// </summary>
internal static class CheckSql
{
    public static string In(string coluna, IEnumerable<string> valores) =>
        $"\"{coluna}\" IN ({string.Join(", ", valores.Select(v => $"'{v.Replace("'", "''")}'"))})";

    public static string In(string coluna, IEnumerable<int> valores) =>
        $"\"{coluna}\" IN ({string.Join(", ", valores)})";

    /// <summary>Parcela atual dentro do total. Nulos passam (lançamento não parcelado).</summary>
    public static string Parcelas(int parcelaMinima) =>
        $"\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN {parcelaMinima} AND \"QuantidadeParcelas\"";
}
