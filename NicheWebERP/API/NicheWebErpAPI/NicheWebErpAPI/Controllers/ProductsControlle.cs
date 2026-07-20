using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductGenerationService _productGenerationService;

        public ProductsController(IProductService productService, IProductGenerationService productGenerationService)
        {
            _productService = productService;
            _productGenerationService = productGenerationService;
        }

        // GET api/Products/GetAllProducts
        // Sprint 03: now paged + filterable (was a raw, unpaged array before - see
        // docs/ai-plan/01-database-map.md / 02-api-plan.md for why every list endpoint should
        // be paged from the start). Now requires auth (was anonymous before Sprint 01) since it
        // needs the current company context - the /products route was already guarded on the
        // frontend, so this doesn't break anything the app actually relies on.
        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<PagedResultDto<ProductListItemDto>>> GetAllProducts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null,
            [FromQuery] Guid? categoryId = null, [FromQuery] Guid? labelId = null)
        {
            var products = await _productService.GetAllProductsAsync(page, pageSize, search, categoryId, labelId);
            return Ok(products);
        }

        // GET api/Products/GetProductStock/{productId}
        [HttpGet("GetProductStock/{productId:guid}")]
        public async Task<ActionResult<ProductStockDto>> GetProductStock(Guid productId)
        {
            try
            {
                return Ok(await _productGenerationService.GetProductStockAsync(productId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET api/Products/GetStockByStyleColor/{styleId}/{colorId}
        [HttpGet("GetStockByStyleColor/{styleId:guid}/{colorId:guid}")]
        public async Task<ActionResult<IEnumerable<StockGridRowDto>>> GetStockByStyleColor(Guid styleId, Guid colorId) =>
            Ok(await _productGenerationService.GetStockByStyleColorAsync(styleId, colorId));
    }
}
