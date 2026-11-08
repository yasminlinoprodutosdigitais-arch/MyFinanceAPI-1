using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

/// <summary>Card #76, fase 2: limpar as pessoas que a importação antiga criou para estabelecimentos.</summary>
public interface ILimpezaPessoasService
{
    /// <summary>As candidatas (fora dos acertos, sem padrão, sem regra, não o "Eu"), com quantos lançamentos cada uma tem.</summary>
    Task<IReadOnlyList<PessoaEstabelecimentoDTO>> CandidatasAsync(int userId);
    /// <summary>Desliga os lançamentos (o nome continua neles) e apaga as pessoas pedidas que ainda são candidatas.</summary>
    Task<int> RemoverAsync(int userId, IReadOnlyCollection<int> ids);
}
