using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IInvoiceService
    {
        Task<PagedResultDto<InvoiceListItemDto>> GetPagedAsync(
            int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo);
        Task<InvoiceDetailDto> GetByIdAsync(Guid id);
        Task<InvoiceDetailDto> GenerateFromOrderAsync(Guid orderId);

        // Added Sprint 12 (Dashboard) - thin pass-throughs to the repository aggregates.
        Task<InvoiceStatusSummaryDto> GetStatusSummaryAsync();
        Task<List<FirmOverCreditLimitDto>> GetFirmsOverCreditLimitAsync();
        Task<List<ArAgingBucketDto>> GetArAgingAsync();
    }
}
