using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IInvoiceService
    {
        Task<PagedResultDto<InvoiceListItemDto>> GetPagedAsync(
            int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo);
        Task<InvoiceDetailDto> GetByIdAsync(Guid id);
        Task<InvoiceDetailDto> GenerateFromOrderAsync(Guid orderId);
    }
}
