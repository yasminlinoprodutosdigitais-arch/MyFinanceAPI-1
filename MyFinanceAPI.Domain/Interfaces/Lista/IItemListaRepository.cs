using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Domain.Interfaces;

public interface IItemListaRepository : IRepositorioDoUsuario<ItemLista>
{
    Task<List<ItemLista>> GetItemListaByListaId(int id, int userId);
}
