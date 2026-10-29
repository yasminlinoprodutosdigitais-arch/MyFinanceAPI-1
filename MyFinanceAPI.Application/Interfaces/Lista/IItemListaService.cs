using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>
/// Itens sempre pela lista (card #67): toda operação confere que a lista é do usuário e que o item é dela.
/// Lista ou item alheio ou inexistente: NaoEncontradoException (404).
/// </summary>
public interface IItemListaService : IServicoCadastro<ItemListaDTO>
{
    Task<List<ItemListaDTO>> ListarDaListaAsync(int listaId, int userId);
    Task<ItemListaDTO> AdicionarNaListaAsync(int listaId, ItemListaDTO dto, int userId);
    Task AtualizarNaListaAsync(int listaId, int itemId, ItemListaDTO dto, int userId);
    Task RemoverDaListaAsync(int listaId, int itemId, int userId);

    /// <summary>Marca ou desmarca os itens dos ids numa operação só; devolve quantos mudaram.</summary>
    Task<int> MarcarFeitoAsync(int listaId, IReadOnlyCollection<int> ids, bool feito, int userId);
    Task<int> DesmarcarTodosAsync(int listaId, int userId);
    Task<int> ExcluirConcluidosAsync(int listaId, int userId);

    // Agenda (card #67, parte 3). Regra inválida ou data fora da regra: RegraNegocioException (409).

    /// <summary>Ocorrências do período (no máximo 366 dias), repetições expandidas e exceções aplicadas.</summary>
    Task<List<OcorrenciaDTO>> ListarPeriodoAsync(int listaId, DateOnly de, DateOnly ate, int userId);

    /// <summary>"Só esta": feito, outro dia ou hora, ou pulada.</summary>
    Task AtualizarOcorrenciaAsync(int listaId, int itemId, DateOnly dataOriginal, AlterarOcorrenciaDTO dto, int userId);

    /// <summary>"Esta e as próximas": a regra antiga termina na véspera e o item novo vale dali em diante.</summary>
    Task<ItemListaDTO> DividirAsync(int listaId, int itemId, DateOnly aPartirDe, ItemListaDTO dto, int userId);

    /// <summary>Encerra a repetição na véspera; desde a primeira ocorrência, exclui o item.</summary>
    Task EncerrarAsync(int listaId, int itemId, DateOnly aPartirDe, int userId);
}
