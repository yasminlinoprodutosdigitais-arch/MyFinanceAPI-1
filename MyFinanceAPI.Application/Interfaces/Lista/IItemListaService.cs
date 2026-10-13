using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IItemListaService : IServicoCadastro<ItemListaDTO>
{
    Task<List<ItemListaDTO>> GetItemListaByListaId(int id, int userId);
}
