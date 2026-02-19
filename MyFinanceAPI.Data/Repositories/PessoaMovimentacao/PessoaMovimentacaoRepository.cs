using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class PessoaMovimentacaoRepository(ContextDB context) : IPessoaMovimentacaoRepository
{
    private readonly ContextDB _context = context;

    public async Task<PessoaMovimentacao> Create(PessoaMovimentacao PessoaMovimentacao, int userId)
    {
        try
        {
            PessoaMovimentacao.UserId = userId;
            await _context.PessoaMovimentacao.AddAsync(PessoaMovimentacao);
            await _context.SaveChangesAsync();
            return PessoaMovimentacao;
        }
        catch (Exception ex)
        {
            // veja ex.Message e ex.InnerException
            throw;
        }
    }

    public async Task<IEnumerable<PessoaMovimentacao>> GetPessoaMovimentacaoByUserId(int userId)
    {
        var PessoaMovimentacao = await _context.PessoaMovimentacao.Where(c => c.UserId == userId).OrderBy(c => c.NomePessoa).ToListAsync();
        return PessoaMovimentacao;
    }

    public async Task<PessoaMovimentacao?> GetPessoaMovimentacaoById(int id, int userId)
    {
        var PessoaMovimentacao = await _context.PessoaMovimentacao
            .Where(c => c.UserId == userId && c.Id == id)
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

    public async Task<PessoaMovimentacao?> FindByIdForUserAsync(int id, int userId)
    {
        return await _context.PessoaMovimentacao
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<bool> UpdateAsync(PessoaMovimentacao incomingPessoaMovimentacao, int userId)
    {
        try
        {
            var existingPessoaMovimentacao = await _context.PessoaMovimentacao
                .FirstOrDefaultAsync(a => a.Id == incomingPessoaMovimentacao.Id && a.UserId == userId);

            if (existingPessoaMovimentacao == null)
            {
                throw new Exception("Movimentação não encontrada ou não pertence ao usuário.");
            }

            existingPessoaMovimentacao.NomePessoa = incomingPessoaMovimentacao.NomePessoa;
            existingPessoaMovimentacao.CategoriaId = incomingPessoaMovimentacao.CategoriaId;
            existingPessoaMovimentacao.TipoMovimentacaoId = incomingPessoaMovimentacao.TipoMovimentacaoId;

            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex.Message, ex.StackTrace, etc.)
            throw; // Rethrow or handle as needed
        }
    }

    public async Task<List<PessoaMovimentacao>> GetPessoaMovimentacao(int userId)
    {
        return await _context.PessoaMovimentacao
            .Where(a => a.UserId == userId)
            .OrderBy(c => c.NomePessoa)
            .ToListAsync();
    }

    public async Task<IEnumerable<PessoaMovimentacao>> VerificaPossuiPessoa(string nomePessoa, int userId)
    {
        try
        {
            return await _context.PessoaMovimentacao
                .Where(a => a.UserId == userId && a.NomePessoa == nomePessoa)
                .OrderBy(c => c.NomePessoa)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // veja ex.Message e ex.InnerException
            throw;
        }
    }

}
