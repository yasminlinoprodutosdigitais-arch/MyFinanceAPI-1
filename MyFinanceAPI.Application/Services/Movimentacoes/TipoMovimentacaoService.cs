using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TipoMovimentacaoService : ITipoMovimentacaoService
{
    private readonly ITipoMovimentacaoRepository _tipoMovimentacaoRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public TipoMovimentacaoService(ITipoMovimentacaoRepository TipoMovimentacaoRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _tipoMovimentacaoRepository = TipoMovimentacaoRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    public async Task Add(TipoMovimentacaoDTO TipoMovimentacaoDTO, int userId)
    {
        var TipoMovimentacao = _mapper.Map<TipoMovimentacao>(TipoMovimentacaoDTO);
        TipoMovimentacao.UserId = userId;
        await _tipoMovimentacaoRepository.Create(TipoMovimentacao);
    }

    public async Task<IEnumerable<TipoMovimentacaoDTO>> GetTipoMovimentacao(int userId)
    {
        var movimentacoDiaria = await _tipoMovimentacaoRepository.GetTipoMovimentacao(userId);
        return _mapper.Map<IEnumerable<TipoMovimentacaoDTO>>(movimentacoDiaria);
    }

    public async Task<TipoMovimentacaoDTO> GetTipoMovimentacaoById(int id, int userId)
    {
        var TipoMovimentacao = await _tipoMovimentacaoRepository.GetTipoMovimentacaoById(id, userId);
        return _mapper.Map<TipoMovimentacaoDTO>(TipoMovimentacao);
    }

    public async Task Remove(int id, int userId)
    {
        await _tipoMovimentacaoRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(TipoMovimentacaoDTO dto, int userId)
    {   
        var account = _mapper.Map<Account>(dto);
        await _accountRepository.Update(account, userId);
        return true;
    }

}
