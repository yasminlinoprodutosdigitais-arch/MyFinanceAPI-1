using System;
using System.Transactions;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class MonthlyUpdateService : IMonthlyUpdateService
{
    private readonly IMonthlyUpdateRepository _monthlyUpdateRepository;
    private readonly IMapper _mapper;

    public MonthlyUpdateService(IMonthlyUpdateRepository monthlyUpdateRepository, IMapper mapper)
    {
        _monthlyUpdateRepository = monthlyUpdateRepository;
        _mapper = mapper;
    }

    public async Task Add(MonthlyUpdateDTO MonthlyUpdateDTO)
    {
        var update = _mapper.Map<MonthlyUpdate>(MonthlyUpdateDTO);
        await _monthlyUpdateRepository.Create(update);
    }

    public async Task Delete(int id)
    {
        await _monthlyUpdateRepository.Remove(id);
    }

    public async Task<IEnumerable<AccountGroupingDTO>> GetMonthlyUpdate()
    {
        var monthlyUpdate = await _monthlyUpdateRepository.GetMonthlyUpdate();
        return _mapper.Map<IEnumerable<AccountGroupingDTO>>(monthlyUpdate);
    }

    public async Task<IEnumerable<MonthlyUpdateDTO>> GetMonthlyUpdateByCategory(int categoryId)
    {
        var MonthlyUpdate = await _monthlyUpdateRepository.GetMonthlyUpdateByCategory(categoryId);
        return _mapper.Map<IEnumerable<MonthlyUpdateDTO>>(MonthlyUpdate);
    }

    public async Task<MonthlyUpdateDTO> GetMonthlyUpdateById(int id)
    {
        var update = await _monthlyUpdateRepository.GetMonthlyUpdateById(id);
        return _mapper.Map<MonthlyUpdateDTO>(update);
    }

    public async Task<IEnumerable<MonthlyUpdateDTO>> GetMonthlyUpdateByDate(DateTime date)
    {
        var MonthlyUpdate = await _monthlyUpdateRepository.GetAccountByDate(date);
        return _mapper.Map<IEnumerable<MonthlyUpdateDTO>>(MonthlyUpdate);
    }

    public async Task Update(MonthlyUpdateDTO MonthlyUpdateDTO)
    {
        var update = _mapper.Map<MonthlyUpdate>(MonthlyUpdateDTO);
        await _monthlyUpdateRepository.Update(update);
    }
}
