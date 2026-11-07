using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

/// <summary>Regras de classificação (card #76, fase 1), sempre do próprio usuário e na ordem de prioridade.</summary>
public class RegraClassificacaoRepository(ContextDB context) : RepositorioDoUsuario<RegraClassificacao>(context), IRegraClassificacaoRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Regra não encontrada.";

    protected override Expression<Func<RegraClassificacao, bool>> Proprios(int userId) => r => r.UsuarioId == userId;

    protected override IQueryable<RegraClassificacao> ComDetalhes(IQueryable<RegraClassificacao> query) =>
        query.Include(r => r.Categoria).Include(r => r.TipoMovimentacao).Include(r => r.PessoaMovimentacao).Include(r => r.Banco);

    protected override IOrderedQueryable<RegraClassificacao> Ordenar(IQueryable<RegraClassificacao> query) =>
        query.OrderBy(r => r.Ordem).ThenBy(r => r.Id);

    protected override void CopiarAlteracoes(RegraClassificacao existente, RegraClassificacao entrada)
    {
        existente.Texto = RegraClassificacao.Normalizar(entrada.Texto)
            ?? throw new RegraNegocioException("O texto da regra é obrigatório.");
        existente.Campo = entrada.Campo;
        existente.Operador = entrada.Operador;
        existente.BancoId = entrada.BancoId;
        existente.CategoriaId = entrada.CategoriaId;
        existente.TipoMovimentacaoId = entrada.TipoMovimentacaoId;
        existente.PessoaMovimentacaoId = entrada.PessoaMovimentacaoId;
        existente.Ativa = entrada.Ativa;
        // Ordem e uso não mudam por aqui: têm caminho próprio (reordenar e registrar uso).
    }

    /// <summary>Regra nova entra no topo: as outras descem uma posição.</summary>
    public override async Task<RegraClassificacao> CriarAsync(RegraClassificacao entidade)
    {
        var outras = await _context.RegrasClassificacao.Where(r => r.UsuarioId == entidade.UsuarioId).ToListAsync();
        foreach (var r in outras)
            r.Ordem++;
        entidade.Ordem = 0;
        entidade.VezesAplicada = 0;
        entidade.UltimaAplicacao = null;
        return await base.CriarAsync(entidade);
    }

    public Task<bool> ExisteFraseAsync(int userId, string campo, string operador, string texto, int? bancoId, int ignorarId) =>
        _context.RegrasClassificacao.AnyAsync(r => r.UsuarioId == userId && r.Campo == campo && r.Operador == operador
            && r.Texto == texto && r.BancoId == bancoId && r.Id != ignorarId);

    public Task<List<RegraClassificacao>> ListarAtivasAsync(int userId) =>
        // Com a pessoa: a regra que liga uma pessoa grava o nome dela no lançamento.
        _context.RegrasClassificacao.AsNoTracking().Include(r => r.PessoaMovimentacao)
            .Where(r => r.UsuarioId == userId && r.Ativa)
            .OrderBy(r => r.Ordem).ThenBy(r => r.Id)
            .ToListAsync();

    public async Task ReordenarAsync(int userId, IReadOnlyList<int> ids)
    {
        var regras = await _context.RegrasClassificacao.Where(r => r.UsuarioId == userId)
            .OrderBy(r => r.Ordem).ThenBy(r => r.Id).ToListAsync();
        var posicao = ids.Distinct().Select((id, i) => (id, i)).ToDictionary(x => x.id, x => x.i);
        // As que não vieram na lista ficam depois, na ordem em que estavam.
        var novaOrdem = regras
            .OrderBy(r => posicao.TryGetValue(r.Id, out var p) ? p : int.MaxValue)
            .ThenBy(r => r.Ordem).ThenBy(r => r.Id)
            .ToList();
        for (var i = 0; i < novaOrdem.Count; i++)
            novaOrdem[i].Ordem = i;
        await _context.SaveChangesAsync();
    }

    public async Task RegistrarUsoAsync(int userId, IReadOnlyDictionary<int, int> vezesPorRegra)
    {
        var usadas = vezesPorRegra.Where(kv => kv.Value > 0).ToDictionary(kv => kv.Key, kv => kv.Value);
        if (usadas.Count == 0)
            return;

        var regras = await _context.RegrasClassificacao
            .Where(r => r.UsuarioId == userId && usadas.Keys.Contains(r.Id)).ToListAsync();
        var agora = DateTime.UtcNow;
        foreach (var r in regras)
        {
            r.VezesAplicada += usadas[r.Id];
            r.UltimaAplicacao = agora;
        }
        await _context.SaveChangesAsync();
    }
}
