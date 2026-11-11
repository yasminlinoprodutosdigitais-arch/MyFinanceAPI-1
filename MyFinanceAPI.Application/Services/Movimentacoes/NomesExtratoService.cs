using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #79 — "Nomes do extrato": os nomes que vieram nos extratos ("CIRCUITO") e a regra que os classifica. Classificar
/// um nome cria (ou altera) a regra "nome do extrato é igual a"; juntar parecidos deixa uma regra só no grupo.
/// </summary>
public class NomesExtratoService(IRegraClassificacaoRepository regras, IExtratoBancarioItemRepository itens, IUnitOfWork unitOfWork)
    : INomesExtratoService
{
    public async Task<IReadOnlyList<NomeExtratoDTO>> ListarAsync(int userId) =>
        Montar(await itens.ListarParaClassificacaoAsync(userId), await regras.ListarAsync(userId));

    public async Task<IReadOnlyList<GrupoParecidosDTO>> GruposAsync(int userId) =>
        Grupos(await regras.ListarAsync(userId), await itens.ListarParaClassificacaoAsync(userId));

    /// <summary>Um por nome do extrato, os com mais lançamentos primeiro. Sem o "Eu" e sem lançamento sem nome.</summary>
    public static List<NomeExtratoDTO> Montar(IEnumerable<ExtratoBancarioItem> lancamentos, IEnumerable<RegraClassificacao> cadastradas)
    {
        var ligadas = cadastradas.Where(r => r.Ativa).OrderBy(r => r.Ordem).ThenBy(r => r.Id).ToList();
        var todos = lancamentos.ToList();
        // Tipo mais usado com cada categoria, em todos os lançamentos (para completar a sugestão).
        var tipoPorCategoria = TipoMaisUsadoPorCategoria(todos);

        return todos
            .Select(i => (Item: i, Chave: NomeExtrato.Chave(i.Descricao, i.NomePessoaTransacao)))
            .Where(x => x.Chave.Length >= 2)
            .GroupBy(x => x.Chave)
            .Select(g =>
            {
                var doNome = g.Select(x => x.Item).ToList();
                var recente = doNome.OrderByDescending(i => i.DataMovimentacao).ThenByDescending(i => i.Id).First();
                var nome = new NomeExtratoDTO
                {
                    Texto = g.Key,
                    Lancamentos = doNome.Count,
                    UltimoUso = recente.DataMovimentacao,
                    SemCategoria = doNome.Count(SemCategoria)
                };

                var regra = ClassificadorRegras.Encontrar(ligadas, recente.Descricao, recente.NomePessoaTransacao, recente.BancoId);
                var parecida = regra == null ? ClassificadorRegras.EncontrarPorSemelhanca(ligadas, recente.Descricao, recente.NomePessoaTransacao, recente.BancoId) : null;
                var usada = regra ?? parecida;
                nome.Cobertura = regra != null
                    ? (EhIgualDoNome(regra, g.Key) ? CoberturaNome.Igual : CoberturaNome.Outra)
                    : parecida != null ? CoberturaNome.Semelhanca : CoberturaNome.Nenhuma;
                nome.RegraId = usada?.Id;
                nome.RegraTexto = usada?.Texto;
                nome.CategoriaId = usada?.CategoriaId;
                nome.TipoMovimentacaoId = usada?.TipoMovimentacaoId;

                if (regra == null)
                {
                    var historico = doNome.Where(i => i.CategoriaId != null && !i.ClassificadoPorSemelhanca)
                        .GroupBy(i => i.CategoriaId!.Value)
                        .OrderByDescending(c => c.Count()).ThenByDescending(c => c.Max(i => i.DataMovimentacao))
                        .FirstOrDefault();
                    if (historico != null)
                    {
                        nome.SugestaoCategoriaId = historico.Key;
                        nome.SugestaoTipoMovimentacaoId = historico.Where(i => i.TipoMovimentacaoId != null)
                            .GroupBy(i => i.TipoMovimentacaoId!.Value).OrderByDescending(t => t.Count()).Select(t => (int?)t.Key).FirstOrDefault()
                            ?? tipoPorCategoria.GetValueOrDefault(historico.Key);
                    }
                    else if (parecida != null)
                    {
                        nome.SugestaoCategoriaId = parecida.CategoriaId;
                        nome.SugestaoTipoMovimentacaoId = parecida.TipoMovimentacaoId;
                    }
                }
                return nome;
            })
            .OrderByDescending(n => n.Lancamentos).ThenBy(n => n.Texto, StringComparer.Ordinal)
            .ToList();
    }

    public Task<ClassificacaoNomesResultadoDTO> ClassificarAsync(int userId, ClassificarNomesDTO pedido) =>
        unitOfWork.ExecuteAsync(async () =>
        {
            var pedidos = (pedido.Itens ?? [])
                .Select(p => (Texto: RegraClassificacao.Normalizar(p.Texto) ?? "", p.CategoriaId, p.TipoMovimentacaoId))
                .Where(p => p.Texto.Length > 0)
                .GroupBy(p => p.Texto).Select(g => g.Last())
                .ToList();
            if (pedidos.Any(p => p.Texto.Length < 2))
                throw new RegraNegocioException("O nome precisa ter pelo menos 2 letras.");
            if (pedidos.Any(p => p.CategoriaId is not > 0 && p.TipoMovimentacaoId is not > 0))
                throw new RegraNegocioException("Escolha uma categoria ou um tipo para cada nome.");

            var existentes = await regras.ListarAsync(userId);
            foreach (var (texto, categoriaId, tipoId) in pedidos)
            {
                var regra = existentes.FirstOrDefault(r => EhIgualDoNome(r, texto) && r.BancoId == null);
                if (regra != null)
                {
                    regra.CategoriaId = categoriaId is > 0 ? categoriaId : null;
                    regra.TipoMovimentacaoId = tipoId is > 0 ? tipoId : null;
                    regra.Ativa = true;
                    await regras.AtualizarAsync(regra, userId);
                }
                else
                {
                    await regras.CriarAsync(new RegraClassificacao
                    {
                        UsuarioId = userId, Texto = texto, Campo = CamposRegra.NomeExtrato, Operador = OperadoresRegra.Igual,
                        CategoriaId = categoriaId is > 0 ? categoriaId : null, TipoMovimentacaoId = tipoId is > 0 ? tipoId : null, Ativa = true
                    });
                }
            }

            var atualizados = 0;
            if (pedido.AplicarAosImportados && pedidos.Count > 0)
            {
                var porTexto = pedidos.ToDictionary(p => p.Texto);
                var alvos = (await itens.ListarParaClassificacaoAsync(userId))
                    .Where(i => (SemCategoria(i) || i.ClassificadoPorSemelhanca)
                        && porTexto.ContainsKey(NomeExtrato.Chave(i.Descricao, i.NomePessoaTransacao)))
                    .ToList();
                foreach (var item in alvos)
                {
                    var p = porTexto[NomeExtrato.Chave(item.Descricao, item.NomePessoaTransacao)];
                    Aplicar(item, p.CategoriaId, p.TipoMovimentacaoId);
                }
                atualizados = await GravarAsync(alvos);
            }
            return new ClassificacaoNomesResultadoDTO { Regras = pedidos.Count, Lancamentos = atualizados };
        });

    /// <summary>
    /// Regras ligadas ou não, "igual a" (nome do extrato ou pessoa), em todas as contas, agrupadas por nome parecido;
    /// só grupos de 2 ou mais. Os que classificam diferente primeiro.
    /// </summary>
    public static List<GrupoParecidosDTO> Grupos(IEnumerable<RegraClassificacao> cadastradas, IEnumerable<ExtratoBancarioItem> lancamentos)
    {
        var candidatas = cadastradas
            .Where(r => r.Operador == OperadoresRegra.Igual && r.BancoId == null && r.Campo is CamposRegra.NomeExtrato or CamposRegra.Pessoa)
            .OrderBy(r => r.Ordem).ThenBy(r => r.Id)
            .ToList();
        var porNome = lancamentos
            .GroupBy(i => NomeExtrato.Chave(i.Descricao, i.NomePessoaTransacao))
            .ToDictionary(g => g.Key, g => g.Count());

        var grupos = new List<List<RegraClassificacao>>();
        foreach (var regra in candidatas)
        {
            var junto = grupos.Where(g => g.Any(r => NomeExtrato.Parecidos(r.Texto, regra.Texto))).ToList();
            if (junto.Count == 0)
            {
                grupos.Add([regra]);
                continue;
            }
            // Uma regra pode ligar dois grupos: viram um só.
            junto[0].Add(regra);
            foreach (var outro in junto.Skip(1))
            {
                junto[0].AddRange(outro);
                grupos.Remove(outro);
            }
        }

        return grupos
            .Where(g => g.Count >= 2)
            .Select(g => new GrupoParecidosDTO
            {
                Divergem = g.Select(r => (r.CategoriaId, r.TipoMovimentacaoId)).Distinct().Count() > 1,
                Regras = g.Select(r => new RegraParecidaDTO
                {
                    Id = r.Id, Texto = r.Texto, CategoriaId = r.CategoriaId, TipoMovimentacaoId = r.TipoMovimentacaoId,
                    Lancamentos = porNome.GetValueOrDefault(r.Texto)
                }).ToList()
            })
            .OrderByDescending(g => g.Divergem).ThenBy(g => g.Regras[0].Texto, StringComparer.Ordinal)
            .ToList();
    }

    public Task<ClassificacaoNomesResultadoDTO> JuntarAsync(int userId, JuntarNomesDTO pedido) =>
        unitOfWork.ExecuteAsync(async () =>
        {
            var ids = (pedido.RegraIds ?? []).Distinct().ToList();
            if (!ids.Contains(pedido.ManterRegraId))
                throw new RegraNegocioException("Escolha qual regra vale entre as do grupo.");
            var doUsuario = (await regras.ListarAsync(userId)).Where(r => ids.Contains(r.Id)).ToList();
            if (doUsuario.Count != ids.Count)
                throw new RegraNegocioException("Regra não encontrada.");

            var manter = doUsuario.Single(r => r.Id == pedido.ManterRegraId);
            var apagar = doUsuario.Where(r => r.Id != manter.Id).ToList();
            foreach (var regra in apagar)
                await regras.RemoverAsync(regra.Id, userId);

            var atualizados = 0;
            if (pedido.AplicarAosImportados)
            {
                var textos = doUsuario.Select(r => r.Texto).ToList();
                var categoriasApagadas = apagar.Select(r => r.CategoriaId).OfType<int>().ToHashSet();
                var alvos = (await itens.ListarParaClassificacaoAsync(userId))
                    .Where(i => !i.Dividido)
                    .Where(i =>
                    {
                        var chave = NomeExtrato.Chave(i.Descricao, i.NomePessoaTransacao);
                        return textos.Any(t => t == chave || NomeExtrato.Parecidos(t, chave));
                    })
                    // Escolha à mão (categoria que nenhuma regra do grupo dava) fica.
                    .Where(i => SemCategoria(i) || i.ClassificadoPorSemelhanca || (i.CategoriaId is int c && categoriasApagadas.Contains(c)))
                    .Where(i => i.CategoriaId != manter.CategoriaId || i.ClassificadoPorSemelhanca)
                    .ToList();
                foreach (var item in alvos)
                    Aplicar(item, manter.CategoriaId, manter.TipoMovimentacaoId);
                atualizados = await GravarAsync(alvos);
            }
            return new ClassificacaoNomesResultadoDTO { Regras = apagar.Count, Lancamentos = atualizados };
        });

    private static bool EhIgualDoNome(RegraClassificacao r, string texto) =>
        r.Operador == OperadoresRegra.Igual && r.Campo is CamposRegra.NomeExtrato or CamposRegra.Pessoa && r.Texto == texto;

    /// <summary>Lançamento dividido já foi classificado pelas partes (card #76, fase 3).</summary>
    private static bool SemCategoria(ExtratoBancarioItem i) => i.CategoriaId == null && !i.Dividido;

    private static void Aplicar(ExtratoBancarioItem item, int? categoriaId, int? tipoId)
    {
        item.CategoriaId = categoriaId is > 0 ? categoriaId : item.CategoriaId;
        item.TipoMovimentacaoId = tipoId is > 0 ? tipoId : item.TipoMovimentacaoId;
        item.ClassificadoPorSemelhanca = false;
        // Lidos sem rastreio e com as navegações: sem isso o UpdateRange gravaria a categoria antiga junto.
        item.Categoria = null;
        item.TipoMovimentacao = null;
    }

    private async Task<int> GravarAsync(List<ExtratoBancarioItem> alvos)
    {
        if (alvos.Count > 0)
            await itens.UpdateRangeAsync(alvos);
        return alvos.Count;
    }

    private static Dictionary<int, int> TipoMaisUsadoPorCategoria(IEnumerable<ExtratoBancarioItem> todos) =>
        todos.Where(i => i.CategoriaId != null && i.TipoMovimentacaoId != null)
            .GroupBy(i => i.CategoriaId!.Value)
            .ToDictionary(g => g.Key, g => g.GroupBy(i => i.TipoMovimentacaoId!.Value).OrderByDescending(t => t.Count()).First().Key);
}
