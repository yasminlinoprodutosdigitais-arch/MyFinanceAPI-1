using System;
using System.Net.Http.Headers;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Interfaces;

public interface IContaService
{
    Task<ContaDTO> Add(ContaDTO accountDTO, int userId);
    Task Update(ContaDTO accountDTO, int userId);
    Task UpdateParcela(ContaDTO accountDTO, int userId);
    Task Remove(int id, int userId);
    Task<IEnumerable<ContaDTO>> GetAccounts(int userId);
    Task<IEnumerable<ContaDTO>> GetContasAtivas(int userId);
    Task<ContaDTO> GetAccountById(int id, int userId);
}
