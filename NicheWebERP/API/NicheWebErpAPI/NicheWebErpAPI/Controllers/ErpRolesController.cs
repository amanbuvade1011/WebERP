using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ErpRolesController : ControllerBase
    {
        private readonly IErpRoleService _erpRoleService;

        public ErpRolesController(IErpRoleService erpRoleService)
        {
            _erpRoleService = erpRoleService;
        }

        // GET api/ErpRoles/GetAllRoles
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<IEnumerable<ErpRoleDto>>> GetAllRoles()
        {
            var roles = await _erpRoleService.GetAllRolesAsync();
            return Ok(roles);
        }
    }
}
