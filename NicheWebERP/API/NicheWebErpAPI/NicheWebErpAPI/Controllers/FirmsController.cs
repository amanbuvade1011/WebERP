using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FirmsController : ControllerBase
    {
        private readonly IFirmService _firmService;

        public FirmsController(IFirmService firmService)
        {
            _firmService = firmService;
        }

        // GET api/Firms/GetAllFirms
        [HttpGet("GetAllFirms")]
        public async Task<ActionResult<PagedResultDto<FirmListItemDto>>> GetAllFirms(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null,
            [FromQuery] string? entityClassName = null)
        {
            return Ok(await _firmService.GetPagedAsync(page, pageSize, search, entityClassName));
        }

        // GET api/Firms/GetFirmById/{id}
        [HttpGet("GetFirmById/{id:guid}")]
        public async Task<ActionResult<FirmDetailDto>> GetFirmById(Guid id)
        {
            try
            {
                return Ok(await _firmService.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST api/Firms/CreateFirm
        [HttpPost("CreateFirm")]
        public async Task<ActionResult<FirmDetailDto>> CreateFirm(CreateFirmDto dto) =>
            Ok(await _firmService.CreateAsync(dto));

        // PUT api/Firms/UpdateFirm/{id}
        [HttpPut("UpdateFirm/{id:guid}")]
        public async Task<ActionResult<FirmDetailDto>> UpdateFirm(Guid id, UpdateFirmDto dto)
        {
            try
            {
                return Ok(await _firmService.UpdateAsync(id, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
