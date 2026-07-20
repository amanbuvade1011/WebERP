using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        // GET api/Sizes/GetAllSizes
        [HttpGet("GetAllSizes")]
        public async Task<ActionResult<IEnumerable<SizeDto>>> GetAllSizes() =>
            Ok(await _sizeService.GetAllAsync());
    }
}
