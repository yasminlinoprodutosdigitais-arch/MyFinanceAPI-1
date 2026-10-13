// Controllers/CategoriesController.cs

using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyFinanceAPI.Api.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class CategoriesController : ApiControllerBase
    {
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
            if (categories is null)
                return NotFound("");
            else
                return Ok(categories);
        }

        [HttpGet("/GetCategoriasAtivas")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriasAtivas()
        {
            var userId = UsuarioId;

            var categories = await _categoryService.GetCategoriasAtivas(userId);
            if (categories is null)
                return NotFound("");
            else
                return Ok(categories);
        }

        [HttpGet("/GetCategoryById/{id}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDTO>> GetCategorybyId(int id)
        {
            var userId = UsuarioId;

            var category = await _categoryService.ObterPorIdAsync(id, userId);
            if (category is null)
                return NotFound("Category not found");
            else
                return Ok(category);
        }

        [HttpPost("/CreateCategory")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            var userId = UsuarioId;

            if (categoryDto is null)
                return BadRequest("Invalid Data");

            await _categoryService.AdicionarAsync(categoryDto, userId);
            return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("/UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDTO categoryDto)
        {
            var userId = UsuarioId;

            // Categoria alheia ou inexistente: NaoEncontradoException → 404.
            await _categoryService.AtualizarAsync(categoryDto, userId);
            return Ok(new { message = "Categoria atualizada com sucesso!" });
        }

        [HttpDelete("/DeleteCategory/{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var userId = UsuarioId;

            var category = await _categoryService.ObterPorIdAsync(id, userId);
            if (category is null)
                return NotFound("Categoria não encontrada!");

            await _categoryService.RemoverAsync(id, userId);
            return Ok(category);
        }
    }
}
