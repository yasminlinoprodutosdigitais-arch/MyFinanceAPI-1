using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces
{
    public interface IExtratoBancarioRepository
    {
        // ----- CREATE / UPDATE / DELETE -----

        Task<ExtratoBancario> CreateAsync(ExtratoBancario extrato);
        Task UpdateAsync(ExtratoBancario extrato);
        Task DeleteAsync(int id, int userId);      // só remove se o extrato for do usuário (BE-P2-04)

        // ----- GET -----

        // Sempre filtra pelo dono (BE-P0-06) — não existe mais a sobrecarga só com id.
        Task<ExtratoBancario?> GetByIdAsync(int id, int userId);

        // o service está chamando GetByUserIdAsync
        Task<IEnumerable<ExtratoBancario>> GetByUserIdAsync(int userId, string month);

        // Buscar extratos por período (opcional para tela)
        Task<IEnumerable<ExtratoBancario>> GetByPeriodoAsync(
            int userId,
            DateOnly? dataInicio,
            DateOnly? dataFim
        );
    }
}
