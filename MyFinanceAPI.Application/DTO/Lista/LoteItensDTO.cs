using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Application.DTO;

/// <summary>Corpo do PATCH /listas/{id}/itens/feito: marca (ou desmarca) estes itens de uma vez.</summary>
public class MarcarItensDTO
{
    [Obrigatorio] public List<int> Ids { get; set; } = [];
    public bool Feito { get; set; } = true;
}

/// <summary>Resposta das ações em lote: quantos itens mudaram.</summary>
public class ResultadoLoteDTO
{
    public int Afetados { get; set; }
}

/// <summary>
/// Corpo do POST /listas/{id}/duplicar. A cópia fica sempre na série da original. Sem nome: "&lt;nome&gt; (cópia)";
/// os padrões do diálogo (nome com o mês seguinte, arquivar a original) ficam no front.
/// </summary>
public class DuplicarListaDTO
{
    [TextoAte(120)] public string? Nome { get; set; }
    /// <summary>Copia os itens, todos desmarcados.</summary>
    public bool CopiarItens { get; set; } = true;
    /// <summary>Sem isso, os itens copiados vão sem valor.</summary>
    public bool ManterValores { get; set; } = true;
    public bool ArquivarOriginal { get; set; }
}

/// <summary>Corpo do PUT /listas/{id}/serie: entra na série desta lista; null = sai para uma série própria.</summary>
public class SerieListaDTO
{
    public int? ComListaId { get; set; }
}
