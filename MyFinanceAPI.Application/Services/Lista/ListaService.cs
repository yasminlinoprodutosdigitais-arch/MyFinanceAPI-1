using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>Listas com campos configuráveis (card #67): resumo, pasta Arquivadas e duplicar.</summary>
public class ListaService(
    IListaRepository listaRepository,
    IItemListaRepository itemListaRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    TimeProvider relogio)
    : ServicoCadastro<Lista, ListaDTO>(listaRepository, mapper), IListaService
{
    private const string SufixoCopia = " (cópia)";
    private const int TamanhoNome = 120;

    private static readonly TimeZoneInfo SaoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    /// <summary>"Próximo item com data" compara com o dia de quem usa, não com o do servidor (UTC).</summary>
    private DateOnly Hoje => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(relogio.GetUtcNow(), SaoPaulo).DateTime);

    protected override Task ValidarGravacaoAsync(ListaDTO dto, int userId, int ignorarId)
    {
        if (dto.UsaLimite && !dto.UsaValor)
            throw new RegraNegocioException("O limite de gasto só pode ser ligado com o campo Valor ligado.");
        if (dto.UsaLimite && dto.Limite is null)
            throw new RegraNegocioException("Informe o limite de gasto.");
        if (!IconePrimeIcons.Valido(dto.Icone))
            throw new RegraNegocioException($"Ícone inválido: {dto.Icone}.");

        return Task.CompletedTask;
    }

    private ListaDTO ParaDto(ListaComResumo r)
    {
        var dto = Mapper.Map<ListaDTO>(r.Lista);
        dto.Resumo = new ResumoListaDTO
        {
            Itens = r.Itens,
            Feitos = r.Feitos,
            Total = r.Total,
            TotalFeito = r.TotalFeito,
            ProximaData = r.ProximaData,
        };
        return dto;
    }

    /// <summary>
    /// A consulta do resumo dá a próxima data só dos itens que não repetem; aqui entra a próxima ocorrência
    /// pendente dos que repetem (card #67, parte 3), e vale a menor das duas.
    /// </summary>
    private async Task<List<ListaDTO>> ParaDtosAsync(IReadOnlyCollection<ListaComResumo> listas, int userId)
    {
        var dtos = listas.Select(ParaDto).ToList();
        if (dtos.Count == 0)
            return dtos;

        var repetem = await itemListaRepository.ListarQueRepetemAsync(dtos.Select(d => d.Id).ToList(), userId);
        if (repetem.Count == 0)
            return dtos;

        var excecoes = await itemListaRepository.ListarOcorrenciasAsync(repetem.Select(i => i.Id).ToList(), userId);
        var hoje = Hoje;
        foreach (var dto in dtos)
            foreach (var item in repetem.Where(i => i.ListaId == dto.Id))
                if (Repeticao.ProximaPendente(item, excecoes.Where(e => e.ItemId == item.Id), hoje) is { } proxima
                    && (dto.Resumo!.ProximaData is null || proxima < dto.Resumo.ProximaData))
                    dto.Resumo!.ProximaData = proxima;
        return dtos;
    }

    public override Task<IEnumerable<ListaDTO>> ListarAsync(int userId) => ListarAsync(userId, arquivadas: false);

    public async Task<IEnumerable<ListaDTO>> ListarAsync(int userId, bool arquivadas) =>
        await ParaDtosAsync(await listaRepository.ListarComResumoAsync(userId, arquivadas, Hoje), userId);

    public override async Task<ListaDTO?> ObterPorIdAsync(int id, int userId)
    {
        var lista = await listaRepository.ObterComResumoAsync(id, userId, Hoje);
        return lista is null ? null : (await ParaDtosAsync([lista], userId)).Single();
    }

    public Task ArquivarAsync(int id, int userId) =>
        listaRepository.DefinirArquivadaAsync(id, userId, relogio.GetUtcNow().UtcDateTime);

    public Task DesarquivarAsync(int id, int userId) =>
        listaRepository.DefinirArquivadaAsync(id, userId, null);

    private async Task<Lista> ObterAsync(int id, int userId) =>
        await listaRepository.ObterPorIdAsync(id, userId) ?? throw new NaoEncontradoException("Lista não encontrada.");

    public async Task<IEnumerable<ListaDTO>> ListarDaSerieAsync(int id, int userId)
    {
        var lista = await ObterAsync(id, userId);
        return await ParaDtosAsync(await listaRepository.ListarDaSerieAsync(lista.SerieId, userId, Hoje), userId);
    }

    public async Task<ComparacaoListasDTO> CompararAsync(int id, int? com, bool soFeitos, int userId)
    {
        if (com == id)
            throw new RegraNegocioException("Escolha outra lista para comparar.");

        var atual = await ObterAsync(id, userId);
        var serie = (await listaRepository.ListarDaSerieAsync(atual.SerieId, userId, Hoje)).Select(r => r.Lista).ToList();

        Lista anterior;
        if (com is null)
            anterior = serie.Where(l => l.Id < atual.Id).MaxBy(l => l.Id)
                ?? throw new RegraNegocioException("Não há lista anterior na série.");
        else
        {
            anterior = await ObterAsync(com.Value, userId);
            if (anterior.SerieId != atual.SerieId)
                throw new RegraNegocioException("As listas não são da mesma série.");
        }

        // Os itens da série inteira: a corrente de origens pode passar por listas do meio (out → nov → dez).
        var itens = await itemListaRepository.ListarDasListasAsync(serie.Select(l => l.Id).ToList(), userId);
        return ComparacaoListas.Comparar(
            anterior, itens.Where(i => i.ListaId == anterior.Id),
            atual, itens.Where(i => i.ListaId == atual.Id),
            itens.ToDictionary(i => i.Id, i => i.ItemOrigemId), soFeitos);
    }

    public async Task DefinirSerieAsync(int id, int? comListaId, int userId)
    {
        await ObterAsync(id, userId);
        var serie = comListaId is null ? Guid.NewGuid() : (await ObterAsync(comListaId.Value, userId)).SerieId;
        await listaRepository.DefinirSerieAsync(id, userId, serie);
    }

    public async Task<ListaDTO> DuplicarAsync(int id, DuplicarListaDTO opcoes, int userId)
    {
        var original = await ObterAsync(id, userId);
        List<ItemLista> itens = opcoes.CopiarItens ? await itemListaRepository.ListarDaListaAsync(id, userId) : [];

        var copia = new Lista
        {
            UsuarioId = userId,
            SerieId = original.SerieId,
            ListaOrigemId = original.Id,
            Nome = string.IsNullOrWhiteSpace(opcoes.Nome) ? NomeDaCopia(original.Nome) : opcoes.Nome.Trim(),
            Icone = original.Icone,
            UsaQuantidade = original.UsaQuantidade,
            UsaValor = original.UsaValor,
            ModoValor = original.ModoValor,
            UsaLimite = original.UsaLimite,
            Limite = original.Limite,
            UsaData = original.UsaData,
            UsaObservacao = original.UsaObservacao,
            Modelo = original.Modelo,
        };

        return await unitOfWork.ExecuteAsync(async () =>
        {
            var criada = await listaRepository.CriarAsync(copia);
            if (itens.Count > 0)
                await itemListaRepository.CriarVariosAsync(itens.Select(i => new ItemLista
                {
                    ListaId = criada.Id,
                    UsuarioId = userId,
                    ItemOrigemId = i.Id,
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    Valor = opcoes.ManterValores ? i.Valor : null,
                    Data = i.Data,
                    Hora = i.Hora,
                    Repeticao = i.Repeticao,
                    DiasSemana = i.DiasSemana,
                    RepetirAte = i.RepetirAte,
                    Observacao = i.Observacao,
                    Ordem = i.Ordem,
                }).ToList());
            if (opcoes.ArquivarOriginal && original.ArquivadaEm is null)
                await listaRepository.DefinirArquivadaAsync(original.Id, userId, relogio.GetUtcNow().UtcDateTime);
            return Mapper.Map<ListaDTO>(criada);
        });
    }

    private static string NomeDaCopia(string nome)
    {
        var base_ = nome.Trim();
        if (base_.Length + SufixoCopia.Length > TamanhoNome)
            base_ = base_[..(TamanhoNome - SufixoCopia.Length)];
        return base_ + SufixoCopia;
    }
}
