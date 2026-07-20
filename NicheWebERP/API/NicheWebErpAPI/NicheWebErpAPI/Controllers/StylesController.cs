using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StylesController : ControllerBase
    {
        private readonly IStyleService _styleService;
        private readonly IStylePricingService _pricingService;
        private readonly IProductGenerationService _productGenerationService;

        public StylesController(
            IStyleService styleService,
            IStylePricingService pricingService,
            IProductGenerationService productGenerationService)
        {
            _styleService = styleService;
            _pricingService = pricingService;
            _productGenerationService = productGenerationService;
        }

        // GET api/Styles/GetAllStyles
        [HttpGet("GetAllStyles")]
        public async Task<ActionResult<PagedResultDto<StyleListItemDto>>> GetAllStyles(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null,
            [FromQuery] Guid? categoryId = null, [FromQuery] Guid? labelId = null,
            [FromQuery] Guid? rangeId = null, [FromQuery] bool? inactive = null)
        {
            var result = await _styleService.GetPagedAsync(page, pageSize, search, categoryId, labelId, rangeId, inactive);
            return Ok(result);
        }

        // GET api/Styles/GetStyleById/{id}
        [HttpGet("GetStyleById/{id:guid}")]
        public async Task<ActionResult<StyleDetailDto>> GetStyleById(Guid id)
        {
            try
            {
                return Ok(await _styleService.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST api/Styles/CreateStyle
        [HttpPost("CreateStyle")]
        public async Task<ActionResult<StyleDetailDto>> CreateStyle(CreateStyleDto dto)
        {
            try
            {
                return Ok(await _styleService.CreateAsync(dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/Styles/UpdateStyle/{id}
        [HttpPut("UpdateStyle/{id:guid}")]
        public async Task<ActionResult<StyleDetailDto>> UpdateStyle(Guid id, UpdateStyleDto dto)
        {
            try
            {
                return Ok(await _styleService.UpdateAsync(id, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/Styles/{id}/Colors/AddColor
        [HttpPost("{id:guid}/Colors/AddColor")]
        public async Task<ActionResult<StyleColorDto>> AddColor(Guid id, AddColorDto dto)
        {
            try
            {
                return Ok(await _styleService.AddColorAsync(id, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/Styles/{id}/GetPrices
        [HttpGet("{id:guid}/GetPrices")]
        public async Task<ActionResult<IEnumerable<StylePriceDto>>> GetPrices(Guid id) =>
            Ok(await _pricingService.GetPricesAsync(id));

        // PUT api/Styles/{id}/UpdatePrices
        [HttpPut("{id:guid}/UpdatePrices")]
        public async Task<ActionResult<IEnumerable<StylePriceDto>>> UpdatePrices(Guid id, UpdateStylePricesDto dto)
        {
            try
            {
                return Ok(await _pricingService.UpdatePricesAsync(id, dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/Styles/{id}/SellLocations
        [HttpGet("{id:guid}/SellLocations")]
        public async Task<ActionResult<IEnumerable<StyleSellLocationDto>>> GetSellLocations(Guid id) =>
            Ok(await _pricingService.GetSellLocationsAsync(id));

        // PUT api/Styles/{id}/SellLocations
        [HttpPut("{id:guid}/SellLocations")]
        public async Task<ActionResult<IEnumerable<StyleSellLocationDto>>> UpdateSellLocations(Guid id, UpdateSellLocationsDto dto) =>
            Ok(await _pricingService.UpdateSellLocationsAsync(id, dto));

        // POST api/Styles/{styleId}/Colors/{colorId}/GenerateProducts
        [HttpPost("{styleId:guid}/Colors/{colorId:guid}/GenerateProducts")]
        public async Task<ActionResult<GenerateProductsResultDto>> GenerateProducts(Guid styleId, Guid colorId)
        {
            try
            {
                return Ok(await _productGenerationService.GenerateProductsAsync(styleId, colorId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
