using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Itens de lista (card #67). A lista vem sempre da rota, nunca do corpo; feito_em é do serviço:
/// grava a hora ao marcar, mantém a original ao editar um item já feito e limpa ao desmarcar.
/// Na agenda (parte 3), o feito de um item que repete é por ocorrência (<see cref="AtualizarOcorrenciaAsync"/>).
/// </summary>
public class ItemListaService(
    IItemListaRepository itemListaRepository,
    IListaRepository listaRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    TimeProvider relogio)
    : ServicoCadastro<ItemLista, ItemListaDTO>(itemListaRepository, mapper), IItemListaService
{
    private const string ItemNaoEncontrado = "Item da lista não encontrado.";
    private const int DiasNoPeriodo = 366;

    private DateTime Agora => relogio.GetUtcNow().UtcDateTime;

    private async Task ConferirListaAsync(int listaId, int userId)
    {
        if (await listaRepository.ObterPorIdAsync(listaId, userId) is null)
            throw new NaoEncontradoException("Lista não encontrada.");
    }

    private async Task<ItemLista> ItemDaListaAsync(int listaId, int itemId, int userId)
    {
        var item = await itemListaRepository.ObterPorIdAsync(itemId, userId);
        return item is not null && item.ListaId == listaId ? item : throw new NaoEncontradoException(ItemNaoEncontrado);
    }

    private static async Task<ItemLista> QueRepeteAsync(Task<ItemLista> buscar)
    {
        var item = await buscar;
        return item.Repeticao != RepeticaoItem.Nenhuma
            ? item
            : throw new RegraNegocioException("Este item não se repete: altere o próprio item.");
    }

    /// <summary>Regra de repetição coerente antes do banco (os CHECKs dão a mesma garantia, com mensagem pior).</summary>
    private static void ValidarRepeticao(ItemLista item)
    {
        if (item.Repeticao != RepeticaoItem.Semanal)
            item.DiasSemana = null;
        if (item.Repeticao == RepeticaoItem.Nenhuma)
        {
            item.RepetirAte = null;
            return;
        }

        if (item.Data is null)
            throw new RegraNegocioException("Informe a data da primeira ocorrência.");
        if (item.Repeticao == RepeticaoItem.Semanal && (item.DiasSemana is null || item.DiasSemana.Length == 0))
            throw new RegraNegocioException("Escolha os dias da semana.");
        if (item.DiasSemana?.Any(d => d is < 1 or > 7) == true)
            throw new RegraNegocioException("Os dias da semana vão de 1 (segunda) a 7 (domingo).");
        if (item.RepetirAte < item.Data)
            throw new RegraNegocioException("Repetir até não pode ser antes da data.");
        item.DiasSemana = item.DiasSemana?.Distinct().Order().ToArray();
    }

    public async Task<List<ItemListaDTO>> ListarDaListaAsync(int listaId, int userId)
    {
        await ConferirListaAsync(listaId, userId);
        return Mapper.Map<List<ItemListaDTO>>(await itemListaRepository.ListarDaListaAsync(listaId, userId));
    }

    private ItemLista NovoItem(int listaId, ItemListaDTO dto, int userId)
    {
        var item = ParaEntidade(dto);
        item.Id = 0;
        item.ListaId = listaId;
        item.FeitoEm = item.Feito ? Agora : null;
        ValidarRepeticao(item);
        DefinirDono(item, userId);
        return item;
    }

    public async Task<ItemListaDTO> AdicionarNaListaAsync(int listaId, ItemListaDTO dto, int userId)
    {
        await ConferirListaAsync(listaId, userId);
        return Mapper.Map<ItemListaDTO>(await itemListaRepository.CriarAsync(NovoItem(listaId, dto, userId)));
    }

    public async Task AtualizarNaListaAsync(int listaId, int itemId, ItemListaDTO dto, int userId)
    {
        var existente = await ItemDaListaAsync(listaId, itemId, userId);

        var item = ParaEntidade(dto);
        item.Id = itemId;
        item.ListaId = listaId;
        item.FeitoEm = !item.Feito ? null : existente.Feito ? existente.FeitoEm : Agora;
        ValidarRepeticao(item);
        await itemListaRepository.AtualizarAsync(item, userId);
    }

    public async Task RemoverDaListaAsync(int listaId, int itemId, int userId)
    {
        await ItemDaListaAsync(listaId, itemId, userId);
        await itemListaRepository.RemoverAsync(itemId, userId);
    }

    public async Task<int> MarcarFeitoAsync(int listaId, IReadOnlyCollection<int> ids, bool feito, int userId)
    {
        if (ids.Count == 0)
            throw new RegraNegocioException("Informe os itens.");
        await ConferirListaAsync(listaId, userId);
        return await itemListaRepository.MarcarFeitoAsync(listaId, ids.Distinct().ToList(), feito, Agora, userId);
    }

    public async Task<int> DesmarcarTodosAsync(int listaId, int userId)
    {
        await ConferirListaAsync(listaId, userId);
        return await unitOfWork.ExecuteAsync(async () =>
            await itemListaRepository.DesmarcarTodosAsync(listaId, Agora, userId)
            + await itemListaRepository.DesmarcarOcorrenciasAsync(listaId, userId));
    }

    public async Task<int> ExcluirConcluidosAsync(int listaId, int userId)
    {
        await ConferirListaAsync(listaId, userId);
        return await itemListaRepository.ExcluirConcluidosAsync(listaId, userId);
    }

    public async Task<List<OcorrenciaDTO>> ListarPeriodoAsync(int listaId, DateOnly de, DateOnly ate, int userId)
    {
        if (ate < de)
            throw new RegraNegocioException("O fim do período não pode ser antes do início.");
        if (ate.DayNumber - de.DayNumber + 1 > DiasNoPeriodo)
            throw new RegraNegocioException($"O período vai até {DiasNoPeriodo} dias.");
        await ConferirListaAsync(listaId, userId);

        var itens = (await itemListaRepository.ListarDaListaAsync(listaId, userId)).Where(i => i.Data is not null).ToList();
        var repetem = itens.Where(i => i.Repeticao != RepeticaoItem.Nenhuma).Select(i => i.Id).ToList();
        var excecoes = repetem.Count == 0 ? [] : await itemListaRepository.ListarOcorrenciasAsync(repetem, userId);

        return Repeticao.NoPeriodo(itens, excecoes, de, ate).Select(o => new OcorrenciaDTO
        {
            ItemId = o.Item.Id,
            DataOriginal = o.DataOriginal,
            Data = o.Data,
            Hora = o.Hora,
            Feito = o.Feito,
            FeitoEm = o.FeitoEm,
            Repete = o.Repete,
            Descricao = o.Item.Descricao,
            Observacao = o.Item.Observacao,
            Quantidade = o.Item.Quantidade,
            Valor = o.Item.Valor,
        }).ToList();
    }

    public async Task AtualizarOcorrenciaAsync(int listaId, int itemId, DateOnly dataOriginal, AlterarOcorrenciaDTO dto, int userId)
    {
        var item = await QueRepeteAsync(ItemDaListaAsync(listaId, itemId, userId));
        if (!Repeticao.EhOcorrencia(item, dataOriginal))
            throw new RegraNegocioException("Esta data não é uma ocorrência do item.");

        var agora = Agora;
        await itemListaRepository.GravarOcorrenciaAsync(itemId, dataOriginal, userId, o =>
        {
            if (dto.Feito is { } feito)
            {
                o.FeitoEm = !feito ? null : o.Feito ? o.FeitoEm : agora;
                o.Feito = feito;
            }
            if (dto.Data is { } data)
                o.NovaData = data == dataOriginal ? null : data;
            if (dto.Hora is { } hora)
                o.NovaHora = hora == item.Hora ? null : hora;
            if (dto.Pulada is { } pulada)
                o.Pulada = pulada;
        });
    }

    public async Task<ItemListaDTO> DividirAsync(int listaId, int itemId, DateOnly aPartirDe, ItemListaDTO dto, int userId)
    {
        var antigo = await QueRepeteAsync(ItemDaListaAsync(listaId, itemId, userId));

        if (aPartirDe <= antigo.Data)
        {
            dto.Data ??= antigo.Data;
            await AtualizarNaListaAsync(listaId, itemId, dto, userId);
            dto.Id = itemId;
            dto.ListaId = listaId;
            return dto;
        }
        if (!Repeticao.EhOcorrencia(antigo, aPartirDe))
            throw new RegraNegocioException("Esta data não é uma ocorrência do item.");

        dto.Data ??= aPartirDe;
        var novo = NovoItem(listaId, dto, userId);
        novo.ItemOrigemId = antigo.Id;
        antigo.RepetirAte = aPartirDe.AddDays(-1);

        return await unitOfWork.ExecuteAsync(async () =>
        {
            await itemListaRepository.AtualizarAsync(antigo, userId);
            await itemListaRepository.ApagarOcorrenciasAsync(itemId, aPartirDe, userId);
            return Mapper.Map<ItemListaDTO>(await itemListaRepository.CriarAsync(novo));
        });
    }

    public async Task EncerrarAsync(int listaId, int itemId, DateOnly aPartirDe, int userId)
    {
        var item = await QueRepeteAsync(ItemDaListaAsync(listaId, itemId, userId));

        if (aPartirDe <= item.Data)
        {
            await itemListaRepository.RemoverAsync(itemId, userId);
            return;
        }

        item.RepetirAte = aPartirDe.AddDays(-1);
        await unitOfWork.ExecuteAsync(async () =>
        {
            await itemListaRepository.AtualizarAsync(item, userId);
            await itemListaRepository.ApagarOcorrenciasAsync(itemId, aPartirDe, userId);
        });
    }
}
