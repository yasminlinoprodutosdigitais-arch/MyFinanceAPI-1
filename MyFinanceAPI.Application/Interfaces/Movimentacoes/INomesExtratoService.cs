using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Card #79: os nomes que vieram nos extratos e as regras que os classificam.</summary>
public interface INomesExtratoService
{
    Task<IReadOnlyList<NomeExtratoDTO>> ListarAsync(int userId);
    Task<ClassificacaoNomesResultadoDTO> ClassificarAsync(int userId, ClassificarNomesDTO pedido);
    Task<IReadOnlyList<GrupoParecidosDTO>> GruposAsync(int userId);
    Task<ClassificacaoNomesResultadoDTO> JuntarAsync(int userId, JuntarNomesDTO pedido);
}
