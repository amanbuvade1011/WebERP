using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET api/Locations/GetAllLocations
        [HttpGet("GetAllLocations")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        // POST api/Locations/CreateLocation
        [HttpPost("CreateLocation")]
        public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationDto dto)
        {
            try
            {
                var created = await _locationService.CreateLocationAsync(dto);
                return Ok(created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
