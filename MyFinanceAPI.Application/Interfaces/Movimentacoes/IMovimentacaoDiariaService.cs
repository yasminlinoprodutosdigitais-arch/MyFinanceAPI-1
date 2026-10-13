using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Application.Interfaces;

public interface IMovimentacaoDiariaService : IServicoCadastro<MovimentacaoDiariaDTO>
{
    Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoByDate(DateTime date, int userId);
}
