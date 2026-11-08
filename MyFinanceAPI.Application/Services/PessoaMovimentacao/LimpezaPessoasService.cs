using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #76, fase 2: a importação deixou de cadastrar estabelecimento como pessoa; esta limpeza apaga as que ela já
/// criou. Só apaga quem ainda é candidata no momento de confirmar, e os lançamentos ficam com o nome.
/// </summary>
public class LimpezaPessoasService(
    IPessoaMovimentacaoRepository pessoas, IRegraClassificacaoRepository regras, IExtratoBancarioItemRepository itens, IUnitOfWork unitOfWork)
    : ILimpezaPessoasService
{
    public async Task<IReadOnlyList<PessoaEstabelecimentoDTO>> CandidatasAsync(int userId) =>
        Candidatas(await pessoas.ListarAsync(userId), await PessoasEmRegrasAsync(userId), await itens.ContarPorPessoaAsync(userId));

    public Task<int> RemoverAsync(int userId, IReadOnlyCollection<int> ids) =>
        unitOfWork.ExecuteAsync(async () =>
        {
            var pedidas = ids.ToHashSet();
            if (pedidas.Count == 0)
                return 0;

            // Recalcula na hora: alguém pode ter ligado "Entra nos acertos" ou dado um padrão depois de a lista abrir.
            var candidatas = Candidatas(await pessoas.ListarAsync(userId), await PessoasEmRegrasAsync(userId), new Dictionary<int, int>())
                .Select(c => c.Id)
                .Where(pedidas.Contains)
                .ToList();
            if (candidatas.Count == 0)
                return 0;

            await itens.DesligarPessoasAsync(userId, candidatas);
            return await pessoas.RemoverVariasAsync(userId, candidatas);
        });

    /// <summary>
    /// Fora dos acertos, sem categoria e sem tipo padrão, sem regra que a use e que não seja o "Eu". Em ordem de nome.
    /// </summary>
    public static List<PessoaEstabelecimentoDTO> Candidatas(
        IEnumerable<PessoaMovimentacao> todas, IReadOnlySet<int> emRegras, IReadOnlyDictionary<int, int> lancamentos) =>
        todas
            .Where(p => !p.EhTitular && !p.EntraNosAcertos && p.CategoriaId == null && p.TipoMovimentacaoId == null && !emRegras.Contains(p.Id))
            .OrderBy(p => p.NomePessoa, StringComparer.CurrentCulture)
            .Select(p => new PessoaEstabelecimentoDTO { Id = p.Id, NomePessoa = p.NomePessoa, Lancamentos = lancamentos.GetValueOrDefault(p.Id) })
            .ToList();

    private async Task<IReadOnlySet<int>> PessoasEmRegrasAsync(int userId) =>
        (await regras.ListarAsync(userId)).Where(r => r.PessoaMovimentacaoId != null).Select(r => r.PessoaMovimentacaoId!.Value)
            .Concat(await itens.PessoasEmPartesAsync(userId)).ToHashSet();
}
