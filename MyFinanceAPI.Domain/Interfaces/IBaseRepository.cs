using System;
using System.Linq.Expressions;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> BuscarTodos();
    TEntity BuscarPorId(int codigoId);
    IEnumerable<TEntity> BuscarPorExpressao(Expression<Func<TEntity, bool>> filtro);
    void Cadastrar(TEntity entidade);
    void Atualizar(TEntity entidade, int codigoId);
    void AlterarStatusAtivo(int codigoId, bool statusAtivo);
    void Deletar(int codigoId);
}

