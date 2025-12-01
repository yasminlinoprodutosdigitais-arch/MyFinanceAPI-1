using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces.Repositories
{
    public interface IExtratoBancarioRepository
    {
        // ----- CREATE / UPDATE / DELETE -----

        Task<ExtratoBancario> CreateAsync(ExtratoBancario extrato);
        Task UpdateAsync(ExtratoBancario extrato);
        Task RemoveAsync(int id);                  // usado pelo service
        Task DeleteAsync(int id, int userId);      // versão com segurança por usuário

        // ----- GET -----

        // usado pelo service (sem userId)
        Task<ExtratoBancario?> GetByIdAsync(int id);

        // versão com filtro por usuário (opcional)
        Task<ExtratoBancario?> GetByIdAsync(int id, int userId);

        // o service está chamando GetByUserIdAsync
        Task<IEnumerable<ExtratoBancario>> GetByUserIdAsync(int userId);

        // equivalente (mantido para reutilização futura)
        Task<IEnumerable<ExtratoBancario>> GetByUserAsync(int userId);

        // Buscar extratos por período (opcional para tela)
        Task<IEnumerable<ExtratoBancario>> GetByPeriodoAsync(
            int userId,
            DateOnly? dataInicio,
            DateOnly? dataFim
        );
    }
}
