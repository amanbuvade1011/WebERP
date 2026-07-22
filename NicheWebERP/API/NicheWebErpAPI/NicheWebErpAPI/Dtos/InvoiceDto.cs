namespace NicheWebErpAPI.Dtos
{
    // Invoice status is computed, never stored - see SalesOrderRepository's sibling
    // InvoiceRepository.GetPagedAsync/GetDetailAsync for the Paid/Pending/Overdue derivation.
    // No "Draft": GenerateInvoice is a one-click, one-shot action (see
    // docs/ai-plan/sprints/sprint-06-invoicing-payments.md), not a two-step draft/issue flow.
    public enum InvoiceStatus
    {
        Paid = 0,
        Pending = 1,
        Overdue = 2
    }

    public class InvoiceListItemDto
    {
        public Guid Id { get; set; }
        public int DocumentNumber { get; set; }
        public Guid FirmId { get; set; }
        public string FirmName { get; set; } = null!;
        public Guid? SalesOrderId { get; set; }
        public int? SalesOrderDocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
    }

    public class InvoiceDetailDto
    {
        public Guid Id { get; set; }
        public int DocumentNumber { get; set; }
        public Guid FirmId { get; set; }
        public string FirmName { get; set; } = null!;
        public Guid? SalesOrderId { get; set; }
        public int? SalesOrderDocumentNumber { get; set; }
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? CustomerReferenceNo { get; set; }
        public string? Narration { get; set; }
        public decimal SubTotalExTax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
        public List<InvoiceLineDto> Lines { get; set; } = new();
        public List<PaymentSummaryDto> Payments { get; set; } = new();
    }

    public class InvoiceLineDto
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

    public class PaymentSummaryDto
    {
        public Guid PaymentId { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? Narration { get; set; }
    }
}
