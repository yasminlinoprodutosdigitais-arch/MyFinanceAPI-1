using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;
using Npgsql;

namespace MyFinanceAPI.Data.Repositories;

public class PessoaMovimentacaoRepository(ContextDB context) : RepositorioDoUsuario<PessoaMovimentacao>(context), IPessoaMovimentacaoRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Pessoa não encontrada.";

    protected override Expression<Func<PessoaMovimentacao, bool>> Proprios(int userId) => p => p.UsuarioId == userId;

    protected override IQueryable<PessoaMovimentacao> ComDetalhes(IQueryable<PessoaMovimentacao> query) =>
        query.Include(p => p.Categoria).Include(p => p.TipoMovimentacao);

    protected override IOrderedQueryable<PessoaMovimentacao> Ordenar(IQueryable<PessoaMovimentacao> query) =>
        query.OrderBy(p => p.NomePessoa);

    protected override void CopiarAlteracoes(PessoaMovimentacao existente, PessoaMovimentacao entrada)
    {
        existente.NomePessoa = Normalizar(entrada.NomePessoa)
            ?? throw new RegraNegocioException("O nome da pessoa é obrigatório.");
        existente.CategoriaId = entrada.CategoriaId;
        existente.TipoMovimentacaoId = entrada.TipoMovimentacaoId;
    }

    /// <summary>
    /// Forma única do nome gravado (UX_PessoaMovimentacao_UserId_NomePessoa): trim + maiúsculas.
    /// Nome vazio vira null — o chamador decide não vincular pessoa.
    /// </summary>
    private static string? Normalizar(string? nome) =>
        string.IsNullOrWhiteSpace(nome) ? null : nome.Trim().ToUpperInvariant();

    private static bool ViolaUnique(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    public override Task<PessoaMovimentacao> CriarAsync(PessoaMovimentacao entidade) =>
        ObterOuCriarAsync(entidade, entidade.UsuarioId);

    /// <summary>
    /// Busca-ou-cria: se outra gravação já criou a mesma pessoa (unique), devolve a existente.
    /// </summary>
    public async Task<PessoaMovimentacao> ObterOuCriarAsync(PessoaMovimentacao pessoa, int userId)
    {
        var nome = Normalizar(pessoa.NomePessoa)
            ?? throw new RegraNegocioException("O nome da pessoa é obrigatório.");

        pessoa.UsuarioId = userId;
        pessoa.NomePessoa = nome;
        await _context.PessoasMovimentacao.AddAsync(pessoa);
        try
        {
            await _context.SaveChangesAsync();
            return pessoa;
        }
        catch (DbUpdateException ex) when (ViolaUnique(ex))
        {
            // Sem o detach, a entidade com erro ficaria no contexto e derrubaria o próximo SaveChanges.
            _context.Entry(pessoa).State = EntityState.Detached;
            return await _context.PessoasMovimentacao.FirstAsync(p => p.UsuarioId == userId && p.NomePessoa == nome);
        }
    }

    public override async Task AtualizarAsync(PessoaMovimentacao entrada, int userId)
    {
        var existente = await ObterProprioAsync(entrada.Id, userId)
            ?? throw new NaoEncontradoException(MensagemNaoEncontrado);

        CopiarAlteracoes(existente, entrada);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ViolaUnique(ex))
        {
            await _context.Entry(existente).ReloadAsync();
            throw new RegraNegocioException($"Já existe uma pessoa chamada {Normalizar(entrada.NomePessoa)}.");
        }
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.PessoasMovimentacao.AnyAsync(p => p.Id == id && p.UsuarioId == userId))
            return false;

        return await _context.ItensExtratoBancario.AnyAsync(i => i.PessoaMovimentacaoId == id);
    }

    public async Task<IEnumerable<PessoaMovimentacao>> VerificaPossuiPessoa(string? nomePessoa, int userId)
    {
        var nome = Normalizar(nomePessoa);
        if (nome == null)
            return [];

        return await ComDetalhes(_context.PessoasMovimentacao.AsNoTracking())
            .Where(a => a.UsuarioId == userId && a.NomePessoa == nome)
            .OrderBy(c => c.NomePessoa)
            .ToListAsync();
    }
}
