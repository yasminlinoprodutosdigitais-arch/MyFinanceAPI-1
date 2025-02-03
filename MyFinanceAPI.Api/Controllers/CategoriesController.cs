// Controllers/CategoriesController.cs

using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.WebUI.Controllers
{
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

        // GET: api/categories
        [Authorize(Policy = "Admin")]
        [HttpGet("/GetCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            int? userId = _userContextService.GetUserIdFromClaims();;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var categories = await _categoryService.GetCategories(userId.Value);
            if (categories is null)
                return NotFound("");
            else
                return Ok(categories);
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("/GetCategoryById/{id}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDTO>> GetCategorybyId(int id)
        {
            int? userId = _userContextService.GetUserIdFromClaims();;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var category = await _categoryService.GetCategoryById(id, userId.Value);
            if (category is null)
                return NotFound("Category not found");
            else
                return Ok(category);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("/CreateCategory")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            int? userId = _userContextService.GetUserIdFromClaims();;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            if (categoryDto is null)
                return BadRequest("Invalid Data");

            await _categoryService.Add(categoryDto, userId.Value);
            return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.Id }, categoryDto);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("/UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDTO categoryDto)
        {
            int? userId = _userContextService.GetUserIdFromClaims();;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            if (categoryDto is null)
            {
                return BadRequest();
            }

            await _categoryService.Update(categoryDto, userId.Value);
            return Ok(categoryDto);
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("/DeleteCategory/{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            int? userId = _userContextService.GetUserIdFromClaims();;
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var category = await _categoryService.GetCategoryById(id, userId.Value);
            if (category is null)
                return NotFound("Category not found");

            await _categoryService.Remove(id, userId.Value);
            return Ok(category);
        }

        private int? GetUserIdFromClaims()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null || !int.TryParse(userIdString, out var userId))
                return null;

            return userId;
        }
    }
}
