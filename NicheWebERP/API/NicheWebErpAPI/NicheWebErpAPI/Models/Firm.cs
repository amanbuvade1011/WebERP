namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Firm - wholesale customers and agents, distinguished by EntityClassName
    // (WholesaleCustomer | Agent). See docs/ai-plan/01-database-map.md.
    public class Firm
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string EntityClassName { get; set; } = null!;
        public string TradingName { get; set; } = null!;
        public string? CompanyName { get; set; }
        public Guid? MainContactID { get; set; }
        public bool Inactive { get; set; }
        public Guid PricePointID { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public Guid CountryID { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? GeneralEmail { get; set; }
        public string? Fax { get; set; }
        public string? CompanyNumber1 { get; set; }
        public string? CompanyNumber2 { get; set; }
        public string? Comment { get; set; }
        public Guid TermsID { get; set; }
        public string? Code { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal DiscountPercent1 { get; set; }
        public Guid ParentFirmID { get; set; }
        public Guid LocationID { get; set; }
        public bool AllowOrder { get; set; }
        public bool AllowInvoice { get; set; }
        public decimal DepositPercent { get; set; }
        public decimal MinimumOrderAmount { get; set; }

        // NOT NULL in the DB with no default - must always be explicitly written on insert or
        // the raw SQL INSERT fails ("Cannot insert the value NULL"), confirmed live in Sprint 04.
        public bool DenyLabelsByDefault { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
