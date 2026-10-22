using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ICategoriaService : IServicoCadastro<CategoriaDTO>
{
    Task<IEnumerable<CategoriaDTO>> GetCategoriasAtivas(int userId);
}
