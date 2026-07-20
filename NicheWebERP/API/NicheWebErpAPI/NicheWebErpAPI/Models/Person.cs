namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Person - retail customers, sales representatives, and 2 legacy employee rows,
    // distinguished by EntityClassName. Employee logins were NOT migrated to this table's
    // UserName/PasswordHash - see ErpUser/ErpRole (Sprint 01) for ERP staff login instead. This
    // model is used for RetailCustomer rows only (Sprint 04).
    public class Person
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string EntityClassName { get; set; } = null!;

        // NOT NULL in the DB with no default - must always be explicitly written on insert.
        // No language-management screens exist yet, so this is set to Guid.Empty (a valid
        // logical "unset" placeholder, not a real Language row) on create - confirmed live in
        // Sprint 04. Revisit if/when a real default-language concept is needed.
        public Guid LanguageID { get; set; }

        // Also NOT NULL, no default - same placeholder reasoning as LanguageID. Retail
        // customers don't log in via the legacy UserIdentity mechanism (ERP staff use
        // ErpUser/ErpRole instead - see Sprint 01), so Guid.Empty means "no identity assigned".
        public Guid DefaultIdentityID { get; set; }

        // Also NOT NULL, no default. Looks like a legacy sequential customer number; no
        // business rule for assigning it is known yet, so it's left at 0 - confirmed live in
        // Sprint 04. No unique index was found on this column, so 0 for every new row is safe
        // for now, but revisit if a real "customer number" requirement surfaces later.
        public int PersonNo { get; set; }

        // Unique per CompanyID (SQL Server treats multiple NULLs as duplicates for this
        // index, same as Firm.Code) - retail customers don't log in here (see ErpUser for real
        // login), so this is just generated as a unique placeholder, never surfaced in the API.
        public string? UserName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? MobilePhone { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public Guid? CountryId { get; set; }
        public bool IsSuspended { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool? AllowCredit { get; set; }
        public DateTime? CreationDate { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
