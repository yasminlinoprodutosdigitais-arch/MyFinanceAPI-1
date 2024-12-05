using System;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IMonthlyUpdateRepository
{
    Task<List<AccountGrouping>> GetMonthlyUpdate();
    Task<IEnumerable<MonthlyUpdate>> GetAccountByDate(DateTime dateTime);
    Task<MonthlyUpdate> GetMonthlyUpdateById(int id);
    Task<IEnumerable<MonthlyUpdate>> GetMonthlyUpdateByCategory(int categoryid);
    Task<MonthlyUpdate> Create(MonthlyUpdate monthlyUpdate);
    Task<MonthlyUpdate> Update(MonthlyUpdate monthlyUpdate);
    Task<MonthlyUpdate> Remove(int id);
}
