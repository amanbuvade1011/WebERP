namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Product
    // Primary key is composite: (CompanyID, EntityID) - configured in ERPDbContext.OnModelCreating
    public class Product
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        // Logical link to Style.EntityID (same CompanyID) - no DB foreign key exists for this
        public Guid StyleID { get; set; }
        public Guid StyleColorID { get; set; }
        public Guid SizewayItemID { get; set; }

        public string? Barcode { get; set; }
        public bool? Inactive { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
