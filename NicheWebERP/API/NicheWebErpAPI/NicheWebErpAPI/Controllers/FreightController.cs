using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FreightController : ControllerBase
    {
        private readonly IFreightService _freightService;

        public FreightController(IFreightService freightService)
        {
            _freightService = freightService;
        }

        // GET api/Freight/CalculateFreight?locationId=&quantity=&countryId=
        [HttpGet("CalculateFreight")]
        public async Task<ActionResult<CalculateFreightResultDto>> CalculateFreight(
            [FromQuery] Guid locationId, [FromQuery] int quantity, [FromQuery] Guid? countryId = null)
        {
            var price = await _freightService.CalculateAsync(locationId, quantity, countryId);
            return Ok(new CalculateFreightResultDto { Price = price });
        }
    }
}
