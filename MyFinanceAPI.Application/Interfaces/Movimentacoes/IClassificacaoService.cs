using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Card #76, fase 1: regras nos lançamentos que já existem e sugestões pelo histórico.</summary>
public interface IClassificacaoService
{
    /// <summary>O que uma regra (ainda não gravada) pegaria: total, sem categoria, conflitos e exemplos.</summary>
    Task<PreviaRegraResultadoDTO> PreviaAsync(int userId, PreviaRegraDTO previa);
    /// <summary>Aplica a regra nos lançamentos sem categoria que batem com ela; devolve quantos e quais mudaram.</summary>
    Task<AplicacaoRegraDTO> AplicarAsync(int userId, int regraId);
    /// <summary>Sugestão de categoria para os lançamentos sem categoria pedidos, pelo histórico.</summary>
    Task<IReadOnlyList<SugestaoCategoriaDTO>> SugestoesAsync(int userId, IEnumerable<int> itemIds);
    /// <summary>Textos que se repetem em lançamentos sem categoria e ainda não têm regra.</summary>
    Task<IReadOnlyList<SugestaoRegraDTO>> SugestoesDeRegraAsync(int userId);
}
