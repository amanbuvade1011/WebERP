namespace NicheWebErpAPI.Models
{
    // Maps to dbo.TransactionBase - generic transaction header. There is no separate
    // "SalesOrder"/"Invoice" table; every order/payment/invoice is a row here, discriminated by
    // EntityClassName (WholesaleStockSalesOrder | WebOrder | Payment - confirmed live values).
    // See docs/ai-plan/01-database-map.md. Only the columns Sprint 05 (Sales Orders) needs are
    // mapped - many other TransactionBase columns exist for Payment/Reconciliation/etc. and
    // are out of scope here.
    public class TransactionBase
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string EntityClassName { get; set; } = null!;
        public Guid LaneID { get; set; }
        public string? Narration { get; set; }
        public DateTime? TransactionDate { get; set; }

        // Logical link to Firm.EntityID (wholesale customer) - no DB foreign key exists
        public Guid OtherPartyID { get; set; }

        public int DocumentNumber1 { get; set; }
        public Guid LocationID { get; set; }
        public decimal Amount1 { get; set; }
        public Guid PricePointID { get; set; }
        public Guid AgentID { get; set; }
        public Guid RepresentativeID { get; set; }
        public bool IsDraft { get; set; }
        public decimal TaxAmount1 { get; set; }
        public int NumLines1 { get; set; }
        public int TotalQuantities1 { get; set; }
        public decimal SubTotalStyles1 { get; set; }
        public decimal SubTotalStylesTax1 { get; set; }

        // New status workflow for orders going forward - see docs/ai-plan/01-database-map.md
        // and docs/ai-plan/sprints/sprint-05-sales-orders.md for why: legacy Status1 data was
        // confirmed unused/unreliable, so this is a fresh, documented scheme, not inherited
        // legacy semantics. 0=Draft, 1=Confirmed, 2=Shipped, 3=Delivered, 4=Cancelled.
        public int? Status1 { get; set; }
        public int? Status2 { get; set; }

        public Guid? CurrencyID { get; set; }
        public Guid? TermsID { get; set; }
        public string? CustomerReferenceNo { get; set; }
        public Guid EmployeeID { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
