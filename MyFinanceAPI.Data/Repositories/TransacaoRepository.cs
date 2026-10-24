using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class TransacaoRepository(ContextDB context) : ITransacaoRepository
{
    private readonly ContextDB _context = context;

    // A data é de calendário (date): grava como veio. A conversão pelo fuso do servidor que havia
    // aqui gravava meia-noite UTC com a API hospedada em UTC — o dia aparecia um antes (DB-P1-02).
    public async Task Create(List<Transacao> transactions)
    {
        await _context.Transacoes.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transacao>> GetTransactionByDate(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var dia = DateOnly.FromDateTime(dateTime);
        var a = await _context.Transacoes.AsNoTracking()
            .Where(TransacaoFiltros.DoDiaOuDoMes(userId, dia, pesquisaDataCompleta))
            .ToListAsync();
        return a;
    }

    public async Task<IEnumerable<Transacao>> GetContaVencida(DateTime hoje, int userId)
    {
        var dataHoje = DateOnly.FromDateTime(hoje);
        var a = await _context.Transacoes.AsNoTracking()
            .Where(c => c.UsuarioId == userId
                && c.Data < dataHoje
                && c.Situacao != StatusTransacao.PagoNoPrazo
                && c.Situacao != StatusTransacao.PagoAtrasado)
            .ToListAsync();

        return a;
    }

    public async Task<List<Transacao>> BuscarContasMensais(DateTime dateTime, int userId, bool pesquisaDataCompleta = false)
    {
        var dia = DateOnly.FromDateTime(dateTime);

        // 1) Carrega transações do mês (com ou sem conta)
        var transactions = await _context.Transacoes
            .Where(TransacaoFiltros.DoDiaOuDoMes(userId, dia, pesquisaDataCompleta))
            .Select(t => new Transacao
            {
                Id = t.Id,
                Data = t.Data,
                Nome = t.Nome,
                Valor = t.Valor,
                Situacao = t.Situacao,

                CategoriaId = t.CategoriaId,
                // Projeção no SQL: transação sem categoria ou sem conta traz null aqui, não exceção.
                CategoriaNome = t.Categoria!.Nome,
                SubcategoriaNome = t.Categoria!.Subcategoria,

                ContaId = t.ContaId,
                ContaNome = t.Conta!.Nome,

                EhParcelado = t.EhParcelado,
                ParcelaAtual = t.ParcelaAtual,
                QuantidadeParcelas = t.QuantidadeParcelas,
                Observacao = t.Observacao,
            })
            .ToListAsync();

        return transactions;
    }

    public async Task<Transacao> GetTransactionById(int id, int userId)
    {
        var transaction = await _context.Transacoes
            .Where(t => t.UsuarioId == userId && t.Id == id)
            .FirstOrDefaultAsync();

        if (transaction == null)
            throw new NaoEncontradoException("Transação não encontrada.");

        return transaction;
    }

    public async Task<List<int>> GetParcelasLancadas(int accountId, int userId)
    {
        return await _context.Transacoes
            .Where(t => t.UsuarioId == userId && t.ContaId == accountId && t.ParcelaAtual != null)
            .Select(t => t.ParcelaAtual!.Value)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<int>> GetParcelasPagas(int contaId, int userId, int exceto)
    {
        return await _context.Transacoes
            .Where(t => t.UsuarioId == userId && t.ContaId == contaId && t.Id != exceto && t.ParcelaAtual != null
                && (t.Situacao == StatusTransacao.PagoNoPrazo || t.Situacao == StatusTransacao.PagoAtrasado))
            .Select(t => t.ParcelaAtual!.Value)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Transacao> Remove(int id, int userId)
    {
        // GetTransactionById já lança NaoEncontradoException; a gravação é assíncrona (BE-P4-15).
        var transacao = await GetTransactionById(id, userId);
        _context.Transacoes.Remove(transacao);
        await _context.SaveChangesAsync();

        return transacao;
    }

    public async Task<Transacao> Update(Transacao transaction, int userId)
    {
        var existingTransaction = await _context.Transacoes
            .FirstOrDefaultAsync(t => t.UsuarioId == userId && t.Id == transaction.Id) ?? throw new NaoEncontradoException("Transação não encontrada.");

        existingTransaction.Data = transaction.Data;
        existingTransaction.Nome = transaction.Nome;
        existingTransaction.Valor = transaction.Valor;
        existingTransaction.Situacao = transaction.Situacao;
        existingTransaction.EhParcelado = transaction.EhParcelado;
        existingTransaction.ParcelaAtual = transaction.ParcelaAtual;
        existingTransaction.CategoriaId = transaction.CategoriaId;
        existingTransaction.QuantidadeParcelas = transaction.QuantidadeParcelas;
        existingTransaction.Observacao = transaction.Observacao;

        await _context.SaveChangesAsync();

        return existingTransaction;
    }
}
