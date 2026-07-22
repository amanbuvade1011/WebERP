using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // GET api/Invoices/GetAllInvoices
        [HttpGet("GetAllInvoices")]
        public async Task<ActionResult<PagedResultDto<InvoiceListItemDto>>> GetAllInvoices(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? status = null,
            [FromQuery] Guid? firmId = null, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null)
        {
            return Ok(await _invoiceService.GetPagedAsync(page, pageSize, status, firmId, dateFrom, dateTo));
        }

        // GET api/Invoices/GetInvoiceById/{id}
        [HttpGet("GetInvoiceById/{id:guid}")]
        public async Task<ActionResult<InvoiceDetailDto>> GetInvoiceById(Guid id)
        {
            try
            {
                return Ok(await _invoiceService.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
