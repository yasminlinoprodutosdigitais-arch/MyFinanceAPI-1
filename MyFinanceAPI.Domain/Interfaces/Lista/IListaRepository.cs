using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IListaRepository : IRepositorioDoUsuario<Lista>
{
    /// <summary>Listas do usuário com o resumo; <paramref name="arquivadas"/> escolhe a pasta.</summary>
    Task<List<ListaComResumo>> ListarComResumoAsync(int userId, bool arquivadas, DateOnly hoje);

    /// <summary>Null se não existe ou é de outro usuário.</summary>
    Task<ListaComResumo?> ObterComResumoAsync(int id, int userId, DateOnly hoje);

    /// <summary>Listas da série, arquivadas inclusive, em ordem de criação.</summary>
    Task<List<ListaComResumo>> ListarDaSerieAsync(Guid serieId, int userId, DateOnly hoje);

    /// <summary>Muda a série da lista. Lista alheia ou inexistente: NaoEncontradoException.</summary>
    Task DefinirSerieAsync(int id, int userId, Guid serieId);

    /// <summary>Preenche (arquivar) ou limpa (desarquivar) arquivada_em. Lista alheia ou inexistente: NaoEncontradoException.</summary>
    Task DefinirArquivadaAsync(int id, int userId, DateTime? arquivadaEm);
}
