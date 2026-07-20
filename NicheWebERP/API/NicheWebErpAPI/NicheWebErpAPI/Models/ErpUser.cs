namespace NicheWebErpAPI.Models
{
    // New table (Sprint 01) - internal ERP staff login. Deliberately separate from Person
    // (retail customers + reps) and Firm (wholesale customers) - see docs/ai-plan/01-database-map.md.
    // Uses a single int identity PK with real FK constraints (RoleId, UpdatedByUserId), unlike
    // the legacy composite (CompanyID, EntityID)/no-FK convention used by every other table.
    public class ErpUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public int RoleId { get; set; }
        public ErpRole? Role { get; set; }

        // Logical link to CompanyLocation.EntityID/CompanyID - not a real FK, because
        // CompanyLocation uses the legacy composite key and this table deliberately doesn't.
        public Guid CompanyId { get; set; }
        public Guid? LocationId { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedByUserId { get; set; }
        public ErpUser? UpdatedByUser { get; set; }

        // Bridges this int-keyed table to every other table's uniqueidentifier UpdatedByID
        // column. Generated once at creation, never reused/reassigned. NOT a Person.EntityID -
        // it never joins to Person. See docs/ai-plan/01-database-map.md for the full rationale.
        public Guid LegacyPersonId { get; set; }
    }
}
