namespace NicheWebErpAPI.Models
{
    // Maps to dbo.TransactionLine - a line on a TransactionBase. Sprint 05 only creates
    // StyleTransactionLine rows (EntityClassName) - DiscountLine/FreightLine/PaymentLine are
    // out of scope here. See docs/ai-plan/01-database-map.md.
    public class TransactionLine
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid TransactionID { get; set; }
        public int? Sequence { get; set; }
        public string? Description { get; set; }
        public decimal LineAmountExTax1 { get; set; }
        public string EntityClassName { get; set; } = null!;
        public decimal LineTaxAmount1 { get; set; }

        // NOT NULL in the DB with no default - discovered live 2026-07-22 (same INSERT-failure
        // investigation as TransactionBase's gaps). No tax-jurisdiction or fault-type concept
        // exists in this app yet, and no line here is ever manually price-overridden - all
        // zeroed/Guid.Empty by every writer.
        public Guid TaxJurisdictionCategoryID { get; set; }
        public bool IsManualLine1 { get; set; }
        public decimal ManualLineAmountExTax { get; set; }
        public decimal ManualLineTax { get; set; }
        public Guid FaultTypeID { get; set; }
        public decimal DiscountExTax1 { get; set; }
        public decimal DiscountTax1 { get; set; }

        // Logical link to StyleSellLocation.EntityID (same CompanyID) - no DB foreign key exists
        public Guid? StyleSellLocationID { get; set; }
        public decimal? StylePriceExTax1 { get; set; }
        public Guid? PricePointID { get; set; }
        public decimal? StylePriceTax1 { get; set; }
        public int? LineQuantity1 { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
