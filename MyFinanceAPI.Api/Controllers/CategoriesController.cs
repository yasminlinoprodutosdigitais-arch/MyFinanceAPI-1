// Controllers/CategoriesController.cs

using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyFinanceAPI.WebUI.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserContextService _userContextService;

        public CategoriesController(ICategoryService categoryService, IUserContextService userContextService)
        {
            _categoryService = categoryService;
            _userContextService = userContextService;
        }

        [HttpGet("/GetCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            int? userId = _userContextService.GetUserIdFromClaims(); ;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var categories = await _categoryService.GetCategories(userId.Value);
            if (categories is null)
                return NotFound("");
            else
                return Ok(categories);
        }

        [HttpGet("/GetCategoryById/{id}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDTO>> GetCategorybyId(int id)
        {
            int? userId = _userContextService.GetUserIdFromClaims(); ;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var category = await _categoryService.GetCategoryById(id, userId.Value);
            if (category is null)
                return NotFound("Category not found");
            else
                return Ok(category);
        }

        [HttpPost("/CreateCategory")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            int? userId = _userContextService.GetUserIdFromClaims(); ;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            if (categoryDto is null)
                return BadRequest("Invalid Data");

            await _categoryService.Add(categoryDto, userId.Value);
            return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("/UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                int? userId = _userContextService.GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { message = "User not authorized" });

                await _categoryService.Update(categoryDto, userId.Value);
                return Ok(new { message = "Categoria atualizada com sucesso!" });

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {message = "Ocorreu um erro inesperado.", details = ex.Message});
            }
        }

        [HttpDelete("/DeleteCategory/{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            int? userId = _userContextService.GetUserIdFromClaims(); ;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var category = await _categoryService.GetCategoryById(id, userId.Value);
            if (category is null)
                return NotFound("Category not found");

            await _categoryService.Remove(id, userId.Value);
            return Ok(category);
        }
    }
}
