using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class BancoRepository(ContextDB context) : RepositorioDoUsuario<Banco>(context), IBancoRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Banco não encontrado.";

    protected override Expression<Func<Banco, bool>> Proprios(int userId) => b => b.UsuarioId == userId;

    protected override IQueryable<Banco> ComDetalhes(IQueryable<Banco> query) => query.Include(b => b.TipoCartao);

    protected override IOrderedQueryable<Banco> Ordenar(IQueryable<Banco> query) => query.OrderBy(b => b.NomeBanco);

    protected override void CopiarAlteracoes(Banco existente, Banco entrada)
    {
        existente.NomeBanco = entrada.NomeBanco;
        existente.Agencia = entrada.Agencia;
        existente.NumeroConta = entrada.NumeroConta;
        existente.TipoCartaoId = entrada.TipoCartaoId;
        existente.SaldoInicial = entrada.SaldoInicial;
        existente.Ativo = entrada.Ativo;
    }

    public async Task<bool> ExisteNomeAsync(int userId, string nome, int ignorarId)
    {
        var alvo = (nome ?? string.Empty).Trim().ToUpper();
        return await _context.Bancos.AnyAsync(b => b.UsuarioId == userId && b.Id != ignorarId
            && b.NomeBanco.Trim().ToUpper() == alvo);
    }

    public async Task<bool> EmUsoAsync(int id, int userId)
    {
        if (!await _context.Bancos.AnyAsync(b => b.Id == id && b.UsuarioId == userId))
            return false;

        return await _context.MovimentacoesDiarias.AnyAsync(m => m.BancoId == id)
            || await _context.ExtratosBancarios.AnyAsync(e => e.BancoId == id)
            || await _context.ItensExtratoBancario.AnyAsync(i => i.BancoId == id);
    }

    public async Task<bool> UpdateSaldo(int bancoId, decimal SaldoAtual, int userId)
    {
        var existingBanco = await ObterProprioAsync(bancoId, userId)
            ?? throw new NaoEncontradoException(MensagemNaoEncontrado);

        existingBanco.SaldoInicial = SaldoAtual;
        await _context.SaveChangesAsync();

        return true;
    }
}
