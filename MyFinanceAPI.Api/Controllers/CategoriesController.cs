// Controllers/CategoriesController.cs

using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MyFinanceAPI.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories
        [Authorize(Policy = "Admin")]
        [HttpGet("/GetCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryService.GetCategories();
            if(categories is null)
                return NotFound("");
            else
                return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("/GetCategoryById/{id}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDTO>> GetCategorybyId(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if(category is null)
                return NotFound("Category not found");
            else
                return Ok(category);
        }

        // POST: api/categories
        [HttpPost("/CreateCategory")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto is null)
                return BadRequest("Invalid Data");
           
            await _categoryService.Add(categoryDto);
            return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.Id }, categoryDto);
            
        }

        // PUT: api/categories/{id}
        [HttpPut("/UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto is null)
            {
                return BadRequest();
            }

            await _categoryService.Update(categoryDto);
            return Ok(categoryDto);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("/DeleteCategory/{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category is null)
                return NotFound("Category not found");

            await _categoryService.Remove(id);
            return Ok(category);
        }
    }
}
