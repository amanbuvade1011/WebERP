namespace NicheWebErpAPI.Models
{
    // New table (Sprint 01) - deliberately NOT the legacy composite (CompanyID, EntityID)
    // pattern used elsewhere. Single int identity PK with a real FK constraint from
    // ErpUser.RoleId. Separate from the legacy Role/RolePermission framework on purpose.
    public class ErpRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<ErpUser> Users { get; set; } = new List<ErpUser>();
    }
}
