using System;
using System.Transactions;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IMonthlyUpdateService
{
    Task<IEnumerable<AccountGroupingDTO>> GetMonthlyUpdate();
    Task<IEnumerable<MonthlyUpdateDTO>> GetMonthlyUpdateByCategory(int categoryId);
    Task<MonthlyUpdateDTO> GetMonthlyUpdateById(int id);
    Task<IEnumerable<MonthlyUpdateDTO>> GetMonthlyUpdateByDate(DateTime date);

    Task Add(MonthlyUpdateDTO monthlyUpdateDTO);
    Task Update(MonthlyUpdateDTO monthlyUpdateDTO);
    Task Delete(int id);
}
