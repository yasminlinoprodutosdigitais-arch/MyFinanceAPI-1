using System.Text.RegularExpressions;

namespace MyFinanceAPI.Application.Utils;

/// <summary>Ícones guardados como classe do PrimeIcons ("pi pi-nome"), em categorias (#23) e listas (#67).</summary>
public static class IconePrimeIcons
{
    private static readonly Regex Formato = new("^pi pi-[a-z0-9-]+$", RegexOptions.Compiled);

    /// <summary>Vazio vale (sem ícone).</summary>
    public static bool Valido(string? icone) => string.IsNullOrWhiteSpace(icone) || Formato.IsMatch(icone.Trim());
}
