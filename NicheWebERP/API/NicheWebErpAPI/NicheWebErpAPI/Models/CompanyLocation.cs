namespace NicheWebErpAPI.Models
{
    // Maps to dbo.CompanyLocation
    // Primary key is composite: (CompanyID, EntityID) - configured in ERPDbContext.OnModelCreating.
    // Nested-set hierarchy (AdministrationLeftTag/RightTag/ParentID) of locations under a
    // single master company. Only 1 row exists today (EntityClassName = "MasterCompany").
    public class CompanyLocation
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string EntityClassName { get; set; } = null!;
        public string? Name { get; set; }
        public string? Code { get; set; }

        public Guid AdministrationParentID { get; set; }
        public int AdministrationLeftTag { get; set; }
        public int AdministrationRightTag { get; set; }

        // Logical links to other tables (same CompanyID) - no DB foreign key exists for these
        public Guid LanguageID { get; set; }
        public Guid? DefaultPricePointWholesaleID { get; set; }
        public Guid? DefaultPricePointRRPID { get; set; }
        public Guid? DefaultPricePointRetailID { get; set; }
        public Guid? DefaultPricePointWebID { get; set; }
        public Guid? DefaultTaxJurisdictionID { get; set; }
        public Guid CountryID { get; set; }
        public Guid FreightID { get; set; }
        public Guid? OwnerID { get; set; }

        public string? Fax { get; set; }
        public string? GeneralEmail { get; set; }
        public string? CompanyNumber1 { get; set; }
        public string? CompanyNumber2 { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public int? NumLicenses { get; set; }

        public bool Inactive { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
