namespace NicheWebErpAPI.Dtos
{
    public class FirmListItemDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string TradingName { get; set; } = null!;
        public string EntityClassName { get; set; } = null!;
        public decimal CreditLimit { get; set; }
        public bool Inactive { get; set; }
    }

    public class FirmDetailDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string TradingName { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string EntityClassName { get; set; } = null!;
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public Guid? CountryId { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? GeneralEmail { get; set; }
        public string? Fax { get; set; }
        public Guid? TermsId { get; set; }
        public string? TermsName { get; set; }
        public Guid? PricePointId { get; set; }
        public string? PricePointName { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal DiscountPercent1 { get; set; }
        public bool AllowOrder { get; set; }
        public bool AllowInvoice { get; set; }
        public decimal DepositPercent { get; set; }
        public bool Inactive { get; set; }
    }

    public class CreateFirmDto
    {
        public string TradingName { get; set; } = null!;
        public string? CompanyName { get; set; }
        // Unique per (CompanyID, EntityClassName) - SQL Server treats multiple NULLs as
        // duplicates for this index, so a code is always generated server-side if omitted
        // rather than left null (confirmed live in Sprint 04).
        public string? Code { get; set; }
        public string EntityClassName { get; set; } = "WholesaleCustomer";
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public Guid? CountryId { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? GeneralEmail { get; set; }
        public Guid? TermsId { get; set; }
        public Guid? PricePointId { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal DiscountPercent1 { get; set; }
        public bool AllowOrder { get; set; } = true;
        public bool AllowInvoice { get; set; } = true;
        public decimal DepositPercent { get; set; }
    }

    public class UpdateFirmDto
    {
        public string TradingName { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public Guid? CountryId { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? GeneralEmail { get; set; }
        public Guid? TermsId { get; set; }
        public Guid? PricePointId { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal DiscountPercent1 { get; set; }
        public bool AllowOrder { get; set; }
        public bool AllowInvoice { get; set; }
        public decimal DepositPercent { get; set; }
        public bool Inactive { get; set; }
    }
}
