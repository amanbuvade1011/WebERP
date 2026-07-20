namespace NicheWebErpAPI.Models
{
    // Maps to dbo.PricePoint - a named price list/channel (Wholesale, Retail, RRP, Web, Agent).
    public class PricePoint
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string? Name { get; set; }
        public Guid TaxJurisdictionID { get; set; }
        public bool? TaxInclusive { get; set; }
        public Guid DefaultLocalTaxJurisdictionCategoryID { get; set; }
        public Guid DefaultInternationalTaxJurisdictionCategoryID { get; set; }
        public Guid DefaultLocalSalesRevenueGeneralLedgerAccountID { get; set; }
        public Guid DefaultInternationalSalesRevenueGeneralLedgerAccountID { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
