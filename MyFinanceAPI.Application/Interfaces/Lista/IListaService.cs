using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface IListaService : IServicoCadastro<ListaDTO>
{
    /// <summary>Listas com o resumo; <paramref name="arquivadas"/> = pasta Arquivadas.</summary>
    Task<IEnumerable<ListaDTO>> ListarAsync(int userId, bool arquivadas);

    /// <summary>Lista alheia ou inexistente: NaoEncontradoException (404).</summary>
    Task ArquivarAsync(int id, int userId);
    Task DesarquivarAsync(int id, int userId);

    /// <summary>
    /// Copia a configuração e, se pedido, os itens (desmarcados, com ou sem valor), na série da original e
    /// com as origens gravadas. Pode arquivar a original na mesma operação.
    /// </summary>
    Task<ListaDTO> DuplicarAsync(int id, DuplicarListaDTO opcoes, int userId);

    /// <summary>Listas da série da lista <paramref name="id"/>, arquivadas inclusive, com o resumo.</summary>
    Task<IEnumerable<ListaDTO>> ListarDaSerieAsync(int id, int userId);

    /// <summary>
    /// Compara a lista <paramref name="id"/> (atual) com <paramref name="com"/> (anterior; sem ela, a criada logo antes
    /// na série). As duas precisam ser da mesma série. <paramref name="soFeitos"/> = base "só itens ok".
    /// </summary>
    Task<ComparacaoListasDTO> CompararAsync(int id, int? com, bool soFeitos, int userId);

    /// <summary>Entra na série de <paramref name="comListaId"/>; null = sai para uma série própria.</summary>
    Task DefinirSerieAsync(int id, int? comListaId, int userId);
}
