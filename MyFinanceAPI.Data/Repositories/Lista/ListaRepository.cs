using System.Linq.Expressions;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Data.Repositories;

public class ListaRepository(ContextDB context) : RepositorioDoUsuario<Lista>(context), IListaRepository
{
    protected override string MensagemNaoEncontrado => "Lista não encontrada.";

    protected override Expression<Func<Lista, bool>> Proprios(int userId) => l => l.UsuarioId == userId;

    protected override IOrderedQueryable<Lista> Ordenar(IQueryable<Lista> query) => query.OrderBy(l => l.Nome);

    protected override void CopiarAlteracoes(Lista existente, Lista entrada)
    {
        existente.Nome = entrada.Nome;
        existente.TipoLista = entrada.TipoLista;
        existente.Ativo = entrada.Ativo;
    }
}
