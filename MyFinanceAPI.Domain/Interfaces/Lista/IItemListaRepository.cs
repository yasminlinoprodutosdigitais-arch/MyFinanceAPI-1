using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IItemListaRepository : IRepositorioDoUsuario<ItemLista>
{
    /// <summary>Itens de uma lista do usuário, pendentes antes dos feitos e depois pela ordem/descrição.</summary>
    Task<List<ItemLista>> ListarDaListaAsync(int listaId, int userId);

    /// <summary>Itens de várias listas do usuário (as de uma série, na comparação).</summary>
    Task<List<ItemLista>> ListarDasListasAsync(IReadOnlyCollection<int> listaIds, int userId);

    Task CriarVariosAsync(IEnumerable<ItemLista> itens);

    // Ações em lote (card #67): cada uma é um UPDATE/DELETE só, então vale inteira ou não vale.
    // Devolvem a quantidade de itens afetados; itens de outra lista ou de outro usuário ficam de fora.
    // <agora> vira feito_em (ao marcar) e data_alteracao: o comando em lote não passa pelo interceptor.
    Task<int> MarcarFeitoAsync(int listaId, IReadOnlyCollection<int> ids, bool feito, DateTime agora, int userId);
    Task<int> DesmarcarTodosAsync(int listaId, DateTime agora, int userId);
    /// <summary>Só itens que não repetem: o feito de um item que repete é por ocorrência.</summary>
    Task<int> ExcluirConcluidosAsync(int listaId, int userId);

    // Ocorrências da agenda (card #67, parte 3).
    /// <summary>Itens que repetem nessas listas (para a próxima data do resumo).</summary>
    Task<List<ItemLista>> ListarQueRepetemAsync(IReadOnlyCollection<int> listaIds, int userId);
    Task<List<OcorrenciaItemLista>> ListarOcorrenciasAsync(IReadOnlyCollection<int> itemIds, int userId);
    /// <summary>Carrega (ou cria) a ocorrência do item na data original, aplica <paramref name="alterar"/> e grava.</summary>
    Task GravarOcorrenciaAsync(int itemId, DateOnly dataOriginal, int userId, Action<OcorrenciaItemLista> alterar);
    /// <summary>Apaga as exceções do item de <paramref name="aPartirDe"/> em diante.</summary>
    Task<int> ApagarOcorrenciasAsync(int itemId, DateOnly aPartirDe, int userId);
    /// <summary>Desmarca as ocorrências feitas dos itens da lista.</summary>
    Task<int> DesmarcarOcorrenciasAsync(int listaId, int userId);
}
