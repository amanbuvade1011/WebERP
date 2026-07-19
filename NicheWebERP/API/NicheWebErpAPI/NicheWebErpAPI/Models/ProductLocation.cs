namespace NicheWebErpAPI.Models
{
    // Maps to dbo.ProductLocation
    // Primary key is composite: (CompanyID, EntityID) - configured in ERPDbContext.OnModelCreating
    public class ProductLocation
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        // Logical link to Product.EntityID (same CompanyID) - no DB foreign key exists for this
        public Guid? ProductID { get; set; }
        public Guid? StyleSellLocationID { get; set; }

        public int Held { get; set; }
        public int Allocated { get; set; }
        public int TransitIn { get; set; }
        public int RequiredOut { get; set; }
        public int TransitOut { get; set; }
        public int PurchaseOrder { get; set; }
        public int RentalHeld { get; set; }
        public int RentalOrder { get; set; }
        public int RentalOut { get; set; }
        public int SalesOrderFirm { get; set; }
        public int SalesOrderPerson { get; set; }
        public int AvailableFirm1 { get; set; }
        public int AvailablePerson1 { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
