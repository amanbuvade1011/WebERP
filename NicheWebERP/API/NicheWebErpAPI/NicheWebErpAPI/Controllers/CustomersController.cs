using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly IRetailCustomerService _retailCustomerService;

        public CustomersController(IRetailCustomerService retailCustomerService)
        {
            _retailCustomerService = retailCustomerService;
        }

        // GET api/Customers/GetAllRetailCustomers
        [HttpGet("GetAllRetailCustomers")]
        public async Task<ActionResult<PagedResultDto<RetailCustomerListItemDto>>> GetAllRetailCustomers(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null)
        {
            return Ok(await _retailCustomerService.GetPagedAsync(page, pageSize, search));
        }

        // POST api/Customers/CreateRetailCustomer
        [HttpPost("CreateRetailCustomer")]
        public async Task<ActionResult<RetailCustomerDetailDto>> CreateRetailCustomer(CreateRetailCustomerDto dto) =>
            Ok(await _retailCustomerService.CreateAsync(dto));
    }
}
