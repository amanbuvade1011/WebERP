using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ErpUsersController : ControllerBase
    {
        private readonly IErpUserService _erpUserService;

        public ErpUsersController(IErpUserService erpUserService)
        {
            _erpUserService = erpUserService;
        }

        // GET api/ErpUsers/GetAllUsers
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ErpUserDto>>> GetAllUsers()
        {
            var users = await _erpUserService.GetAllUsersAsync();
            return Ok(users);
        }

        // POST api/ErpUsers/CreateUser
        [HttpPost("CreateUser")]
        public async Task<ActionResult<ErpUserDto>> CreateUser(CreateErpUserDto dto)
        {
            try
            {
                var created = await _erpUserService.CreateUserAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT api/ErpUsers/UpdateUser/{id}
        [HttpPut("UpdateUser/{id:int}")]
        public async Task<ActionResult<ErpUserDto>> UpdateUser(int id, UpdateErpUserDto dto)
        {
            try
            {
                var updated = await _erpUserService.UpdateUserAsync(id, dto);
                return Ok(updated);
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

        // PUT api/ErpUsers/{id}/ResetPassword
        [HttpPut("{id:int}/ResetPassword")]
        public async Task<IActionResult> ResetPassword(int id, ResetPasswordDto dto)
        {
            try
            {
                await _erpUserService.ResetPasswordAsync(id, dto.NewPassword);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
