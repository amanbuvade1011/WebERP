using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Repository.Repo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        // Order fulfillment statuses an invoice may be generated from - matches the sprint's
        // "confirmed/shipped sales order" wording. Draft/Cancelled are excluded.
        private static readonly int[] InvoiceableOrderStatuses = { 1, 2, 3 }; // Confirmed, Shipped, Delivered

        public InvoiceService(IInvoiceRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public Task<PagedResultDto<InvoiceListItemDto>> GetPagedAsync(
            int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;
            return _repository.GetPagedAsync(_currentUserService.CompanyId, page, pageSize, status, firmId, dateFrom, dateTo);
        }

        public async Task<InvoiceDetailDto> GetByIdAsync(Guid id) =>
            await _repository.GetDetailAsync(_currentUserService.CompanyId, id)
                ?? throw new KeyNotFoundException($"Invoice {id} not found.");

        public async Task<InvoiceDetailDto> GenerateFromOrderAsync(Guid orderId)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var order = await _repository.GetOrderHeaderAsync(companyId, orderId)
                ?? throw new KeyNotFoundException($"Sales order {orderId} not found.");

            if (!InvoiceableOrderStatuses.Contains(order.Status1 ?? 0))
            {
                throw new InvalidOperationException("Only a Confirmed, Shipped, or Delivered order can be invoiced.");
            }
            if (order.Status2 == 1)
            {
                throw new InvalidOperationException("This order has already been invoiced.");
            }

            var firm = await _repository.GetFirmAsync(companyId, order.OtherPartyID)
                ?? throw new KeyNotFoundException($"Firm {order.OtherPartyID} not found.");
            if (!firm.AllowInvoice)
            {
                throw new InvalidOperationException($"'{firm.TradingName}' is not allowed to be invoiced (AllowInvoice is off).");
            }

            var now = DateTime.UtcNow;
            var dueDate = now;
            if (order.TermsID.HasValue)
            {
                var terms = await _repository.GetTradingTermsAsync(companyId, order.TermsID.Value);
                if (terms is not null)
                {
                    dueDate = now.AddDays(terms.NumberOfDays);
                }
            }

            var invoiceId = Guid.NewGuid();
            var orderLines = await _repository.GetOrderLinesAsync(companyId, orderId);

            foreach (var orderLine in orderLines)
            {
                var quantities = await _repository.GetLineQuantitiesAsync(companyId, orderLine.EntityID);

                var newLine = new TransactionLine
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    TransactionID = invoiceId,
                    Sequence = orderLine.Sequence,
                    Description = orderLine.Description,
                    LineAmountExTax1 = orderLine.LineAmountExTax1,
                    EntityClassName = SalesOrderRepository.LineClass,
                    LineTaxAmount1 = orderLine.LineTaxAmount1,
                    StyleSellLocationID = orderLine.StyleSellLocationID,
                    StylePriceExTax1 = orderLine.StylePriceExTax1,
                    PricePointID = orderLine.PricePointID,
                    StylePriceTax1 = orderLine.StylePriceTax1,
                    LineQuantity1 = orderLine.LineQuantity1,
                    LastUpdated = now,
                    UpdatedByID = updatedById
                };
                await _repository.AddLineAsync(newLine);

                foreach (var quantity in quantities)
                {
                    var newQuantity = new TransactionQuantity
                    {
                        CompanyID = companyId,
                        EntityID = Guid.NewGuid(),
                        TransactionLineID = newLine.EntityID,
                        EntityClassName = SalesOrderRepository.QuantityClass,
                        Quantity = quantity.Quantity,
                        ProductLocationID = quantity.ProductLocationID,
                        Held = 0,
                        RequiredOut = 0,
                        Allocated = 0,
                        SalesOrderFirm = quantity.SalesOrderFirm,
                        SalesOrderPerson = quantity.SalesOrderPerson,
                        LastUpdated = now,
                        UpdatedByID = updatedById
                    };
                    await _repository.AddQuantityAsync(newQuantity);
                }
            }

            var documentNumber = await _repository.GetNextDocumentNumberAsync(companyId, InvoiceRepository.InvoiceClass);

            var header = new TransactionBase
            {
                CompanyID = companyId,
                EntityID = invoiceId,
                EntityClassName = InvoiceRepository.InvoiceClass,
                LaneID = Guid.Empty,
                Narration = order.Narration,
                TransactionDate = now,
                OtherPartyID = order.OtherPartyID,
                DocumentNumber1 = documentNumber,
                LocationID = order.LocationID,
                Amount1 = order.Amount1,
                PricePointID = order.PricePointID,
                AgentID = Guid.Empty,
                RepresentativeID = Guid.Empty,
                IsDraft = false,
                TaxAmount1 = order.TaxAmount1,
                NumLines1 = order.NumLines1,
                TotalQuantities1 = order.TotalQuantities1,
                SubTotalStyles1 = order.SubTotalStyles1,
                SubTotalStylesTax1 = order.SubTotalStylesTax1,
                SalesOrderID = order.EntityID,
                PaymentDueDate = dueDate,
                TermsID = order.TermsID,
                CustomerReferenceNo = order.CustomerReferenceNo,
                EmployeeID = updatedById,
                LastUpdated = now,
                UpdatedByID = updatedById
            };
            await _repository.AddHeaderAsync(header);

            order.Status2 = 1;
            order.LastUpdated = now;
            order.UpdatedByID = updatedById;
            _repository.UpdateOrderHeader(order);

            await _repository.SaveChangesAsync();

            return await GetByIdAsync(invoiceId);
        }
    }
}
