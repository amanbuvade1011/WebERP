using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PricePointsController : ControllerBase
    {
        private readonly IStylePricingService _pricingService;

        public PricePointsController(IStylePricingService pricingService)
        {
            _pricingService = pricingService;
        }

        // GET api/PricePoints/GetAllPricePoints
        [HttpGet("GetAllPricePoints")]
        public async Task<ActionResult<IEnumerable<PricePointDto>>> GetAllPricePoints() =>
            Ok(await _pricingService.GetAllPricePointsAsync());
    }
}
