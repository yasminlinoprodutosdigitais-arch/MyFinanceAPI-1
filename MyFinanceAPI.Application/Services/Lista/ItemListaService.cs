using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class ItemListaService(IItemListaRepository itemListaRepository, IMapper mapper)
    : ServicoCadastro<ItemLista, ItemListaDTO>(itemListaRepository, mapper), IItemListaService
{
    public async Task<List<ItemListaDTO>> GetItemListaByListaId(int id, int userId) =>
        Mapper.Map<List<ItemListaDTO>>(await itemListaRepository.GetItemListaByListaId(id, userId));
}
