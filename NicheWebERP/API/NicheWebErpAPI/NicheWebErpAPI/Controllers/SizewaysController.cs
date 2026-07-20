using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SizewaysController : ControllerBase
    {
        private readonly ISizewayService _sizewayService;

        public SizewaysController(ISizewayService sizewayService)
        {
            _sizewayService = sizewayService;
        }

        // GET api/Sizeways/GetAllSizeways
        [HttpGet("GetAllSizeways")]
        public async Task<ActionResult<IEnumerable<SizewayDto>>> GetAllSizeways() =>
            Ok(await _sizewayService.GetAllAsync());

        // POST api/Sizeways/CreateSizeway
        [HttpPost("CreateSizeway")]
        public async Task<ActionResult<SizewayDto>> CreateSizeway(CreateSizewayDto dto)
        {
            try
            {
                return Ok(await _sizewayService.CreateAsync(dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/Sizeways/{id}/UpdateSizeSequence
        [HttpPut("{id:guid}/UpdateSizeSequence")]
        public async Task<ActionResult<SizewayDto>> UpdateSizeSequence(Guid id, UpdateSizeSequenceDto dto)
        {
            try
            {
                return Ok(await _sizewayService.UpdateSizeSequenceAsync(id, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
