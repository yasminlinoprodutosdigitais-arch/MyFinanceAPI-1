// Controllers/CategoriesController.cs

using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    public class CategoriasController : ApiControllerBase
    {
        public const string RotaPorId = "GetCategory";

        private readonly ICategoriaService _categoryService;

        public CategoriasController(ICategoriaService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("/GetCategories")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategories()
        {
            var userId = UsuarioId;

            var categories = await _categoryService.ListarAsync(userId);
            return Ok(categories);
        }

        [HttpGet("/GetCategoriasAtivas")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasAtivas()
        {
            var userId = UsuarioId;

            var categories = await _categoryService.GetCategoriasAtivas(userId);
            return Ok(categories);
        }

        [HttpGet("/GetCategoryById/{id}", Name = RotaPorId)]
        public async Task<ActionResult<CategoriaDTO>> GetCategorybyId(int id)
        {
            var userId = UsuarioId;

            var category = await _categoryService.ObterPorIdAsync(id, userId);
            if (category is null)
                return NaoEncontrado("Categoria não encontrada.");

            return Ok(category);
        }

        [HttpPost("/CreateCategory")]
        public async Task<ActionResult<CategoriaDTO>> CreateCategory([FromBody] CategoriaDTO categoryDto)
        {
            var userId = UsuarioId;

            var criada = await _categoryService.AdicionarAsync(categoryDto, userId);
            return Criado(RotaPorId, criada.Id, criada);
        }

        [HttpPut("/UpdateCategory")]
        public async Task<ActionResult<CategoriaDTO>> UpdateCategory([FromBody] CategoriaDTO categoryDto)
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
