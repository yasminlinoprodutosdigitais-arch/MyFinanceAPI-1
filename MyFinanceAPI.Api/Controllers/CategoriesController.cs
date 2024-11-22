using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
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

        [HttpGet("{id}", Name = "GetCategory")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            try
            {
                var category = await _categoryService.GetCategories();
                return Ok(category);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto is null)
                return BadRequest("Invalid Data");
           
            await _categoryService.Add(categoryDto);
            return new CreatedAtRouteResult("GetCategory", new { id = categoryDto.Id }, categoryDto);
            
        }

    }
}
