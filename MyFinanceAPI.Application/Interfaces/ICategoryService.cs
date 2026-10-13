using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface ICategoryService : IServicoCadastro<CategoryDTO>
{
    Task<IEnumerable<CategoryDTO>> GetCategoriasAtivas(int userId);
}
