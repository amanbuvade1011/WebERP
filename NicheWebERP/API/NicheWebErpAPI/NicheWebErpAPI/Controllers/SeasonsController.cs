using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SeasonsController : ControllerBase
    {
        private readonly ISeasonService _seasonService;

        public SeasonsController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        // GET api/Seasons/GetAllSeasons
        [HttpGet("GetAllSeasons")]
        public async Task<ActionResult<IEnumerable<SeasonDto>>> GetAllSeasons() =>
            Ok(await _seasonService.GetAllAsync());
    }
}
