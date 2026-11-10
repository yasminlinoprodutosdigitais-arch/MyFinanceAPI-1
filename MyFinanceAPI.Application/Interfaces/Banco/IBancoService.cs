using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IBancoService : IServicoCadastro<BancoDTO>
{
    Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId);
    /// <summary>Switch "Outras pessoas usam este cartão" do detalhe (card #77).</summary>
    Task DefinirCompartilhamentoAsync(int id, int userId, MyFinanceAPI.Application.DTO.Movimentacoes.CompartilhamentoDTO dto);
}
