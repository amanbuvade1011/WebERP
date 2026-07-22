using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IInvoiceService _invoiceService;
        private readonly ICuttingSheetService _cuttingSheetService;

        public SalesOrdersController(
            ISalesOrderService salesOrderService, IInvoiceService invoiceService, ICuttingSheetService cuttingSheetService)
        {
            _salesOrderService = salesOrderService;
            _invoiceService = invoiceService;
            _cuttingSheetService = cuttingSheetService;
        }

        // GET api/SalesOrders/GetAllSalesOrders
        [HttpGet("GetAllSalesOrders")]
        public async Task<ActionResult<PagedResultDto<SalesOrderListItemDto>>> GetAllSalesOrders(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? status = null,
            [FromQuery] Guid? firmId = null, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null)
        {
            return Ok(await _salesOrderService.GetPagedAsync(page, pageSize, status, firmId, dateFrom, dateTo));
        }

        // GET api/SalesOrders/GetSalesOrderById/{id}
        [HttpGet("GetSalesOrderById/{id:guid}")]
        public async Task<ActionResult<SalesOrderDetailDto>> GetSalesOrderById(Guid id)
        {
            try
            {
                return Ok(await _salesOrderService.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST api/SalesOrders/CreateSalesOrder
        [HttpPost("CreateSalesOrder")]
        public async Task<ActionResult<SalesOrderDetailDto>> CreateSalesOrder(CreateSalesOrderDto dto)
        {
            try
            {
                return Ok(await _salesOrderService.CreateAsync(dto));
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

        // PUT api/SalesOrders/{id}/UpdateStatus
        [HttpPut("{id:guid}/UpdateStatus")]
        public async Task<ActionResult<SalesOrderDetailDto>> UpdateStatus(Guid id, UpdateSalesOrderStatusDto dto)
        {
            try
            {
                return Ok(await _salesOrderService.UpdateStatusAsync(id, dto));
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

        // POST api/SalesOrders/{id}/AddLine
        [HttpPost("{id:guid}/AddLine")]
        public async Task<ActionResult<SalesOrderDetailDto>> AddLine(Guid id, AddSalesOrderLineDto dto)
        {
            try
            {
                return Ok(await _salesOrderService.AddLineAsync(id, dto));
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

        // PUT api/SalesOrders/{id}/UpdateLine/{lineId}
        [HttpPut("{id:guid}/UpdateLine/{lineId:guid}")]
        public async Task<ActionResult<SalesOrderDetailDto>> UpdateLine(Guid id, Guid lineId, UpdateSalesOrderLineDto dto)
        {
            try
            {
                return Ok(await _salesOrderService.UpdateLineAsync(id, lineId, dto));
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

        // POST api/SalesOrders/{id}/GenerateInvoice
        [HttpPost("{id:guid}/GenerateInvoice")]
        public async Task<ActionResult<InvoiceDetailDto>> GenerateInvoice(Guid id)
        {
            try
            {
                return Ok(await _invoiceService.GenerateFromOrderAsync(id));
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

        // GET api/SalesOrders/{id}/CuttingSheetPreview
        [HttpGet("{id:guid}/CuttingSheetPreview")]
        public async Task<ActionResult<List<CuttingSheetPreviewGroupDto>>> CuttingSheetPreview(Guid id)
        {
            try
            {
                return Ok(await _cuttingSheetService.PreviewFromOrderAsync(id));
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

        // POST api/SalesOrders/{id}/GenerateCuttingSheet
        [HttpPost("{id:guid}/GenerateCuttingSheet")]
        public async Task<ActionResult<List<CuttingSheetDetailDto>>> GenerateCuttingSheet(Guid id)
        {
            try
            {
                return Ok(await _cuttingSheetService.GenerateFromOrderAsync(id));
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

        // DELETE api/SalesOrders/{id}/RemoveLine/{lineId}
        [HttpDelete("{id:guid}/RemoveLine/{lineId:guid}")]
        public async Task<ActionResult<SalesOrderDetailDto>> RemoveLine(Guid id, Guid lineId)
        {
            try
            {
                return Ok(await _salesOrderService.RemoveLineAsync(id, lineId));
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
    }
}
