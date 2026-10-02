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

        // Sempre filtra pelo dono (BE-P0-06) — não existe mais a sobrecarga só com id.
        Task<ExtratoBancario?> GetByIdAsync(int id, int userId);

        // o service está chamando GetByUserIdAsync
        Task<IEnumerable<ExtratoBancario>> GetByUserIdAsync(int userId, string month);

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
