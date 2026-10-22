using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ContaRepository(ContextDB context) : IContaRepository
{
    private readonly ContextDB _context = context;
    public async Task<Conta> Create(Conta account)
    {
        var existingCategory = await _context.Categorias
         .Where(c => c.UsuarioId == account.UsuarioId && c.Id == account.CategoriaId).FirstOrDefaultAsync();

        if (existingCategory == null)
            throw new KeyNotFoundException("Categoria não encontrada");

        await _context.Contas.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }
    public async Task CreateContaVencimento(List<ContaVencimento> vencimentos)
    {
        await _context.ContasVencimento.AddRangeAsync(vencimentos);
        await _context.SaveChangesAsync();
    }

    // Local: MyFinanceAPI/MyFinanceAPI.Data/Repositories/AccountRepository.cs
    public async Task Remove(int id, int userId)
    {
        var account = await _context.Contas
            .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == userId);

        if (account != null)
        {
            // Os vencimentos saem junto pelo ON DELETE CASCADE de ContaVencimento_ContaId_fkey (D-9).
            _context.Contas.Remove(account);

            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.Contas.AnyAsync(a => a.Id == id && a.UsuarioId == userId))
            return false;

        return await _context.Transacoes.AnyAsync(t => t.ContaId == id);
    }
    public async Task RemoveContaVencimento(int id, int userId) // O método recebe id e userId
    {
        var account = await _context.Contas
           .FirstOrDefaultAsync(a => a.Id == id && a.UsuarioId == userId);

        if (account != null)
        {
            var vencimentos = await _context.ContasVencimento
                .Where(v => v.ContaId == id)
                .ToListAsync();

            _context.ContasVencimento.RemoveRange(vencimentos);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<Conta> Update(Conta incomingAccount, int userId)
    {
        var existingAccount = await _context.Contas
            .Include(a => a.Categoria)
            .FirstOrDefaultAsync(a => a.Id == incomingAccount.Id && a.UsuarioId == userId);

        if (existingAccount == null)
        {
            throw new NaoEncontradoException("Conta não encontrada.");
        }

        existingAccount.Nome = incomingAccount.Nome;
        existingAccount.Valor = incomingAccount.Valor;
        existingAccount.CategoriaId = incomingAccount.CategoriaId;
        existingAccount.Ativo = incomingAccount.Ativo;
        existingAccount.EhParcelado = incomingAccount.EhParcelado;
        existingAccount.QuantidadeParcelas = incomingAccount.QuantidadeParcelas;
        existingAccount.ParcelaAtual = incomingAccount.ParcelaAtual;

        await _context.SaveChangesAsync();

        return existingAccount;
    }
    public async Task<IEnumerable<Conta>> GetAccountsByCategory(int categoryId, int userId)
    {
        return await _context.Contas.AsNoTracking()
            .Where(a => a.UsuarioId == userId && a.CategoriaId == categoryId)
            .ToListAsync();
    }

    public async Task<Conta?> GetAccountById(int id, int userId)
    {
        var account = await _context.Contas
            .Where(c => c.UsuarioId == userId && c.Id == id)
            .Include(c => c.Vencimentos)
            .FirstOrDefaultAsync();
        return account;
    }

    public async Task<List<Conta>>? GetAccounts(int userId)
    {
        var accounts = await _context.Contas.AsNoTracking()
            .Where(a => a.UsuarioId == userId)
            .Include(a => a.Categoria)  // Incluindo a categoria nas contas
            .Include(a => a.Vencimentos)  // Incluindo a categoria nas contas
            .Select(t => new Conta
            {
                Id = t.Id,
                Nome = t.Nome,
                Valor = t.Valor,
                CategoriaId = t.CategoriaId,
                CategoriaNome = t.Categoria.Nome,
                Ativo = t.Ativo,
                EhParcelado = t.EhParcelado,
                ParcelaAtual = t.ParcelaAtual,
                QuantidadeParcelas = t.QuantidadeParcelas,
                Vencimentos = t.Vencimentos
            })
            .OrderBy(c => c.Nome)
            .ToListAsync();

        return accounts;
    }

    public async Task<List<Conta>>? GetContasAtivas(int userId)
    {
        var accounts = await _context.Contas.AsNoTracking()
            .Where(a => a.UsuarioId == userId && a.Ativo == StatusAtivo.Ativo)
            .Include(a => a.Categoria)  // Incluindo a categoria nas contas
            .Include(a => a.Vencimentos)  // Incluindo a categoria nas contas
            .OrderBy(c => c.Nome)
            .ToListAsync();

        return accounts;
    }

}
