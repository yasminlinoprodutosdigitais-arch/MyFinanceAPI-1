using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class ItemListaService : IItemListaService
{
    private readonly IItemListaRepository _itemListaRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public ItemListaService(IItemListaRepository ItemListaRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _itemListaRepository = ItemListaRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    public async Task Add(ItemListaDTO ItemListaDTO, int userId)
    {
        var ItemLista = _mapper.Map<ItemLista>(ItemListaDTO);
        ItemLista.UserId = userId;
        await _itemListaRepository.Create(ItemLista);
    }

    public async Task<IEnumerable<ItemListaDTO>> GetItemLista(int userId)
    {
        var movimentacoDiaria = await _itemListaRepository.GetItemListas(userId);
        return _mapper.Map<IEnumerable<ItemListaDTO>>(movimentacoDiaria);
    }

    public async Task<ItemListaDTO> GetItemListaById(int id, int userId)
    {
        var ItemLista = await _itemListaRepository.GetItemListaById(id, userId);
        return _mapper.Map<ItemListaDTO>(ItemLista);
    }

    public async Task Remove(int id, int userId)
    {
        await _itemListaRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(ItemListaDTO dto, int userId)
    {   
        var account = _mapper.Map<Account>(dto);
        await _accountRepository.Update(account, userId);
        return true;
    }

}
