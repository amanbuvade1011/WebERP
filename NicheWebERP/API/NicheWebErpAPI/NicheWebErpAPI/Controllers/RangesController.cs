using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RangesController : ControllerBase
    {
        private readonly IRangeService _rangeService;

        public RangesController(IRangeService rangeService)
        {
            _rangeService = rangeService;
        }

        // GET api/Ranges/GetRangeTree
        [HttpGet("GetRangeTree")]
        public async Task<ActionResult<IEnumerable<RangeNodeDto>>> GetRangeTree() =>
            Ok(await _rangeService.GetTreeAsync());
    }
}
