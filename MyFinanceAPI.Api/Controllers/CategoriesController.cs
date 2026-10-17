// Controllers/CategoriesController.cs

using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    public class CategoriesController : ApiControllerBase
    {
        public const string RotaPorId = "GetCategory";

        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("/GetCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var userId = UsuarioId;

            var categories = await _categoryService.ListarAsync(userId);
            return Ok(categories);
        }

        [HttpGet("/GetCategoriasAtivas")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriasAtivas()
        {
            var userId = UsuarioId;

            var categories = await _categoryService.GetCategoriasAtivas(userId);
            return Ok(categories);
        }

        [HttpGet("/GetCategoryById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<CategoryDTO>> GetCategorybyId(int id)
        {
            var userId = UsuarioId;

            var category = await _categoryService.ObterPorIdAsync(id, userId);
            if (category is null)
                return NaoEncontrado("Categoria não encontrada.");

            return Ok(category);
        }

        [HttpPost("/CreateCategory")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            var userId = UsuarioId;

            var criada = await _categoryService.AdicionarAsync(categoryDto, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateCategory")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory([FromBody] CategoryDTO categoryDto)
        {
            var userId = UsuarioId;

            // Categoria alheia ou inexistente: NaoEncontradoException → 404.
            await _categoryService.AtualizarAsync(categoryDto, userId);
            return Ok(categoryDto);
        }

        [HttpDelete("/DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = UsuarioId;

            var category = await _categoryService.ObterPorIdAsync(id, userId);
            if (category is null)
                return NaoEncontrado("Categoria não encontrada.");

            await _categoryService.RemoverAsync(id, userId);
            return NoContent();
        }
    }
}
