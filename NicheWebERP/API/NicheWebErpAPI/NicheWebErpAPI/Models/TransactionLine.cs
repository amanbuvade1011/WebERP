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
