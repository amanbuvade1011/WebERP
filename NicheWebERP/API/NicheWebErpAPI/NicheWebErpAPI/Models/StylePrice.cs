namespace NicheWebErpAPI.Models
{
    // Maps to dbo.StylePrice - price for a Style at a given PricePoint.
    public class StylePrice
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid PricePointID { get; set; }
        public Guid StyleID { get; set; }

        public Guid? LocalTaxJurisdictionCategoryID { get; set; }
        public Guid? InternationalTaxJurisdictionCategoryID { get; set; }
        public Guid? LocalSalesRevenueGeneralLedgerAccountID { get; set; }
        public Guid? InternationalSalesRevenueGeneralLedgerAccountID { get; set; }

        public decimal LocalUnitPriceExTax1 { get; set; }
        public decimal LocalUnitPriceTax1 { get; set; }
        public decimal InternationalUnitPriceExTax1 { get; set; }
        public decimal InternationalUnitPriceTax1 { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
