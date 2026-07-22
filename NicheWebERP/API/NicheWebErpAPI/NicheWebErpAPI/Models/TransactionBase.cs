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
        // NOT NULL in the DB with no default - discovered live 2026-07-22 while testing Sprint
        // 10's Cashbook-balance integration (GenerateInvoice failed with "Cannot insert the value
        // NULL into column 'PreviousProcessID'"). A pre-existing gap dating back to Sprint 05,
        // never hit before because live testing hadn't exercised this exact path with a fresh
        // insert since this column was added to the physical schema. Every writer below sets it
        // to Guid.Empty, the same "no link" convention used everywhere else.
        public Guid PreviousProcessID { get; set; }
        public string? Narration { get; set; }
        public DateTime? TransactionDate { get; set; }

        // Logical link to Firm.EntityID (wholesale customer) - no DB foreign key exists
        public Guid OtherPartyID { get; set; }

        public int DocumentNumber1 { get; set; }
        public Guid LocationID { get; set; }
        public decimal Amount1 { get; set; }
        public Guid PricePointID { get; set; }
        // NOT NULL in the DB with no default - WebOrder/retail-storefront delivery-address
        // columns, irrelevant to wholesale/invoice/payment rows but still required by the
        // column. Set to Guid.Empty. Discovered live 2026-07-22 alongside PreviousProcessID.
        public Guid DeliveryCountryID { get; set; }
        public Guid AgentID { get; set; }
        public Guid RepresentativeID { get; set; }
        public bool IsDraft { get; set; }
        public decimal TaxAmount1 { get; set; }
        public int NumLines1 { get; set; }
        // NOT NULL, no default - discovered live 2026-07-22. Set to 0 by every writer below;
        // none of them track "lines that are specifically styled" as a distinct count from NumLines1.
        public int NumLinesOfStyle1 { get; set; }
        public decimal TotalWeight1 { get; set; }
        public int TotalQuantities1 { get; set; }
        public decimal SubTotalStyles1 { get; set; }
        public decimal SubTotalStylesTax1 { get; set; }
        // The remaining NOT NULL numeric columns below are all legacy stock-movement/rental
        // tracking fields with no equivalent concept in this app yet (no partial
        // shipment/fulfillment tracking, no rental module) - zeroed by every writer. Discovered
        // live 2026-07-22 (same INSERT failure investigation as PreviousProcessID above).
        public int TotalQuantitiesVariance1 { get; set; }
        public int TotalQuantitiesProcessed1 { get; set; }
        public int TotalQuantitiesHeld1 { get; set; }
        public int TotalQuantitiesTransitIn1 { get; set; }
        public int TotalQuantitiesRequiredOut1 { get; set; }
        public int TotalQuantitiesTransitOut1 { get; set; }
        public int TotalQuantitiesPurchaseOrder1 { get; set; }
        public int TotalRentalHeld1 { get; set; }
        public int TotalRentalOrder1 { get; set; }
        public int TotalRentalOut1 { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal OtherPartyCurrencyAmount { get; set; }
        public decimal OtherPartyExchangeRate { get; set; }
        public decimal CashbookCurrencyAmount { get; set; }
        // NOT NULL string, no default - real live value for a Firm counterparty is
        // "WholesaleCustomer" (confirmed via a live GROUP BY on WholesaleStockSalesOrder rows),
        // not the OtherPartyID's own logical type name - reused for Invoice/Payment rows too
        // since they also reference a Firm.
        public string OtherPartyClassName1 { get; set; } = null!;
        public int Sign1 { get; set; }
        public int TotalQuantitiesFaulty1 { get; set; }
        public int TotalQuantitiesSalesOrderPerson1 { get; set; }
        public int TotalQuantitiesSalesOrderFirm1 { get; set; }

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

        // Added Sprint 06 (Invoicing & Payments) - real columns, confirmed live via
        // INFORMATION_SCHEMA.COLUMNS. SalesOrderID links an invoice back to its source order
        // (invoice -> order, e.g. Amount1 above is the invoice's own total, not the order's).
        // PaymentMethodID/PaymentDueDate are used by Payment and Invoice rows respectively.
        public Guid? SalesOrderID { get; set; }
        public Guid? PaymentMethodID { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        // Added Sprint 10 (Finance: Cash Management) - which Cashbook a Payment settled into.
        // Real live data: 13,939 legacy "Payment" rows already have this set (SecurePay gateway
        // settlements) - confirmed via INFORMATION_SCHEMA.COLUMNS and a live GROUP BY. Now also
        // set (optionally) by PaymentService.RecordPaymentAsync so new payments recorded through
        // this app actually flow into a cashbook's balance, per this sprint's own dependency note.
        public Guid? CashbookID { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
