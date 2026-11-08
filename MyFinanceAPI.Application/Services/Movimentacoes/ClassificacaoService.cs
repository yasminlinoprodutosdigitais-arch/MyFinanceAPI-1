using System.Text.RegularExpressions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #76, fase 1: o que as regras fazem com os lançamentos que já existem (prévia ao vivo e aplicar, só nos sem
/// categoria), a sugestão de categoria pelo histórico e as sugestões de regra (textos repetidos sem categoria).
/// </summary>
public partial class ClassificacaoService(IRegraClassificacaoRepository regras, IExtratoBancarioItemRepository itens) : IClassificacaoService
{
    private const int MaxExemplos = 5;
    private const int MaxExemplosConflito = 3;
    private const int MaxSugestoesDeRegra = 5;

    public async Task<PreviaRegraResultadoDTO> PreviaAsync(int userId, PreviaRegraDTO previa)
    {
        var texto = RegraClassificacao.Normalizar(previa.Texto);
        if (texto == null || texto.Length < 2)
            return new PreviaRegraResultadoDTO();

        var regra = new RegraClassificacao
        {
            Texto = texto,
            Campo = RegraClassificacaoService.CampoOuPadrao(previa.Campo),
            Operador = RegraClassificacaoService.OperadorOuPadrao(previa.Operador),
            BancoId = previa.BancoId is > 0 ? previa.BancoId : null,
            Ativa = true
        };
        var batem = (await itens.ListarParaClassificacaoAsync(userId)).Where(i => Bate(regra, i)).ToList();
        var categoria = previa.CategoriaId is > 0 ? previa.CategoriaId : null;
        var conflitos = batem
            .Where(i => categoria != null && i.CategoriaId != null && i.CategoriaId != categoria)
            .OrderByDescending(i => i.DataMovimentacao).ThenByDescending(i => i.Id)
            .ToList();

        return new PreviaRegraResultadoDTO
        {
            Total = batem.Count,
            SemCategoria = batem.Count(SemCategoria),
            Conflitos = conflitos.Count,
            Exemplos = batem
                .OrderBy(i => i.CategoriaId == null ? 0 : 1)
                .ThenByDescending(i => i.DataMovimentacao).ThenByDescending(i => i.Id)
                .Take(MaxExemplos).Select(Exemplo).ToList(),
            ExemplosConflito = conflitos.Take(MaxExemplosConflito).Select(Exemplo).ToList()
        };
    }

    public async Task<AplicacaoRegraDTO> AplicarAsync(int userId, int regraId)
    {
        var regra = await regras.ObterPorIdAsync(regraId, userId)
            ?? throw new NaoEncontradoException("Regra não encontrada.");

        // Ligada ou não: "aplicar de novo" é um pedido explícito.
        regra.Ativa = true;
        var alvos = (await itens.ListarParaClassificacaoAsync(userId))
            .Where(i => SemCategoria(i) && Bate(regra, i))
            .ToList();
        foreach (var item in alvos)
        {
            item.CategoriaId = regra.CategoriaId ?? item.CategoriaId;
            item.TipoMovimentacaoId ??= regra.TipoMovimentacaoId;
            item.PessoaMovimentacaoId ??= regra.PessoaMovimentacaoId;
        }

        if (alvos.Count > 0)
        {
            await itens.UpdateRangeAsync(alvos);
            await regras.RegistrarUsoAsync(userId, new Dictionary<int, int> { [regra.Id] = alvos.Count });
        }
        return new AplicacaoRegraDTO { Quantidade = alvos.Count, ItemIds = alvos.Select(i => i.Id).ToList() };
    }

    public async Task<IReadOnlyList<SugestaoCategoriaDTO>> SugestoesAsync(int userId, IEnumerable<int> itemIds)
    {
        var ids = itemIds.ToHashSet();
        if (ids.Count == 0)
            return [];

        var todos = await itens.ListarParaClassificacaoAsync(userId);
        return Sugerir(todos.Where(i => i.CategoriaId != null), todos.Where(i => ids.Contains(i.Id) && SemCategoria(i)));
    }

    public async Task<IReadOnlyList<SugestaoRegraDTO>> SugestoesDeRegraAsync(int userId)
    {
        var todos = await itens.ListarParaClassificacaoAsync(userId);
        var existentes = await regras.ListarAsync(userId);
        return SugerirRegras(todos, existentes);
    }

    /// <summary>
    /// Textos (o <see cref="TextoSugerido"/>) que aparecem em 2 ou mais lançamentos sem categoria, sem regra com o mesmo
    /// texto e sem regra ligada que já pegue esses lançamentos. Com a categoria mais usada nesse texto, se houver.
    /// </summary>
    public static List<SugestaoRegraDTO> SugerirRegras(IEnumerable<ExtratoBancarioItem> todos, IEnumerable<RegraClassificacao> existentes)
    {
        var lista = todos.ToList();
        var regrasLista = existentes.ToList();
        var textosComRegra = regrasLista.Select(r => r.Texto).ToHashSet();
        var ligadas = regrasLista.Where(r => r.Ativa).ToList();

        var historico = lista
            .Where(i => i.CategoriaId != null)
            .GroupBy(i => RegraClassificacao.Normalizar(TextoSugerido(i.Descricao, i.NomePessoaTransacao)) ?? "")
            .Where(g => g.Key.Length >= 2)
            .ToDictionary(g => g.Key, g => g
                .GroupBy(i => (i.CategoriaId!.Value, Nome: i.Categoria?.Nome))
                .Select(c => (c.Key.Value, c.Key.Nome, Vezes: c.Count(), Ultima: c.Max(i => i.DataMovimentacao)))
                .OrderByDescending(c => c.Vezes).ThenByDescending(c => c.Ultima)
                .First());

        return lista
            .Where(SemCategoria)
            .Where(i => ClassificadorRegras.Encontrar(ligadas, i.Descricao, i.NomePessoaTransacao, i.BancoId) == null)
            .GroupBy(i => RegraClassificacao.Normalizar(TextoSugerido(i.Descricao, i.NomePessoaTransacao)) ?? "")
            .Where(g => g.Key.Length >= 2 && g.Count() >= 2 && !textosComRegra.Contains(g.Key))
            .Select(g =>
            {
                var achou = historico.TryGetValue(g.Key, out var h);
                return new SugestaoRegraDTO
                {
                    Texto = g.Key,
                    Quantidade = g.Count(),
                    CategoriaId = achou ? h.Value : null,
                    CategoriaNome = achou ? h.Nome : null,
                    VezesCategoria = achou ? h.Vezes : 0
                };
            })
            .OrderByDescending(s => s.Quantidade).ThenBy(s => s.Texto, StringComparer.Ordinal)
            .Take(MaxSugestoesDeRegra)
            .ToList();
    }

    /// <summary>
    /// O texto que uma regra usaria para este lançamento (o mesmo do front): o nome da pessoa, menos "Eu"; senão a
    /// descrição depois do último " - ". Corta no "*" ("IFOOD *RESTAURANTE" vira "IFOOD"), menos depois de sigla curta ("MP *LOJA"), e tira os números.
    /// </summary>
    public static string TextoSugerido(string? descricao, string? nomePessoa)
    {
        var pessoa = (nomePessoa ?? "").Trim();
        var usarPessoa = pessoa.Length > 0 && RegraClassificacao.Normalizar(pessoa) != "EU";
        var base_ = usarPessoa ? pessoa : (descricao ?? "");
        var traco = base_.LastIndexOf(" - ", StringComparison.Ordinal);
        if (!usarPessoa && traco >= 0)
            base_ = base_[(traco + 3)..];
        // "IFOOD *RESTAURANTE" vira "IFOOD"; mas sigla de intermediador ("MP *LOJA", "DL*GOOGLE") fica com a loja.
        var asterisco = base_.IndexOf('*');
        var semAsterisco = asterisco >= 0 && base_[..asterisco].Trim().Length > 3 ? base_[..asterisco] : base_;
        return Espacos().Replace(Digitos().Replace(semAsterisco, ""), " ").Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Para cada lançamento sem categoria, a categoria mais usada nos lançamentos com a mesma descrição sem números (ou o
    /// mesmo nome de pessoa, quando não há descrição). Empate: a mais recente. Sem histórico, sem sugestão.
    /// </summary>
    public static List<SugestaoCategoriaDTO> Sugerir(IEnumerable<ExtratoBancarioItem> historico, IEnumerable<ExtratoBancarioItem> semCategoria)
    {
        var porChave = historico
            .Where(i => i.CategoriaId != null)
            .GroupBy(Chave)
            .Where(g => g.Key.Length > 0)
            .ToDictionary(g => g.Key, g => g
                .GroupBy(i => i.CategoriaId!.Value)
                .Select(c => (CategoriaId: c.Key, Vezes: c.Count(), Ultima: c.Max(i => i.DataMovimentacao)))
                .OrderByDescending(c => c.Vezes).ThenByDescending(c => c.Ultima)
                .First());

        return semCategoria
            .Select(i => (Item: i, Achou: porChave.TryGetValue(Chave(i), out var s), Sugestao: s))
            .Where(x => x.Achou)
            .Select(x => new SugestaoCategoriaDTO { ItemId = x.Item.Id, CategoriaId = x.Sugestao.CategoriaId, Vezes = x.Sugestao.Vezes })
            .ToList();
    }

    /// <summary>Lançamento dividido já foi classificado pelas partes (card #76, fase 3).</summary>
    private static bool SemCategoria(ExtratoBancarioItem i) => i.CategoriaId == null && !i.Dividido;

    private static bool Bate(RegraClassificacao regra, ExtratoBancarioItem item) =>
        ClassificadorRegras.Bate(regra, RegraClassificacao.Normalizar(item.Descricao),
            RegraClassificacao.Normalizar(item.NomePessoaTransacao), item.BancoId);

    private static ExemploRegraDTO Exemplo(ExtratoBancarioItem i) => new()
    {
        Id = i.Id,
        DataMovimentacao = i.DataMovimentacao,
        Texto = string.IsNullOrWhiteSpace(i.Descricao) ? i.NomePessoaTransacao ?? "" : i.Descricao,
        Valor = i.Valor,
        TipoLancamento = i.TipoLancamento,
        CategoriaId = i.CategoriaId,
        CategoriaNome = i.Categoria?.Nome
    };

    /// <summary>A descrição sem números (como a ChaveDescricao da importação), ou o nome da pessoa sem descrição.</summary>
    private static string Chave(ExtratoBancarioItem i)
    {
        var base_ = string.IsNullOrWhiteSpace(i.Descricao) ? i.NomePessoaTransacao : i.Descricao;
        return RegraClassificacao.Normalizar(ExtratoBancarioService.GerarChaveDescricao(base_)) ?? "";
    }

    [GeneratedRegex("[0-9]")]
    private static partial Regex Digitos();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Espacos();
}
