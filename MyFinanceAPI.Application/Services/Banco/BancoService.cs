using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class BancoService : IBancoService
{
    private readonly IBancoRepository _bancoRepository;
    private readonly IMapper _mapper;

    public BancoService(IBancoRepository BancoRepository, IMapper mapper)
    {
        _bancoRepository = BancoRepository;
        _mapper = mapper;
    }
    public async Task Add(BancoDTO BancoDTO, int userId)
    {
        var Banco = _mapper.Map<Banco>(BancoDTO);
        Banco.UserId = userId;
        await _bancoRepository.Create(Banco);
    }

    public async Task<IEnumerable<BancoDTO>> GetBanco(int userId)
    {
        var movimentacoDiaria = await _bancoRepository.GetBancos(userId);
        return _mapper.Map<IEnumerable<BancoDTO>>(movimentacoDiaria);
    }

    public async Task<BancoDTO> GetBancoById(int id, int userId)
    {
        var Banco = await _bancoRepository.GetBancoById(id, userId);
        return _mapper.Map<BancoDTO>(Banco);
    }

    public async Task Remove(int id, int userId)
    {
        await _bancoRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(BancoDTO dto, int userId)
    {   
        var banco = _mapper.Map<Banco>(dto);
        await _bancoRepository.UpdateAsync(banco, userId);
        return true;
    }
  
    public async Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId)
    {   
        await _bancoRepository.UpdateSaldo(bancoId, saldoAtual , userId);
        return true;
    }

}
