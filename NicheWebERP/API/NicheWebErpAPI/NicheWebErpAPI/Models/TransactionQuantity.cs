namespace NicheWebErpAPI.Models
{
    // Maps to dbo.TransactionQuantity - per-location quantity breakdown of a TransactionLine.
    // Sprint 05 only creates SalesOrderQuantity rows (the only live EntityClassName value - see
    // docs/ai-plan/01-database-map.md).
    public class TransactionQuantity
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid TransactionLineID { get; set; }
        public string EntityClassName { get; set; } = null!;
        public int Quantity { get; set; }

        // Logical link to ProductLocation.EntityID (same CompanyID) - no DB foreign key exists
        public Guid ProductLocationID { get; set; }

        public int Held { get; set; }
        public int RequiredOut { get; set; }
        public int Allocated { get; set; }
        public int SalesOrderFirm { get; set; }
        public int SalesOrderPerson { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
