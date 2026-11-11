using System.Text.RegularExpressions;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Services;

/// <summary>Como um nome do extrato está classificado (card #79).</summary>
public static class CoberturaNome
{
    /// <summary>Tem a regra "nome do extrato é igual a" este nome.</summary>
    public const string Igual = "igual";
    /// <summary>Outra regra pega este nome (contém, começa com, por conta...).</summary>
    public const string Outra = "outra";
    /// <summary>Nenhuma regra pega, mas uma regra "igual a" tem nome parecido: classifica por semelhança.</summary>
    public const string Semelhanca = "semelhanca";
    /// <summary>Para classificar.</summary>
    public const string Nenhuma = "nenhuma";
}

/// <summary>
/// Card #79: o nome que vem no extrato ("CIRCUITO"), que não é a pessoa que usou o cartão. É por ele que as regras
/// "nome do extrato é igual a" classificam, e é ele que a tela "Nomes do extrato" lista.
/// </summary>
public static partial class NomeExtrato
{
    /// <summary>Menos letras que isso, "começa com" não conta como parecido (UBER × UBER EATS).</summary>
    private const int MinimoParaPrefixo = 8;

    private static readonly HashSet<string> Sufixos = ["LTDA", "LT", "L", "ME", "EPP", "EIRELI", "SA", "S/A", "S.A"];

    /// <summary>O nome do extrato de um lançamento: o texto sugerido das regras, normalizado. Sem nome, vazio.</summary>
    public static string Chave(string? descricao, string? nomePessoa) =>
        RegraClassificacao.Normalizar(ClassificacaoService.TextoSugerido(descricao, nomePessoa)) ?? "";

    /// <summary>
    /// O nome só com letras, para comparar: sem CPF/CNPJ no começo, sem números, sem LTDA/LT/L/ME/EPP/EIRELI/SA no fim
    /// e sem espaços. "12.345.678/0001-90 FARMACIA LED FARMA LTDA" vira "FARMACIALEDFARMA".
    /// </summary>
    public static string Compacta(string? texto)
    {
        var normal = RegraClassificacao.Normalizar(texto);
        if (normal == null)
            return "";

        var palavras = Digitos().Replace(Documento().Replace(normal, ""), " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        while (palavras.Count > 1 && Sufixos.Contains(palavras[^1]))
            palavras.RemoveAt(palavras.Count - 1);

        return NaoLetras().Replace(string.Concat(palavras), "");
    }

    /// <summary>
    /// O mesmo lugar escrito de outro jeito: iguais depois de <see cref="Compacta"/>, ou um começa com o outro e o mais
    /// curto tem pelo menos 8 letras (TELEFONICA × TELEFONICA BRAS).
    /// </summary>
    public static bool Parecidos(string? a, string? b)
    {
        var x = Compacta(a);
        var y = Compacta(b);
        if (x.Length == 0 || y.Length == 0)
            return false;
        if (x == y)
            return true;

        var (curto, longo) = x.Length <= y.Length ? (x, y) : (y, x);
        return curto.Length >= MinimoParaPrefixo && longo.StartsWith(curto, StringComparison.Ordinal);
    }

    [GeneratedRegex(@"^[\d\.\-/\s]+")]
    private static partial Regex Documento();

    [GeneratedRegex(@"\d+")]
    private static partial Regex Digitos();

    [GeneratedRegex(@"[^A-Z]")]
    private static partial Regex NaoLetras();
}
