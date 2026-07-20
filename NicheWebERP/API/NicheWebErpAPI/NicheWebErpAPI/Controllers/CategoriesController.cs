using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET api/Categories/GetCategoryTree
        [HttpGet("GetCategoryTree")]
        public async Task<ActionResult<IEnumerable<CategoryNodeDto>>> GetCategoryTree() =>
            Ok(await _categoryService.GetTreeAsync());

        // POST api/Categories/CreateCategory
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<CategoryNodeDto>> CreateCategory(CreateCategoryDto dto)
        {
            try
            {
                return Ok(await _categoryService.CreateAsync(dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
