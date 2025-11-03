using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class MovimentacaoDiariaService : IMovimentacaoDiariaService
{
    private readonly IMovimentacaoDiariaRepository _movimentacaoDiariaRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public MovimentacaoDiariaService(IMovimentacaoDiariaRepository MovimentacaoDiariaRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _movimentacaoDiariaRepository = MovimentacaoDiariaRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    public async Task Add(MovimentacaoDiariaDTO MovimentacaoDiariaDTO, int userId)
    {
        var MovimentacaoDiaria = _mapper.Map<MovimentacaoDiaria>(MovimentacaoDiariaDTO);
        MovimentacaoDiaria.UserId = userId;
        await _movimentacaoDiariaRepository.Create(MovimentacaoDiaria);
    }

    public async Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoDiaria(int userId)
    {
        var movimentacoDiaria = await _movimentacaoDiariaRepository.GetMovimentacaoDiaria(userId);
        return _mapper.Map<IEnumerable<MovimentacaoDiariaDTO>>(movimentacoDiaria);
    }

    public async Task<MovimentacaoDiariaDTO> GetMovimentacaoDiariaById(int id, int userId)
    {
        var MovimentacaoDiaria = await _movimentacaoDiariaRepository.GetMovimentacaoDiariaById(id, userId);
        return _mapper.Map<MovimentacaoDiariaDTO>(MovimentacaoDiaria);
    }

    
    public async Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoByDate(DateTime date, int userId)
    {
        var Transaction = await _movimentacaoDiariaRepository.GetMovimentacaoByDate(date, userId);
        return _mapper.Map<IEnumerable<MovimentacaoDiariaDTO>>(Transaction);
    }

    public async Task Remove(int id, int userId)
    {
        await _movimentacaoDiariaRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(MovimentacaoDiariaDTO dto, int userId)
    {   
        var account = _mapper.Map<MovimentacaoDiaria>(dto);
        await _movimentacaoDiariaRepository.UpdateAsync(account, userId);
        return true;
    }

}
