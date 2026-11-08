// MyFinanceAPI.Application/Interfaces/IExtratoBancarioItemService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces
{
    public interface IExtratoBancarioItemService
    {
        Task<ExtratoBancarioItemDTO?> GetByIdAsync(int id, int userId);

        Task<ExtratoBancarioItemDTO> AddAsync(ExtratoBancarioItemDTO dto, int userId);
        Task UpdateAsync(ExtratoBancarioItemDTO dto, int userId);
        /// <summary>Divide o lançamento nestas partes (a soma tem que bater); lista vazia desfaz (card #76, fase 3).</summary>
        Task DividirAsync(int id, IReadOnlyList<ExtratoItemParteDTO> partes, int userId);
        Task RemoveAsync(int id, int userId);
        Task<IEnumerable<ExtratoBancarioItemDTO>> GetByMonthAsync(int userId, int year, int month, int? bancoId = null);

    }
}
