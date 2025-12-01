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
        Task<IEnumerable<ExtratoBancarioItemDTO>> GetByExtratoAsync(int extratoId, int userId);

        Task<ExtratoBancarioItemDTO> AddAsync(ExtratoBancarioItemDTO dto, int userId);
        Task UpdateAsync(ExtratoBancarioItemDTO dto, int userId);
        Task RemoveAsync(int id, int userId);
        Task<IEnumerable<ExtratoBancarioItemDTO>> GetByMonthAsync(int userId, int year, int month);

    }
}
