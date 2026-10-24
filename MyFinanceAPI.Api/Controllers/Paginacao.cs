using MyFinanceAPI.Application.Utils;

namespace MyFinanceAPI.Api.Controllers;

/// <summary>
/// Paginação opcional dos GET de lista (BE-P4-16): <c>?pagina=2&amp;tamanhoPagina=50</c>. Sem nenhum dos dois, a
/// lista vem inteira (o front pagina no navegador). Com algum, vem a fatia e o total no cabeçalho X-Total-Count.
/// </summary>
public sealed class Paginacao
{
    public const int TamanhoPadrao = 20;
    public const int TamanhoMaximo = 100;

    [Faixa(1, int.MaxValue)] public int? Pagina { get; set; }
    [Faixa(1, TamanhoMaximo)] public int? TamanhoPagina { get; set; }

    public bool Ativa => Pagina is not null || TamanhoPagina is not null;
}
