using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TipoCartaoService : ITipoCartaoService
{
    private readonly ITipoCartaoRepository _tipoCartaoRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public TipoCartaoService(ITipoCartaoRepository TipoCartaoRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _tipoCartaoRepository = TipoCartaoRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    public async Task Add(TipoCartaoDTO TipoCartaoDTO, int userId)
    {
        var TipoCartao = _mapper.Map<TipoCartao>(TipoCartaoDTO);
        TipoCartao.UserId = userId;
        await _tipoCartaoRepository.Create(TipoCartao);
    }

    public async Task<IEnumerable<TipoCartaoDTO>> GetTipoCartao(int userId)
    {
        var movimentacoDiaria = await _tipoCartaoRepository.GetTipoCartao(userId);
        return _mapper.Map<IEnumerable<TipoCartaoDTO>>(movimentacoDiaria);
    }

    public async Task<TipoCartaoDTO> GetTipoCartaoById(int id, int userId)
    {
        var TipoCartao = await _tipoCartaoRepository.GetTipoCartaoById(id, userId);
        return _mapper.Map<TipoCartaoDTO>(TipoCartao);
    }

    public async Task Remove(int id, int userId)
    {
        await _tipoCartaoRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(TipoCartaoDTO dto, int userId)
    {   
        var account = _mapper.Map<Account>(dto);
        await _accountRepository.Update(account, userId);
        return true;
    }

}
