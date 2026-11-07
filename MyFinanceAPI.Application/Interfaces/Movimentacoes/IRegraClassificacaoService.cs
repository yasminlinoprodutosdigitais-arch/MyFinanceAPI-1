using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Regras de classificação (card #76, fase 1).</summary>
public interface IRegraClassificacaoService : IServicoCadastro<RegraClassificacaoDTO>
{
    /// <summary>Nova ordem de prioridade: o primeiro id fica no topo.</summary>
    Task ReordenarAsync(int userId, IReadOnlyList<int> ids);
}
