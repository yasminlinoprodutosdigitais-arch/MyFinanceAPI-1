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

    protected override IQueryable<Banco> ComDetalhes(IQueryable<Banco> query) =>
        query.Include(b => b.TipoCartao).Include(b => b.Pessoas).ThenInclude(p => p.PessoaMovimentacao);

    protected override IOrderedQueryable<Banco> Ordenar(IQueryable<Banco> query) => query.OrderBy(b => b.NomeBanco);

    protected override void CopiarAlteracoes(Banco existente, Banco entrada)
    {
        existente.NomeBanco = entrada.NomeBanco;
        existente.Agencia = entrada.Agencia;
        existente.NumeroConta = entrada.NumeroConta;
        existente.TipoCartaoId = entrada.TipoCartaoId;
        existente.SaldoInicial = entrada.SaldoInicial;
        existente.DiaVencimento = entrada.DiaVencimento;
        existente.DiaFechamento = entrada.DiaFechamento;
        existente.Compartilhado = entrada.Compartilhado;
        existente.ConviteCompartilharDispensado = entrada.ConviteCompartilharDispensado;
        existente.Ativo = entrada.Ativo;
        SincronizarPessoas(existente.Id, entrada.Pessoas.Select(p => p.PessoaMovimentacaoId));
    }

    /// <summary>Deixa o cartão exatamente com estas pessoas (card #77).</summary>
    private void SincronizarPessoas(int bancoId, IEnumerable<int> pessoasIds)
    {
        var desejadas = pessoasIds.ToHashSet();
        var atuais = _context.BancosPessoas.Where(p => p.BancoId == bancoId).ToList();
        _context.BancosPessoas.RemoveRange(atuais.Where(p => !desejadas.Contains(p.PessoaMovimentacaoId)));
        _context.BancosPessoas.AddRange(desejadas.Except(atuais.Select(p => p.PessoaMovimentacaoId))
            .Select(id => new BancoPessoa { BancoId = bancoId, PessoaMovimentacaoId = id }));
    }

    public async Task<bool> PessoasValidasAsync(int userId, IReadOnlyCollection<int> pessoasIds)
    {
        var ids = pessoasIds.Distinct().ToList();
        return await _context.PessoasMovimentacao.CountAsync(p => ids.Contains(p.Id) && p.UsuarioId == userId && !p.EhTitular) == ids.Count;
    }

    public async Task DefinirCompartilhamentoAsync(int id, int userId, bool? compartilhado, IReadOnlyCollection<int>? pessoasIds, bool? conviteDispensado)
    {
        var existente = await ObterProprioAsync(id, userId) ?? throw new NaoEncontradoException(MensagemNaoEncontrado);
        if (compartilhado is bool c)
            existente.Compartilhado = c;
        if (conviteDispensado is bool d)
            existente.ConviteCompartilharDispensado = d;
        if (pessoasIds != null)
            SincronizarPessoas(id, pessoasIds);
        await _context.SaveChangesAsync();
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
