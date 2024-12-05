using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Data.Configuration;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class MonthlyUpdateRepository(ContextDB context) : IMonthlyUpdateRepository
{
    private readonly ContextDB _context = context;

    public async Task<MonthlyUpdate> Create(MonthlyUpdate monthlyUpdate)
    {
        await _context.MonthlyUpdates.AddAsync(monthlyUpdate);
        await _context.SaveChangesAsync();
        return monthlyUpdate;
    }

    public Task<IEnumerable<MonthlyUpdate>> GetAccountByDate(DateTime dateTime)
    {
        throw new NotImplementedException();
    }
    public async Task<List<AccountGrouping>> GetMonthlyUpdate()
    {
        var accounts = await _context.Accounts
            .Include(a => a.Category)             // Inclui a Categoria
            .Include(a => a.MonthlyUpdates)       // Inclui os MonthlyUpdates associados
            .OrderBy(a => a.Category.Name)        // Ordena pelas categorias
            .ToListAsync();                       // Carrega todos os dados necessários

        var groupedAccounts = accounts
            .GroupBy(a => a.Category)  // Agrupar as contas por categoria
            .Select(group => new AccountGrouping
            (
                group.Key.Id,          // ID da categoria
                group.Key.Name,        // Nome da categoria
                group.Key.SubCategory, // Subcategoria da categoria
                group.Select(a => new Account
                {
                    Id = a.Id,
                    Name = a.Name,
                    Value = a.Value,
                    MonthlyUpdates = a.MonthlyUpdates.Select(m => new MonthlyUpdate
                    {
                        Id = m.Id,
                        Date = m.Date,
                        Name = m.Name,
                        IdAccount = m.IdAccount,
                        Value = m.Value,
                        Status = m.Status
                    }).ToList() // Para cada conta, adicionar os detalhes de todos os MonthlyUpdates
                }).ToList() // Lista de contas agrupadas por categoria
            ))
            .ToList();  // Realiza a execução e retorna a lista agrupada

        return groupedAccounts;
    }

    public Task<IEnumerable<MonthlyUpdate>> GetMonthlyUpdateByCategory(int categoryid)
    {
        throw new NotImplementedException();
    }

    public async Task<MonthlyUpdate> GetMonthlyUpdateById(int id)
    {
        return await _context.MonthlyUpdates.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<MonthlyUpdate> Remove(int id)
    {
        var update = await GetMonthlyUpdateById(id);
        if (update != null)
        {
            _context.MonthlyUpdates.Remove(update);
            _context.SaveChanges();
        }

        return update;
    }

    public async Task<MonthlyUpdate> Update(MonthlyUpdate monthlyUpdate)
    {
        // Primeiro, obtenha o MonthlyUpdate existente pelo ID
        var existingMonthlyUpdate = await _context.MonthlyUpdates
            .FirstOrDefaultAsync(m => m.Id == monthlyUpdate.Id);

        // Se não encontrar, lança uma exceção
        if (existingMonthlyUpdate == null)
        {
            throw new Exception("Update not found");
        }

        // Atualize as propriedades desejadas
        existingMonthlyUpdate.Date = monthlyUpdate.Date;
        existingMonthlyUpdate.Name = monthlyUpdate.Name;
        existingMonthlyUpdate.Value = monthlyUpdate.Value;
        existingMonthlyUpdate.Status = monthlyUpdate.Status;
        existingMonthlyUpdate.IdAccount = monthlyUpdate.IdAccount;  // Certifique-se de atualizar todas as propriedades necessárias

        // Salve as mudanças no banco de dados
        await _context.SaveChangesAsync();

        return existingMonthlyUpdate;
    }


    // public async Task<IEnumerable<MonthlyUpdate>> GetAccountByDate(DateTime dateTime)
    // {
    //     var dateMounth = dateTime.Month;
    //     var dateYear = dateTime.Year;

    //     var history = await _context
    //                     .Find(t => t.Date.Month == dateMounth
    //                         && t.Date.Year == dateYear)
    //                     .ToListAsync();

    //     return history;

    // }

    // public async Task<IEnumerable<MonthlyUpdate>> GetMonthlyUpdate()
    // {
    //     return await _context.Find(h => true).ToListAsync();
    // }

    // public async Task<IEnumerable<MonthlyUpdate>> GetMonthlyUpdateByCategory(int idCategory)
    // {
    //     return await _context.Find(c => c.IdCategory == idCategory).ToListAsync();

    // }

    // public async Task<MonthlyUpdate> GetMonthlyUpdateById(int id)
    // {
    //     return await _context.Find(t => t.Id == id).FirstOrDefaultAsync();
    // }

    // public async Task<MonthlyUpdate> Remove(int id)
    // {
    //     var account = await _context.FindOneAndDeleteAsync(c => c.Id == id);
    //     return account;
    // }

    // public async Task<MonthlyUpdate> Update(MonthlyUpdate monthlyUpdate)
    // {
    //     await _context.ReplaceOneAsync(t => t.Id == monthlyUpdate.Id, monthlyUpdate);
    //     return monthlyUpdate;
    // }
}
