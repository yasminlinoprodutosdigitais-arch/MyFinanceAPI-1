using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

/// <summary>Regras de classificação do usuário (card #76, fase 1), sempre na ordem de prioridade (a de cima vale).</summary>
public interface IRegraClassificacaoRepository : IRepositorioDoUsuario<RegraClassificacao>
{
    /// <summary>True se o usuário já tem outra regra com a mesma frase (campo, operador, texto normalizado e conta).</summary>
    Task<bool> ExisteFraseAsync(int userId, string campo, string operador, string texto, int? bancoId, int ignorarId);
    /// <summary>As regras ligadas do usuário, na ordem (o que a importação e o lançamento manual usam).</summary>
    Task<List<RegraClassificacao>> ListarAtivasAsync(int userId);
    /// <summary>Grava a ordem nova: o primeiro id fica no topo. Ids de outro usuário são ignorados.</summary>
    Task ReordenarAsync(int userId, IReadOnlyList<int> ids);
    /// <summary>Soma o uso de cada regra (id → quantos lançamentos classificou agora) e marca a última aplicação.</summary>
    Task RegistrarUsoAsync(int userId, IReadOnlyDictionary<int, int> vezesPorRegra);
}
