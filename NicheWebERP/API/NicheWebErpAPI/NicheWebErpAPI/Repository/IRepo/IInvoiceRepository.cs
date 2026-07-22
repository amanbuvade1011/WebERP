using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IInvoiceRepository
    {
        Task<Firm?> GetFirmAsync(Guid companyId, Guid firmId);
        Task<CompanyLocation?> GetLocationAsync(Guid companyId, Guid locationId);
        Task<TradingTerms?> GetTradingTermsAsync(Guid companyId, Guid termsId);
        Task<ProductDescriptor?> GetProductDescriptorAsync(Guid companyId, Guid productId);

        // The sales order this invoice is generated from - filtered to
        // SalesOrderRepository.OrderClass, reusing that constant directly rather than
        // duplicating the class-name literal.
        Task<TransactionBase?> GetOrderHeaderAsync(Guid companyId, Guid orderId);
        Task<List<TransactionLine>> GetOrderLinesAsync(Guid companyId, Guid orderId);
        Task<List<TransactionQuantity>> GetLineQuantitiesAsync(Guid companyId, Guid lineId);
        void UpdateOrderHeader(TransactionBase header);

        Task<int> GetNextDocumentNumberAsync(Guid companyId, string entityClassName);

        Task<PagedResultDto<InvoiceListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo);
        Task<TransactionBase?> GetInvoiceHeaderAsync(Guid companyId, Guid id);
        Task<InvoiceDetailDto?> GetDetailAsync(Guid companyId, Guid id);
        Task<decimal> GetAllocatedAmountAsync(Guid companyId, Guid invoiceId);

        // Added Sprint 12 (Dashboard) - aggregate counterparts to GetPagedAsync's per-invoice
        // status computation, without paging through the whole set. Reuses the same
        // ComputeStatus logic so the dashboard's numbers can never drift from the Invoices list's.
        Task<InvoiceStatusSummaryDto> GetStatusSummaryAsync(Guid companyId);
        Task<List<FirmOverCreditLimitDto>> GetFirmsOverCreditLimitAsync(Guid companyId);
        Task<List<ArAgingBucketDto>> GetArAgingAsync(Guid companyId);

        Task AddHeaderAsync(TransactionBase header);
        Task AddLineAsync(TransactionLine line);
        Task AddQuantityAsync(TransactionQuantity quantity);

        Task<int> SaveChangesAsync();
    }
}
