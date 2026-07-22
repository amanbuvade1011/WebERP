using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;

namespace NicheWebErpAPI.Repository.Repo
{
    // Invoices live on the same generic legacy TransactionBase/TransactionLine/
    // TransactionQuantity tables as Sales Orders (EntityClassName = "WholesaleInvoice" - new,
    // no live precedent, decided in Sprint 06 after confirming zero live Invoice/CreditNote rows
    // exist - see docs/ai-plan/sprints/sprint-06-invoicing-payments.md). Self-sufficient like
    // SalesOrderRepository - doesn't cross-depend on ISalesOrderRepository.
    public class InvoiceRepository : IInvoiceRepository
    {
        public const string InvoiceClass = "WholesaleInvoice";

        private readonly IEfCoreService<TransactionBase> _headerService;
        private readonly IEfCoreService<TransactionLine> _lineService;
        private readonly IEfCoreService<TransactionQuantity> _quantityService;
        private readonly IEfCoreService<Firm> _firmService;
        private readonly IEfCoreService<CompanyLocation> _locationService;
        private readonly IEfCoreService<TradingTerms> _tradingTermsService;
        private readonly IEfCoreService<Product> _productService;
        private readonly IEfCoreService<Style> _styleService;
        private readonly IEfCoreService<StyleColor> _styleColorService;
        private readonly IEfCoreService<SizewayItem> _sizewayItemService;
        private readonly IEfCoreService<Size> _sizeService;
        private readonly IEfCoreService<ProductLocation> _productLocationService;
        private readonly IEfCoreService<FinancialAllocation> _allocationService;
        private readonly IEfCoreService<PaymentMethod> _paymentMethodService;

        public InvoiceRepository(
            IEfCoreService<TransactionBase> headerService,
            IEfCoreService<TransactionLine> lineService,
            IEfCoreService<TransactionQuantity> quantityService,
            IEfCoreService<Firm> firmService,
            IEfCoreService<CompanyLocation> locationService,
            IEfCoreService<TradingTerms> tradingTermsService,
            IEfCoreService<Product> productService,
            IEfCoreService<Style> styleService,
            IEfCoreService<StyleColor> styleColorService,
            IEfCoreService<SizewayItem> sizewayItemService,
            IEfCoreService<Size> sizeService,
            IEfCoreService<ProductLocation> productLocationService,
            IEfCoreService<FinancialAllocation> allocationService,
            IEfCoreService<PaymentMethod> paymentMethodService)
        {
            _headerService = headerService;
            _lineService = lineService;
            _quantityService = quantityService;
            _firmService = firmService;
            _locationService = locationService;
            _tradingTermsService = tradingTermsService;
            _productService = productService;
            _styleService = styleService;
            _styleColorService = styleColorService;
            _sizewayItemService = sizewayItemService;
            _sizeService = sizeService;
            _productLocationService = productLocationService;
            _allocationService = allocationService;
            _paymentMethodService = paymentMethodService;
        }

        public Task<Firm?> GetFirmAsync(Guid companyId, Guid firmId) =>
            _firmService.Query().FirstOrDefaultAsync(f => f.CompanyID == companyId && f.EntityID == firmId);

        public Task<CompanyLocation?> GetLocationAsync(Guid companyId, Guid locationId) =>
            _locationService.Query().FirstOrDefaultAsync(l => l.CompanyID == companyId && l.EntityID == locationId);

        public Task<TradingTerms?> GetTradingTermsAsync(Guid companyId, Guid termsId) =>
            _tradingTermsService.Query().FirstOrDefaultAsync(t => t.CompanyID == companyId && t.EntityID == termsId);

        public Task<ProductDescriptor?> GetProductDescriptorAsync(Guid companyId, Guid productId) =>
            (
                from p in _productService.Query()
                where p.CompanyID == companyId && p.EntityID == productId
                join st in _styleService.Query() on new { p.CompanyID, StyleID = p.StyleID } equals new { st.CompanyID, StyleID = st.EntityID }
                join sc in _styleColorService.Query() on new { p.CompanyID, StyleColorID = p.StyleColorID } equals new { sc.CompanyID, StyleColorID = sc.EntityID }
                join si in _sizewayItemService.Query() on new { p.CompanyID, SizewayItemID = p.SizewayItemID } equals new { si.CompanyID, SizewayItemID = si.EntityID }
                join sz in _sizeService.Query() on new { si.CompanyID, si.SizeID } equals new { sz.CompanyID, SizeID = sz.EntityID }
                select new ProductDescriptor
                {
                    ProductId = p.EntityID,
                    StyleId = st.EntityID,
                    StyleCode = st.Code,
                    StyleColorId = sc.EntityID,
                    Color = sc.Color,
                    SizeDescription = sz.Description,
                    Inactive = p.Inactive
                }
            ).FirstOrDefaultAsync();

        public Task<TransactionBase?> GetOrderHeaderAsync(Guid companyId, Guid orderId) =>
            _headerService.Query().FirstOrDefaultAsync(
                h => h.CompanyID == companyId && h.EntityID == orderId && h.EntityClassName == SalesOrderRepository.OrderClass);

        public Task<List<TransactionLine>> GetOrderLinesAsync(Guid companyId, Guid orderId) =>
            _lineService.Query()
                .Where(l => l.CompanyID == companyId && l.TransactionID == orderId && l.EntityClassName == SalesOrderRepository.LineClass)
                .ToListAsync();

        public Task<List<TransactionQuantity>> GetLineQuantitiesAsync(Guid companyId, Guid lineId) =>
            _quantityService.Query()
                .Where(q => q.CompanyID == companyId && q.TransactionLineID == lineId && q.EntityClassName == SalesOrderRepository.QuantityClass)
                .ToListAsync();

        public void UpdateOrderHeader(TransactionBase header) => _headerService.Update(header);

        public async Task<int> GetNextDocumentNumberAsync(Guid companyId, string entityClassName)
        {
            var query = _headerService.Query().Where(h => h.CompanyID == companyId && h.EntityClassName == entityClassName);
            var max = await query.Select(h => (int?)h.DocumentNumber1).MaxAsync();
            return (max ?? 0) + 1;
        }

        public Task<TransactionBase?> GetInvoiceHeaderAsync(Guid companyId, Guid id) =>
            _headerService.Query().FirstOrDefaultAsync(
                h => h.CompanyID == companyId && h.EntityID == id && h.EntityClassName == InvoiceClass);

        // Status is computed (Paid/Pending/Overdue), never stored - see InvoiceDto.cs. Filtering
        // by a computed value can't be pushed into SQL cleanly, and this DB's compat level (100)
        // blocks the usual `.Where(x => ids.Contains(x.Id))` pattern for batching lookups (the
        // Sprint 03 OPENJSON finding), so allocations are loaded company-wide (a small,
        // brand-new table) and matched in memory instead of per-id filtering.
        public async Task<PagedResultDto<InvoiceListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo)
        {
            var headerQuery =
                from h in _headerService.Query()
                where h.CompanyID == companyId && h.EntityClassName == InvoiceClass
                join f in _firmService.Query() on new { h.CompanyID, OtherPartyID = h.OtherPartyID } equals new { f.CompanyID, OtherPartyID = f.EntityID }
                select new { h, f };

            if (firmId.HasValue) headerQuery = headerQuery.Where(x => x.h.OtherPartyID == firmId.Value);
            if (dateFrom.HasValue) headerQuery = headerQuery.Where(x => x.h.TransactionDate >= dateFrom.Value);
            if (dateTo.HasValue) headerQuery = headerQuery.Where(x => x.h.TransactionDate <= dateTo.Value);

            var headers = await headerQuery.ToListAsync();
            var allocations = await _allocationService.Query().Where(a => a.CompanyID == companyId).ToListAsync();
            var orders = await _headerService.Query()
                .Where(h => h.CompanyID == companyId && h.EntityClassName == SalesOrderRepository.OrderClass)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var items = headers.Select(x =>
            {
                var paid = allocations.Where(a => a.TransactionToID == x.h.EntityID).Sum(a => a.Amount);
                var (invoiceStatus, statusName) = ComputeStatus(x.h.Amount1, paid, x.h.PaymentDueDate, now);
                var order = x.h.SalesOrderID.HasValue ? orders.FirstOrDefault(o => o.EntityID == x.h.SalesOrderID.Value) : null;

                return new InvoiceListItemDto
                {
                    Id = x.h.EntityID,
                    DocumentNumber = x.h.DocumentNumber1,
                    FirmId = x.f.EntityID,
                    FirmName = x.f.TradingName,
                    SalesOrderId = x.h.SalesOrderID,
                    SalesOrderDocumentNumber = order?.DocumentNumber1,
                    IssueDate = x.h.TransactionDate,
                    DueDate = x.h.PaymentDueDate,
                    TotalAmount = x.h.Amount1,
                    PaidAmount = paid,
                    Status = invoiceStatus,
                    StatusName = statusName
                };
            });

            if (status.HasValue)
            {
                items = items.Where(i => (int)i.Status == status.Value);
            }

            var materialized = items
                .OrderByDescending(i => i.IssueDate)
                .ThenByDescending(i => i.DocumentNumber)
                .ToList();

            var totalCount = materialized.Count;
            var paged = materialized.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResultDto<InvoiceListItemDto> { Items = paged, TotalCount = totalCount, Page = page, PageSize = pageSize };
        }

        public async Task<InvoiceDetailDto?> GetDetailAsync(Guid companyId, Guid id)
        {
            var header = await GetInvoiceHeaderAsync(companyId, id);
            if (header is null)
            {
                return null;
            }

            var firm = await _firmService.Query().FirstOrDefaultAsync(f => f.CompanyID == companyId && f.EntityID == header.OtherPartyID);
            var location = await _locationService.Query().FirstOrDefaultAsync(l => l.CompanyID == companyId && l.EntityID == header.LocationID);
            var order = header.SalesOrderID.HasValue
                ? await _headerService.Query().FirstOrDefaultAsync(h => h.CompanyID == companyId && h.EntityID == header.SalesOrderID.Value)
                : null;

            var paid = await GetAllocatedAmountAsync(companyId, id);
            var (invoiceStatus, statusName) = ComputeStatus(header.Amount1, paid, header.PaymentDueDate, DateTime.UtcNow);

            var dto = new InvoiceDetailDto
            {
                Id = header.EntityID,
                DocumentNumber = header.DocumentNumber1,
                FirmId = header.OtherPartyID,
                FirmName = firm?.TradingName ?? "(unknown)",
                SalesOrderId = header.SalesOrderID,
                SalesOrderDocumentNumber = order?.DocumentNumber1,
                LocationId = header.LocationID,
                LocationName = location?.Name,
                IssueDate = header.TransactionDate,
                DueDate = header.PaymentDueDate,
                CustomerReferenceNo = header.CustomerReferenceNo,
                Narration = header.Narration,
                SubTotalExTax = header.SubTotalStyles1,
                TaxAmount = header.TaxAmount1,
                TotalAmount = header.Amount1,
                PaidAmount = paid,
                RemainingAmount = header.Amount1 - paid,
                Status = invoiceStatus,
                StatusName = statusName
            };

            var lines =
                from tl in _lineService.Query()
                where tl.CompanyID == companyId && tl.TransactionID == id && tl.EntityClassName == SalesOrderRepository.LineClass
                join tq in _quantityService.Query() on new { tl.CompanyID, TransactionLineID = tl.EntityID }
                    equals new { tq.CompanyID, TransactionLineID = tq.TransactionLineID } into tqJoin
                from tq in tqJoin.DefaultIfEmpty()
                select new { tl, tq };

            var lineRows = await lines.ToListAsync();

            dto.Lines = lineRows
                .GroupBy(x => x.tl.EntityID)
                .Select(g =>
                {
                    var tl = g.First().tl;
                    var qty = g.Sum(x => x.tq?.Quantity ?? 0);
                    return new InvoiceLineDto
                    {
                        LineId = tl.EntityID,
                        ProductId = Guid.Empty,
                        Quantity = qty,
                        UnitPriceExTax = tl.StylePriceExTax1 ?? 0,
                        UnitPriceTax = tl.StylePriceTax1 ?? 0,
                        LineTotalExTax = tl.LineAmountExTax1,
                        LineTotalTax = tl.LineTaxAmount1
                    };
                })
                .ToList();

            foreach (var lineDto in dto.Lines)
            {
                var firstQuantity = lineRows.First(x => x.tl.EntityID == lineDto.LineId).tq;
                if (firstQuantity is null)
                {
                    continue;
                }

                var productLocation = await _productLocationService.Query()
                    .FirstOrDefaultAsync(pl => pl.CompanyID == companyId && pl.EntityID == firstQuantity.ProductLocationID);
                if (productLocation?.ProductID is null)
                {
                    continue;
                }

                var descriptor = await GetProductDescriptorAsync(companyId, productLocation.ProductID.Value);
                if (descriptor is null)
                {
                    continue;
                }

                lineDto.ProductId = descriptor.ProductId;
                lineDto.StyleCode = descriptor.StyleCode;
                lineDto.Color = descriptor.Color;
                lineDto.SizeDescription = descriptor.SizeDescription;
            }

            var allocationRows = await _allocationService.Query()
                .Where(a => a.CompanyID == companyId && a.TransactionToID == id)
                .ToListAsync();

            var payments = new List<PaymentSummaryDto>();
            foreach (var allocation in allocationRows)
            {
                var payment = await _headerService.Query()
                    .FirstOrDefaultAsync(h => h.CompanyID == companyId && h.EntityID == allocation.TransactionFromID);
                if (payment is null)
                {
                    continue;
                }

                var method = payment.PaymentMethodID.HasValue
                    ? await _paymentMethodService.Query().FirstOrDefaultAsync(pm => pm.CompanyID == companyId && pm.EntityID == payment.PaymentMethodID.Value)
                    : null;

                payments.Add(new PaymentSummaryDto
                {
                    PaymentId = payment.EntityID,
                    DocumentNumber = payment.DocumentNumber1,
                    PaymentDate = payment.TransactionDate,
                    Amount = allocation.Amount,
                    PaymentMethodName = method?.Description,
                    Narration = payment.Narration
                });
            }
            dto.Payments = payments.OrderByDescending(p => p.PaymentDate).ToList();

            return dto;
        }

        public async Task<decimal> GetAllocatedAmountAsync(Guid companyId, Guid invoiceId) =>
            await _allocationService.Query()
                .Where(a => a.CompanyID == companyId && a.TransactionToID == invoiceId)
                .SumAsync(a => (decimal?)a.Amount) ?? 0;

        public Task AddHeaderAsync(TransactionBase header) => _headerService.AddAsync(header);
        public Task AddLineAsync(TransactionLine line) => _lineService.AddAsync(line);
        public Task AddQuantityAsync(TransactionQuantity quantity) => _quantityService.AddAsync(quantity);

        public Task<int> SaveChangesAsync() => _headerService.SaveChangesAsync();

        private static (InvoiceStatus status, string name) ComputeStatus(decimal total, decimal paid, DateTime? dueDate, DateTime now)
        {
            if (paid >= total)
            {
                return (InvoiceStatus.Paid, nameof(InvoiceStatus.Paid));
            }
            if (dueDate.HasValue && dueDate.Value < now)
            {
                return (InvoiceStatus.Overdue, nameof(InvoiceStatus.Overdue));
            }
            return (InvoiceStatus.Pending, nameof(InvoiceStatus.Pending));
        }
    }
}
