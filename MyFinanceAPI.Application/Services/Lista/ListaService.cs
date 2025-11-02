using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class ListaService : IListaService
{
    private readonly IListaRepository _listaRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public ListaService(IListaRepository ListaRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _listaRepository = ListaRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    public async Task Add(ListaDTO ListaDTO, int userId)
    {
        var Lista = _mapper.Map<Lista>(ListaDTO);
        Lista.UserId = userId;
        await _listaRepository.Create(Lista);
    }

    public async Task<IEnumerable<ListaDTO>> GetLista(int userId)
    {
        var movimentacoDiaria = await _listaRepository.GetListas(userId);
        return _mapper.Map<IEnumerable<ListaDTO>>(movimentacoDiaria);
    }

    public async Task<ListaDTO> GetListaById(int id, int userId)
    {
        var Lista = await _listaRepository.GetListaById(id, userId);
        return _mapper.Map<ListaDTO>(Lista);
    }

    public async Task Remove(int id, int userId)
    {
        await _listaRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(ListaDTO dto, int userId)
    {   
        var account = _mapper.Map<Account>(dto);
        await _accountRepository.Update(account, userId);
        return true;
    }

}
