using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class MovimentacaoDiariaRepository(ContextDB context) : RepositorioDoUsuario<MovimentacaoDiaria>(context), IMovimentacaoDiariaRepository
{
    private readonly ContextDB _context = context;

    protected override string MensagemNaoEncontrado => "Movimentação diária não encontrada.";

    protected override Expression<Func<MovimentacaoDiaria, bool>> Proprios(int userId) => m => m.UserId == userId;

    protected override IOrderedQueryable<MovimentacaoDiaria> Ordenar(IQueryable<MovimentacaoDiaria> query) =>
        query.OrderBy(m => m.DataMovimentacao);

    // Só a listagem trazia as navegações; o ObterPorId continua sem includes, como antes.
    public override async Task<List<MovimentacaoDiaria>> ListarAsync(int userId) =>
        await Ordenar(ComNavegacoes(Tabela.Where(Proprios(userId)))).ToListAsync();

    // Identificador fica de fora: é a chave da importação do extrato.
    protected override void CopiarAlteracoes(MovimentacaoDiaria existente, MovimentacaoDiaria entrada)
    {
        existente.BancoId = entrada.BancoId;
        existente.TipoCartaoId = entrada.TipoCartaoId;
        existente.TipoMovimentacaoId = entrada.TipoMovimentacaoId;
        existente.Valor = entrada.Valor;
        existente.TipoLancamento = entrada.TipoLancamento;
        existente.Descricao = entrada.Descricao;
        existente.NomePessoaTransacao = entrada.NomePessoaTransacao;
        existente.DataMovimentacao = entrada.DataMovimentacao;
    }

    private static IQueryable<MovimentacaoDiaria> ComNavegacoes(IQueryable<MovimentacaoDiaria> query) =>
        query.Include(m => m.Banco).Include(m => m.TipoCartao).Include(m => m.TipoMovimentacao);

    public async Task<IEnumerable<MovimentacaoDiaria>> GetMovimentacaoByDate(DateTime dateTime, int userId)
    {
        return await ComNavegacoes(_context.MovimentacaoDiaria)
            .Where(c => c.UserId == userId && c.DataMovimentacao.Month == dateTime.Month && c.DataMovimentacao.Year == dateTime.Year)
            .ToListAsync();
    }
}
