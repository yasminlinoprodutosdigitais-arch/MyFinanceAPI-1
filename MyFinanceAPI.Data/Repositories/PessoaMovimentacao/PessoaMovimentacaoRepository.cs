using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;
using Npgsql;

namespace MyFinanceAPI.Data.Repositories;

public class PessoaMovimentacaoRepository(ContextDB context) : IPessoaMovimentacaoRepository
{
    private readonly ContextDB _context = context;

    /// <summary>
    /// Forma única do nome gravado (UX_PessoaMovimentacao_UserId_NomePessoa): trim + maiúsculas.
    /// Nome vazio vira null — o chamador decide não vincular pessoa.
    /// </summary>
    private static string? Normalizar(string? nome) =>
        string.IsNullOrWhiteSpace(nome) ? null : nome.Trim().ToUpperInvariant();

    private static bool ViolaUnique(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    /// <summary>
    /// Busca-ou-cria: se outra gravação já criou a mesma pessoa (unique), devolve a existente.
    /// </summary>
    public async Task<PessoaMovimentacao> Create(PessoaMovimentacao PessoaMovimentacao, int userId)
    {
        var nome = Normalizar(PessoaMovimentacao.NomePessoa)
            ?? throw new InvalidOperationException("O nome da pessoa é obrigatório.");

        PessoaMovimentacao.UserId = userId;
        PessoaMovimentacao.NomePessoa = nome;
        await _context.PessoaMovimentacao.AddAsync(PessoaMovimentacao);
        try
        {
            await _context.SaveChangesAsync();
            return PessoaMovimentacao;
        }
        catch (DbUpdateException ex) when (ViolaUnique(ex))
        {
            // Sem o detach, a entidade com erro ficaria no contexto e derrubaria o próximo SaveChanges.
            _context.Entry(PessoaMovimentacao).State = EntityState.Detached;
            return await _context.PessoaMovimentacao.FirstAsync(p => p.UserId == userId && p.NomePessoa == nome);
        }
    }

    public async Task<IEnumerable<PessoaMovimentacao>> GetPessoaMovimentacaoByUserId(int userId)
    {
        var PessoaMovimentacao = await _context.PessoaMovimentacao.Where(c => c.UserId == userId).OrderBy(c => c.NomePessoa).Include(a => a.Categoria).Include(a => a.TipoMovimentacao).ToListAsync();
        return PessoaMovimentacao;
    }

    public async Task<PessoaMovimentacao?> GetPessoaMovimentacaoById(int id, int userId)
    {
        var PessoaMovimentacao = await _context.PessoaMovimentacao
            .Where(c => c.UserId == userId && c.Id == id)
            .Include(c => c.Categoria)
            .Include(c => c.TipoMovimentacao)
            .FirstOrDefaultAsync();

        if (PessoaMovimentacao == null)
            return null;

        return PessoaMovimentacao;
    }

    public async Task<PessoaMovimentacao?> Remove(int id, int userId)
    {
        var PessoaMovimentacao = await GetPessoaMovimentacaoById(id, userId);
        if (PessoaMovimentacao == null)
            return null;

        _context.PessoaMovimentacao.Remove(PessoaMovimentacao);
        await _context.SaveChangesAsync();

        return PessoaMovimentacao;
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.PessoaMovimentacao.AnyAsync(p => p.Id == id && p.UserId == userId))
            return false;

        return await _context.ExtratoBancarioItens.AnyAsync(i => i.PessoaMovimentacaoId == id);
    }

    public async Task<PessoaMovimentacao?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.PessoaMovimentacao
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(PessoaMovimentacao incomingPessoaMovimentacao, int userId)
    {
        var existingPessoaMovimentacao = await _context.PessoaMovimentacao
            .FirstOrDefaultAsync(a => a.Id == incomingPessoaMovimentacao.Id && a.UserId == userId);

        if (existingPessoaMovimentacao == null)
        {
            throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
        }

        var nome = Normalizar(incomingPessoaMovimentacao.NomePessoa)
            ?? throw new InvalidOperationException("O nome da pessoa é obrigatório.");

        existingPessoaMovimentacao.NomePessoa = nome;
        existingPessoaMovimentacao.CategoriaId = incomingPessoaMovimentacao.CategoriaId;
        existingPessoaMovimentacao.TipoMovimentacaoId = incomingPessoaMovimentacao.TipoMovimentacaoId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ViolaUnique(ex))
        {
            await _context.Entry(existingPessoaMovimentacao).ReloadAsync();
            throw new InvalidOperationException($"Já existe uma pessoa chamada {nome}.");
        }

        return true;
    }

    public async Task<List<PessoaMovimentacao>> GetPessoaMovimentacao(int userId)
    {
        return await _context.PessoaMovimentacao
            .Where(a => a.UserId == userId)
            .OrderBy(c => c.NomePessoa)
            .Include(a => a.Categoria)
            .Include(a => a.TipoMovimentacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<PessoaMovimentacao>> VerificaPossuiPessoa(string? nomePessoa, int userId)
    {
        var nome = Normalizar(nomePessoa);
        if (nome == null)
            return [];

        try
        {
            return await _context.PessoaMovimentacao
                .Where(a => a.UserId == userId && a.NomePessoa == nome)
                .OrderBy(c => c.NomePessoa)
                .Include(a => a.Categoria)
                .Include(a => a.TipoMovimentacao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // veja ex.Message e ex.InnerException
            throw;
        }
    }

}
