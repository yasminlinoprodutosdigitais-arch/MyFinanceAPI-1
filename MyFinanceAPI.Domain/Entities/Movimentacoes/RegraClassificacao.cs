using System.Globalization;
using System.Text;

namespace MyFinanceAPI.Domain.Entities;

/// <summary>
/// Regra de classificação (card #76, fase 1), lida como frase: "Se <see cref="Campo"/> <see cref="Operador"/>
/// <see cref="Texto"/> em <see cref="BancoId"/> (ou todas as contas), então categoria, tipo e pessoa". Mais de uma
/// regra bate: vale a de cima (<see cref="Ordem"/> menor).
/// </summary>
public class RegraClassificacao : BaseEntity
{
    /// <summary>Gravado normalizado (<see cref="Normalizar"/>).</summary>
    public string Texto { get; set; } = null!;
    /// <summary>Onde procurar o texto: <see cref="CamposRegra"/>.</summary>
    public string Campo { get; set; } = CamposRegra.DescricaoOuPessoa;
    /// <summary>Como comparar: <see cref="OperadoresRegra"/>.</summary>
    public string Operador { get; set; } = OperadoresRegra.Contem;
    /// <summary>Só lançamentos desta conta; null = todas as contas.</summary>
    public int? BancoId { get; set; }
    public Banco? Banco { get; set; }
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public int? TipoMovimentacaoId { get; set; }
    public TipoMovimentacao? TipoMovimentacao { get; set; }
    public int? PessoaMovimentacaoId { get; set; }
    public PessoaMovimentacao? PessoaMovimentacao { get; set; }
    /// <summary>Desligada não classifica, mas continua cadastrada.</summary>
    public bool Ativa { get; set; } = true;
    /// <summary>Prioridade: 0 é a de cima. Regra nova entra no topo.</summary>
    public int Ordem { get; set; }
    /// <summary>Quantos lançamentos a regra já classificou (importação, manual e "aplicar").</summary>
    public int VezesAplicada { get; set; }
    public DateTime? UltimaAplicacao { get; set; }

    /// <summary>Forma única de comparar: sem acento, maiúsculas e espaços simples. Vazio vira null.</summary>
    public static string? Normalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return null;

        var decomposto = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposto.Length);
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var semAcento = sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
        return string.Join(' ', semAcento.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}

/// <summary>Onde a regra procura o texto.</summary>
public static class CamposRegra
{
    public const string DescricaoOuPessoa = "descricao_ou_pessoa";
    public const string Descricao = "descricao";
    public const string Pessoa = "pessoa";
    public static readonly string[] Todos = [DescricaoOuPessoa, Descricao, Pessoa];
}

/// <summary>Como a regra compara o texto.</summary>
public static class OperadoresRegra
{
    public const string Contem = "contem";
    public const string ComecaCom = "comeca_com";
    public const string Igual = "igual";
    public static readonly string[] Todos = [Contem, ComecaCom, Igual];
}
