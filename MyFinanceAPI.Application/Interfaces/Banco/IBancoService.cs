using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IBancoService : IServicoCadastro<BancoDTO>
{
    Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId);
}
