namespace NicheWebErpAPI.Dtos
{
    // Status workflow decided in Sprint 05 (see docs/ai-plan/sprints/sprint-05-sales-orders.md) -
    // reuses TransactionBase.Status1 for new orders only. Historic rows predate this scheme and
    // must not be reinterpreted through it.
    public enum SalesOrderStatus
    {
        Draft = 0,
        Confirmed = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }

    public class SalesOrderListItemDto
    {
        public Guid Id { get; set; }
        public int DocumentNumber { get; set; }
        public Guid FirmId { get; set; }
        public string FirmName { get; set; } = null!;
        public DateTime? OrderDate { get; set; }
        public string? CustomerReferenceNo { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; } = null!;
        public int TotalQuantities { get; set; }
        public decimal TotalAmount { get; set; }

        // Added Sprint 06 - billing status is an independent axis from fulfillment Status1
        // (a Shipped order can still need invoicing), so it reuses the previously-unused
        // Status2 column rather than extending the Status1 enum. See InvoiceService.
        public bool IsInvoiced { get; set; }
    }

    public class SalesOrderDetailDto
    {
        public Guid Id { get; set; }
        public int DocumentNumber { get; set; }
        public Guid FirmId { get; set; }
        public string FirmName { get; set; } = null!;
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public Guid PricePointId { get; set; }
        public string? PricePointName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? CustomerReferenceNo { get; set; }
        public string? Narration { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; } = null!;
        public decimal SubTotalExTax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalQuantities { get; set; }
        public bool IsInvoiced { get; set; }
        // Added Sprint 07 - already folded into TotalAmount, exposed separately so the UI can
        // show them as their own line items on the order.
        public decimal DiscountAmount { get; set; }
        public decimal FreightAmount { get; set; }
        public List<SalesOrderLineDto> Lines { get; set; } = new();
    }

    public class SalesOrderLineDto
    {
        public Guid LineId { get; set; }
        public Guid ProductId { get; set; }
        public string StyleCode { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string SizeDescription { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPriceExTax { get; set; }
        public decimal UnitPriceTax { get; set; }
        public decimal LineTotalExTax { get; set; }
        public decimal LineTotalTax { get; set; }
    }

    public class CreateSalesOrderDto
    {
        public Guid FirmId { get; set; }
        public Guid LocationId { get; set; }
        public string? CustomerReferenceNo { get; set; }
        public string? Narration { get; set; }
        public string? CouponCode { get; set; }
        public List<CreateSalesOrderLineDto> Lines { get; set; } = new();
    }

    // Deliberately has no price field - Acceptance criterion: pricing always comes from
    // StylePrice server-side, a client-submitted price would be ignored so it's not exposed here.
    public class CreateSalesOrderLineDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddSalesOrderLineDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateSalesOrderLineDto
    {
        public int Quantity { get; set; }
    }

    public class UpdateSalesOrderStatusDto
    {
        public int Status { get; set; }
    }
}
