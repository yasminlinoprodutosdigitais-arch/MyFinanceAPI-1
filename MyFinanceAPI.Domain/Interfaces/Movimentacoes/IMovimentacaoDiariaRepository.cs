using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IMovimentacaoDiariaRepository : IRepositorioDoUsuario<MovimentacaoDiaria>
{
    Task<IEnumerable<MovimentacaoDiaria>> GetMovimentacaoByDate(DateTime date, int userId);
}
